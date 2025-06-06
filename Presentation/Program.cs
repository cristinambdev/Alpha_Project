using Business.Services;
using Data.Contexts;
using Data.Entities;
using Data.Repositories;
using Presentation.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
// By chat gpt- Load the default appsettings.json (already happens implicitly, but it is explicitly here)
// Then load  secrets file � optional: true means "OK if it's missing"
builder.Configuration
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(x => 
    x.UseSqlServer(builder.Configuration.GetConnectionString("AlphaDB"))
);

builder.Services.AddIdentity<UserEntity, IdentityRole>(x =>
{
    x.SignIn.RequireConfirmedAccount = false;
    x.User.RequireUniqueEmail = true;
    x.Password.RequiredLength = 8;

})

.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<RoleManager<IdentityRole>>();  // suggested by Chat GPT as UserService depends on RoleManager<UserEntity>

builder.Services.ConfigureApplicationCookie(x =>
{
    x.LoginPath = "/auth/signin";
    x.AccessDeniedPath = "/auth/denied";
    x.Cookie.HttpOnly = true;
    x.Cookie.IsEssential = true;
    x.ExpireTimeSpan = TimeSpan.FromHours(1);
    x.SlidingExpiration = true;
    x.Cookie.SameSite = SameSiteMode.None;
    x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});


builder.Services.AddAuthentication(x =>
{
    x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

})
    .AddCookie()
    .AddGoogle(x =>
    {
        x.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        x.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        x.CallbackPath = "/signin-google";


        //byt Chat GPT - Override the default Google authentication events so that the Google screen option shows every time.
        x.Events = new OAuthEvents
        {
            OnRedirectToAuthorizationEndpoint = context =>
            {
                context.Response.Redirect(
                    $"{context.RedirectUri}&prompt=select_account");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMiniProjectRepository, MiniProjectRepository>();



builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMiniProjectService, MiniProjectService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => !context.Request.Cookies.ContainsKey("cookieConsent");
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

var app = builder.Build();

app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();


app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

//ROLE
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "User" };

    foreach (var roleName in roleNames)
    {
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
    var user = new UserEntity { UserName = "admin@domain.com", Email = "admin@domain.com", UserImage = "/images/logo_img.svg" };

    var userExists = await userManager.Users.AnyAsync( x => x.Email == user.Email );
    if (!userExists)
    {
        var result = await userManager.CreateAsync(user, "ChangeMe123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, "Admin");

    }

}

app.MapStaticAssets();
app.UseStaticFiles();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapControllers();
app.UseRewriter(new RewriteOptions().AddRedirect("^$", "/admin/overview"));

app.MapHub<NotificationHub>("/notificationHub");
app.Run();
