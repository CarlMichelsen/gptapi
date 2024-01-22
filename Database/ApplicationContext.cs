using Domain.Entity;
using Domain.Entity.Id;
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

    public DbSet<Conversation> Conversation { get; init; }

    public DbSet<UserProfile> UserProfile { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>(entity =>
        {
            // Configure the primary key
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasConversion(
                    id => id.Value, // How to convert to Guid
                    guid => new UserProfileId(guid)); // How to convert from Guid
        });
        
        modelBuilder.Entity<Conversation>(entity =>
        {
            // Configure the primary key
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasConversion(
                    id => id.Value, // How to convert to Guid
                    guid => new ConversationId(guid)); // How to convert from Guid
        });

        modelBuilder.Entity<Message>(entity =>
        {
            // Configure the primary key
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasConversion(
                    id => id.Value, // How to convert to Guid
                    guid => new MessageId(guid)); // How to convert from Guid

            entity.HasOne(c => c.PreviousMessageId)
                .WithMany()
                .HasForeignKey(c => c.PreviousMessageId);
        });
    }
}
