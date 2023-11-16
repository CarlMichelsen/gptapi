using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Database;

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

    public required DbSet<Conversation> Conversations { get; init; }

    public required DbSet<OAuthRecord> OAuthRecords { get; init; }
}
