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
    public virtual DbSet<UserAddressEntity> UserAddreses { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public DbSet<ProjectUserEntity> ProjectUsers { get; set; }
    public DbSet<ProjectClientEntity> ProjectClients { get; set; }

    //Model Creating with help from Chat GPT
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This line ensures Identity-related configurations (like primary keys) are properly applied
        base.OnModelCreating(modelBuilder);

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

     
        //feed the database with values for status - chat gpt
        modelBuilder.Entity<StatusEntity>().HasData(
            new StatusEntity { Id = 1, StatusName = "Not Started" },
            new StatusEntity { Id = 2, StatusName = "In Progress" },
            new StatusEntity { Id = 3, StatusName = "Completed" },
            new StatusEntity { Id = 4, StatusName = "On Hold" }
            );
        

    }
}
