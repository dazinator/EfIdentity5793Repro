using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfIdentity5793Repro
{
    /// <summary>
    /// This one works becuase it inherits from base type there is some magic detection:
    /// See here: https://github.com/aspnet/AspNetCore/blob/bfec2c14be1e65f7dd361a43950d4c848ad0cd35/src/Identity/EntityFrameworkCore/src/IdentityEntityFrameworkBuilderExtensions.cs#L54
    /// 
    /// </summary>
    //public class ReproDbContext : IdentityDbContext<User, Role, int>
    //{

    //    public ReproDbContext(DbContextOptions<ReproDbContext> options)
    //   : base(options)
    //    {
    //    }

    //}

    /// <summary>
    /// See https://github.com/aspnet/AspNetIdentity/blob/master/src/Microsoft.AspNet.Identity.EntityFramework/IdentityDbContext.cs
    /// </summary>
    public class ReproDbContext : DbContext
    {

        public ReproDbContext(DbContextOptions<ReproDbContext> options)
       : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<RoleClaim> RoleClaims { get; set; }
        public virtual DbSet<UserClaim> UserClaims { get; set; }

      
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(b =>
            {
                // b.ToTable("User");
                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
                b.HasMany<UserClaim>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();

                b.Property(u => u.UserName).HasMaxLength(256);
                b.Property(u => u.NormalizedUserName).HasMaxLength(256);
                b.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex").IsUnique();

                b.Property(u => u.Email).HasMaxLength(256);
                b.Property(u => u.NormalizedEmail).HasMaxLength(256);
                b.HasIndex(u => u.NormalizedEmail).HasName("UserEmailIndex").IsUnique();

                b.Property(u => u.PasswordHash).HasMaxLength(256);
                b.Property(u => u.SecurityStamp).HasMaxLength(100);
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken().HasMaxLength(36);

                b.Property(u => u.PhoneNumber).HasMaxLength(15);

            });

            builder.Entity<Role>(b =>
            {
                b.HasKey(r => r.Id);
                b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();
                //  b.ToTable("Role");
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken().HasMaxLength(36);

                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasMany<RoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
            });

            builder.Entity<RoleClaim>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.Property(rc => rc.ClaimType).HasMaxLength(100);
                b.Property(rc => rc.ClaimValue).HasMaxLength(256);
                // b.ToTable("AspNetRoleClaims");
            });

            builder.Entity<UserClaim>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.Property(rc => rc.ClaimType).HasMaxLength(100);
                b.Property(rc => rc.ClaimValue).HasMaxLength(256);
                // b.ToTable("AspNetRoleClaims");
            });



            builder.Entity<UserRole>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                //  b.ToTable("AspNetUserRoles");
            });          

        }
    }

}
