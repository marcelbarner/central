using Central.Domain.Users;
using Central.Infrastructure.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Persistence;

/// <summary>
/// Application database context with Identity support.
/// Uses long as the primary key type for all Identity entities.
/// </summary>
public sealed class ApplicationDbContext : IdentityDbContext<
    User,
    IdentityRole<long>,
    long,
    IdentityUserClaim<long>,
    IdentityUserRole<long>,
    IdentityUserLogin<long>,
    IdentityRoleClaim<long>,
    IdentityUserToken<long>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Documents DbSet.
    /// </summary>
    public DbSet<DocumentEntity> Documents => Set<DocumentEntity>();

    /// <summary>
    /// Gets or sets the Tags DbSet.
    /// </summary>
    public DbSet<TagEntity> Tags => Set<TagEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all IEntityTypeConfiguration implementations from this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure Identity tables with custom names
        builder.Entity<IdentityRole<long>>().ToTable("Roles");
        builder.Entity<IdentityUserClaim<long>>().ToTable("UserClaims");
        builder.Entity<IdentityUserRole<long>>().ToTable("UserRoles");
        builder.Entity<IdentityUserLogin<long>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<long>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<long>>().ToTable("UserTokens");
    }
}