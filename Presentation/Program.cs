using Business.Services;
using Data.Contexts;
using Data.Entities;
using Data.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

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


builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();


var app = builder.Build();

app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.UseRewriter(new RewriteOptions().AddRedirect("^$", "/admin/overview"));
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
