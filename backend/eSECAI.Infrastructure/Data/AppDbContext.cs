using Microsoft.EntityFrameworkCore;
using esecai.Domain.Entities;

namespace esecai.Infrastructure.Data;

/// <summary>
/// Application Database Context
/// Entity Framework Core DbContext for esecai learning management system
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
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Record> Records => Set<Record>();
    public DbSet<RecordAnswer> RecordAnswers => Set<RecordAnswer>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Release> Releases => Set<Release>();
    public DbSet<UserReleaseRead> UserReleaseReads => Set<UserReleaseRead>();
    public DbSet<UserReleaseDismiss> UserReleaseDismisses => Set<UserReleaseDismiss>();

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
            entity.Property(e => e.class_banner);
            entity.Property(e => e.class_is_archived).HasDefaultValue(false);
            entity.HasOne(e => e.user)
                .WithMany(e => e.classrooms)
                .HasForeignKey(e => e.user_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            entity.Property(e => e.class_created_at);
            entity.Property(e => e.class_updated_at);
            entity.HasIndex(e => e.user_id); // Index for finding classrooms by teacher
            entity.HasIndex(e => e.class_id); // Index for direct classroom lookup
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.student_id);
            entity.Property(e => e.student_fname).IsRequired().HasMaxLength(150);
            entity.Property(e => e.student_mname).HasMaxLength(150);
            entity.Property(e => e.student_lname).IsRequired().HasMaxLength(150);
            entity.Property(e => e.student_created_at);
            entity.Property(e => e.student_updated_at);
            entity.HasOne(e => e.classroom)
                .WithMany(e => e.students)
                .HasForeignKey(e => e.class_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            entity.HasIndex(e => new { e.class_id, e.student_fname, e.student_mname, e.student_lname }).IsUnique();
        });

        modelBuilder.Entity<Assessment>(entity => 
        {
            entity.HasKey(e => e.ass_id);
            entity.Property(e => e.ass_title);
            entity.Property(e => e.ass_type).HasMaxLength(20);
            entity.Property(e => e.ass_answer_key_url);
            entity.Property(e => e.ass_instruction);
            entity.Property(e => e.ass_total_points);
            entity.Property(e => e.ass_status).HasDefaultValue("draft");
            entity.Property(e => e.ass_created_at);
            entity.Property(e => e.ass_updated_at);
            entity.HasOne(e => e.classroom)
                .WithMany(e => e.assessments)
                .HasForeignKey(e => e.class_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            entity.HasIndex(e => e.ass_id);
        });

        modelBuilder.Entity<Question>(entity => 
        {
            entity.HasKey(e => e.quest_id);
            entity.Property(e => e.quest_num);
            entity.Property(e => e.quest_type).HasMaxLength(20);
            entity.Property(e => e.quest_text);
            entity.Property(e => e.quest_correct_answer);
            entity.Property(e => e.quest_rubric);
            entity.Property(e => e.quest_max_points);
            entity.Property(e => e.quest_ai_confidence);
            entity.HasOne(e => e.assessment)
                .WithMany(e => e.questions)
                .HasForeignKey(e => e.ass_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity<Record>(entity => 
        {
            entity.HasKey(e => e.rec_id);
            entity.Property(e => e.rec_scan_url);
            entity.Property(e => e.rec_total_score);
            entity.Property(e => e.rec_percentage);
            entity.Property(e => e.rec_status).HasDefaultValue("pending");
            entity.Property(e => e.rec_graded_at);
            entity.Property(e => e.rec_created_at);
            entity.HasOne(e => e.assessment)
                .WithMany(e => e.records)
                .HasForeignKey(e => e.ass_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            entity.HasOne(e => e.student)
                .WithMany(e => e.records)
                .HasForeignKey(e => e.student_id)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });

        modelBuilder.Entity<RecordAnswer>(entity => 
        {
            entity.HasKey(e => e.ra_id);
            entity.Property(e => e.ra_student_ans);
            entity.Property(e => e.ra_awarded_pts);
            entity.Property(e => e.ra_ai_confidence);
            entity.Property(e => e.ra_feedback);
            entity.Property(e => e.ra_teacher_rev).HasDefaultValue(false);
            entity.Property(e => e.ra_teacher_op);
            entity.Property(e => e.ra_raw_response);
            entity.HasOne(e => e.question)
                .WithMany(e => e.record_answers)
                .HasForeignKey(e => e.quest_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            entity.HasOne(e => e.record)
                .WithMany(e => e.record_answers)
                .HasForeignKey(e => e.rec_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });
        
        modelBuilder.Entity<Notification>(entity => 
        {
            entity.HasKey(e => e.notif_id);
            entity.HasOne(e => e.user)
                .WithMany()
                .HasForeignKey(e => e.user_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            entity.Property(e => e.notif_title);
            entity.Property(e => e.notif_message);
            entity.Property(e => e.notif_is_read);
            entity.Property(e => e.notif_created_at);
        });

        modelBuilder.Entity<Release>(entity =>
        {
            entity.HasKey(e => e.release_id);
            entity.Property(e => e.release_version).HasMaxLength(50);
            entity.Property(e => e.release_title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.release_summary).IsRequired().HasMaxLength(400);
            entity.Property(e => e.release_body).IsRequired();
            entity.Property(e => e.release_status).IsRequired().HasMaxLength(20).HasDefaultValue("draft");
            entity.Property(e => e.release_created_at);
            entity.Property(e => e.release_updated_at);
            entity.Property(e => e.release_published_at);
            entity.HasIndex(e => e.release_status);
            entity.HasIndex(e => e.release_published_at);
            entity.HasOne(e => e.created_by_user)
                .WithMany()
                .HasForeignKey(e => e.created_by_user_id)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            entity.HasOne(e => e.published_by_user)
                .WithMany()
                .HasForeignKey(e => e.published_by_user_id)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        });

        modelBuilder.Entity<UserReleaseRead>(entity =>
        {
            entity.HasKey(e => e.user_release_read_id);
            entity.Property(e => e.read_at);
            entity.HasIndex(e => new { e.user_id, e.release_id }).IsUnique();
            entity.HasOne(e => e.user)
                .WithMany()
                .HasForeignKey(e => e.user_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            entity.HasOne(e => e.release)
                .WithMany()
                .HasForeignKey(e => e.release_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity<UserReleaseDismiss>(entity =>
        {
            entity.HasKey(e => e.user_release_dismiss_id);
            entity.Property(e => e.dismissed_at);
            entity.HasIndex(e => new { e.user_id, e.release_id }).IsUnique();
            entity.HasOne(e => e.user)
                .WithMany()
                .HasForeignKey(e => e.user_id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            entity.HasOne(e => e.release)
                .WithMany()
                .HasForeignKey(e => e.release_id)
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
                if (entityEntry.State == EntityState.Modified || entityEntry.State == EntityState.Added)
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
