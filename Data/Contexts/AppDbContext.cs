using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserEntity, IdentityRole, string>(options)
{
    public virtual DbSet<ClientEntity> Clients { get; set; }

    public virtual DbSet<StatusEntity> Statuses { get; set; }

    public virtual DbSet<ProjectEntity> Projects { get; set; }
    public virtual DbSet<MiniProjectEntity> MiniProjects { get; set; }
    public virtual DbSet<UserAddressEntity> UserAddresses { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<ProjectUserEntity> ProjectUsers { get; set; }
    public virtual DbSet<ProjectClientEntity> ProjectClients { get; set; }
    public virtual DbSet<NotificationEntity> Notifications { get; set; }
    public virtual DbSet<NotificationDismissedEntity> DismissedNotifications { get; set; }
    public virtual DbSet<NotificationTypeEntity> NotificationTypes { get; set; }
    public virtual DbSet<NotificationTargetGroupEntity> NotificationTargetGroups { get; set; }



    //Model Creating with help from Chat GPT
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<Domain.Models.UserAddress>();

        modelBuilder.Entity<UserAddressEntity>()
            .HasKey(a => a.UserId);

        modelBuilder.Entity<UserEntity>()
            .HasOne(u => u.Address)
            .WithOne(a => a.User)
            .HasForeignKey<UserAddressEntity>(a => a.UserId);

        // Configuring the ProjectUserEntity
        modelBuilder.Entity<ProjectUserEntity>()
            .HasKey(pu => new { pu.ProjectId, pu.UserId });

        modelBuilder.Entity<ProjectUserEntity>()
            .HasOne(pu => pu.Project)
            .WithMany(p => p.ProjectUsers)
            .HasForeignKey(pu => pu.ProjectId);

        modelBuilder.Entity<ProjectUserEntity>()
            .HasOne(pu => pu.User)
            .WithMany(u => u.ProjectUsers)
            .HasForeignKey(pu => pu.UserId);

        // Configuring the ProjectClientEntity
        modelBuilder.Entity<ProjectClientEntity>()
            .HasKey(pc => new { pc.ProjectId, pc.ClientId });

        modelBuilder.Entity<ProjectClientEntity>()
            .HasOne(pc => pc.Project)
            .WithMany(p => p.ProjectClients)
            .HasForeignKey(pc => pc.ProjectId);

        modelBuilder.Entity<ProjectClientEntity>()
            .HasOne(pc => pc.Client)
            .WithMany(c => c.ProjectClients)
            .HasForeignKey(pc => pc.ClientId);

        modelBuilder.Entity<NotificationDismissedEntity>()
                  .HasOne(nd => nd.User)
                  .WithMany(u => u.DismissedNotifications)
                  .HasForeignKey(nd => nd.UserId);

        //feed the database with values for status - chat gpt
        modelBuilder.Entity<StatusEntity>().HasData(
            new StatusEntity { Id = 1, StatusName = "Not Started" },
            new StatusEntity { Id = 2, StatusName = "In Progress" },
            new StatusEntity { Id = 3, StatusName = "Completed" },
            new StatusEntity { Id = 4, StatusName = "On Hold" }
            );
        

    }
}
