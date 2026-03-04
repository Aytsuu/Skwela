using Microsoft.EntityFrameworkCore;
using eSECAI.Domain.Entities;

namespace eSECAI.Infrastructure.Data;

/// <summary>
/// Application Database Context
/// Entity Framework Core DbContext for eSECAI learning management system
/// Handles database configuration, migrations, and entity relationships
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes the AppDbContext with database options
    /// </summary>
    /// <param name="options">Database context options from dependency injection</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Classroom> Classrooms => Set<Classroom>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Posting> Postings => Set<Posting>();
    public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity mapping
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.user_id); // Primary key
            entity.Property(e => e.email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.email).IsUnique();
            entity.Property(e => e.password).IsRequired();
            entity.Property(e => e.display_name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.display_image);
            entity.Property(e => e.is_admin).HasDefaultValue(false);
            entity.Property(e => e.is_email_verified).HasDefaultValue(false);
            entity.Property(e => e.refreshToken);
            entity.Property(e => e.refreshTokenExpiryTime);
            entity.Property(e => e.user_created_at);
            entity.Property(e => e.user_updated_at);
        });

        // Configure Classroom entity mapping
        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.class_id); // Primary key
            entity.Property(e => e.class_name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.class_description).HasMaxLength(1000);
            entity.Property(e => e.class_image);
            entity.Property(e => e.class_is_archived).HasDefaultValue(false);
            // Foreign key relationship to User (teacher)
            entity.HasOne(e => e.user)
                .WithMany(e => e.classrooms)
                .HasForeignKey(e => e.user_id)
                .OnDelete(DeleteBehavior.Cascade) // Delete classrooms when teacher is deleted
                .IsRequired();
            entity.Property(e => e.class_created_at);
            entity.Property(e => e.class_updated_at);
            entity.HasIndex(e => e.user_id); // Index for finding classrooms by teacher
            entity.HasIndex(e => e.class_id); // Index for direct classroom lookup
        });

        // Configure Enrollment entity mapping
        modelBuilder.Entity<Enrollment>(entity =>
        {

            entity.HasKey(e => e.enroll_id);
            entity.Property(e => e.enroll_is_approved).HasDefaultValue(false);
            entity.Property(e => e.enroll_created_at);
            entity.HasOne(e => e.classroom)
                .WithMany()
                .HasForeignKey(e => e.class_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            entity.HasOne(e => e.user)
                .WithMany()
                .HasForeignKey(e => e.user_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        // Configure Posting entity mapping
        modelBuilder.Entity<Posting>(entity => 
        {
            entity.HasKey(e => e.post_id);
            entity.Property(e => e.post_heading).IsRequired().HasMaxLength(200);
            entity.Property(e => e.post_details).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.post_created_at);
            entity.Property(e => e.post_updated_at);
            entity.HasOne(e => e.classroom)
                .WithMany()
                .HasForeignKey(e => e.class_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity<Submission>(entity => 
        {
            entity.HasKey(e => e.sub_id);
            entity.Property(e => e.sub_score).HasDefaultValue(0f);
            entity.Property(e => e.sub_remark);
            entity.Property(e => e.sub_created_at);
            entity.HasOne(e => e.user)
                .WithMany()
                .HasForeignKey(e => e.user_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            entity.HasOne(e => e.posting)
                .WithMany()
                .HasForeignKey(e => e.post_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {   
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

        foreach (var entityEntry in entries)
        {
            // Get ALL properties for this specific entity, then find the one ending in "updated_at"
            var updatedAtProperty = entityEntry.Metadata.GetProperties()
                .FirstOrDefault(p => p.Name.EndsWith("updated_at", StringComparison.OrdinalIgnoreCase));
            
            // If we found one, update it
            if (updatedAtProperty != null && updatedAtProperty.ClrType == typeof(DateTime))
            {
                if (entityEntry.State == EntityState.Modified)
                {
                    // We use updatedAtProperty.Name to dynamically pass the exact property name we found
                    entityEntry.Property(updatedAtProperty.Name).CurrentValue = DateTime.UtcNow;
                }
            }
            
            // For created_at
            var createdAtProperty = entityEntry.Metadata.GetProperties()
                .FirstOrDefault(p => p.Name.EndsWith("created_at", StringComparison.OrdinalIgnoreCase));

            if (createdAtProperty != null && createdAtProperty.ClrType == typeof(DateTime))
            {
                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property(createdAtProperty.Name).CurrentValue = DateTime.UtcNow;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}