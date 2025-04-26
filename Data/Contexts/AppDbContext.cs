using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserEntity, IdentityRole, string>(options)
{
    public virtual DbSet<ClientEntity> Clients { get; set; }

    public virtual DbSet<StatusEntity> Statuses { get; set; }

    public virtual DbSet<ProjectEntity> Projects { get; set; }

    public virtual DbSet<UserAddressEntity> UserAddreses { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

}

