using Microsoft.EntityFrameworkCore;
using Skwela.Domain.Entities;
using Skwela.Domain.Enums;

namespace Skwela.Infrastructure.Data;

/// <summary>
/// Application Database Context
/// Entity Framework Core DbContext for Skwela learning management system
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
    
    // Future entities - uncomment when ready to implement
    //public DbSet<Assignment> Assignments => Set<Assignment>();
    //public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity mapping
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.user_id); // Primary key

            entity.Property(e => e.username);
            entity.HasIndex(e => e.username).IsUnique();
            entity.Property(e => e.email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.email).IsUnique();
            entity.Property(e => e.password).IsRequired();
            entity.Property(e => e.display_name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.display_image);
            entity.Property(e => e.role).IsRequired(); // Teacher or Student enum
            entity.Property(e => e.is_email_verified);
            entity.Property(e => e.refreshToken);
            entity.Property(e => e.refreshTokenExpiryTime);
            entity.Property(e => e.user_created_at).IsRequired();
            entity.HasIndex(e => e.role); // Index for role-based queries
        });

        // Configure Classroom entity mapping
        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.class_id); // Primary key
            entity.Property(e => e.class_name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.class_description).HasMaxLength(1000);
            // Foreign key relationship to User (teacher)
            entity.HasOne(e => e.user)
                  .WithMany(u => u.classrooms)
                  .HasForeignKey(e => e.user_id)
                  .OnDelete(DeleteBehavior.Cascade); // Delete classrooms when teacher is deleted
            entity.Property(e => e.class_created_at).IsRequired();
            entity.HasIndex(e => e.user_id); // Index for finding classrooms by teacher
            entity.HasIndex(e => e.class_id); // Index for direct classroom lookup
        });

        // Configure Enrollment entity mapping
        modelBuilder.Entity<Enrollment>(entity =>
        {
            // Composite primary key: class_id and user_id
            entity.HasKey(e => e.class_id);
            entity.HasKey(e => e.user_id);
            entity.Property(e => e.enrolled_at).IsRequired();
            entity.Property(e => e.enrolled_status).IsRequired().HasMaxLength(50); // "active" or "inactive"
        });
    }
}