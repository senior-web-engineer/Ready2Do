using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;


namespace PalestreGoGo.IdentityModel
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        const string IDENTITY_SCHEMA_NAME = "security";

        public AppIdentityDbContext() : base()
        {

        }

        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            /*Remapping tabelle identity sulle nostre tabelle */
            builder.Entity<AppUser>(entity =>
            {
                entity.ToTable("Users", IDENTITY_SCHEMA_NAME);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            });
            builder.Entity<AppRole>(entity =>
            {
                entity.ToTable("Roles", IDENTITY_SCHEMA_NAME);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            });
            builder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.ToTable("UserRoles", IDENTITY_SCHEMA_NAME);
            });
            builder.Entity<IdentityUserClaim<Guid>>(entity =>
            {
                entity.ToTable("UserClaims", IDENTITY_SCHEMA_NAME);
            });
            builder.Entity<IdentityUserLogin<Guid>>(entity =>
            {
                entity.ToTable("UserLogins", IDENTITY_SCHEMA_NAME);
            });
            builder.Entity<IdentityRoleClaim<Guid>>(entity =>
            {
                entity.ToTable("RoleClaims", IDENTITY_SCHEMA_NAME);
            });
            builder.Entity<IdentityUserToken<Guid>>(entity =>
            {
                entity.ToTable("UserTokens", IDENTITY_SCHEMA_NAME);
            });

        }
    }
}
