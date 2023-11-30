using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Database;

/// <summary>
/// EntityFramework application context.
/// </summary>
public sealed class ApplicationContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationContext"/> class.
    /// </summary>
    /// <param name="options">Options for datacontext.</param>
    public ApplicationContext(
        DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>()
            .HasIndex(e => e.AuthenticationId)
            .IsUnique();
    }

    public DbSet<Conversation> Conversations { get; init; }

    public DbSet<OAuthRecord> OAuthRecords { get; init; }

    public DbSet<UserProfile> UserProfiles { get; init; }
}
