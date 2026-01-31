using Microsoft.EntityFrameworkCore;
using Skwela.Domain.Entities;
using Skwela.Domain.Enums;

namespace Skwela.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Classroom> Classrooms => Set<Classroom>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    //public DbSet<Assignment> Assignments => Set<Assignment>();
    //public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.user_id);
            entity.Property(e => e.username);
            entity.Property(e => e.email);
            entity.Property(e => e.password).IsRequired();
            entity.Property(e => e.display_name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.display_image);
            entity.Property(e => e.role).IsRequired();
            entity.Property(e => e.refreshToken);
            entity.Property(e => e.refreshTokenExpiryTime);
            entity.Property(e => e.user_created_at).IsRequired();
            entity.HasIndex(e => e.role);
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.class_id);
            entity.Property(e => e.class_name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.class_description).HasMaxLength(1000);
            entity.HasOne(e => e.user)
                  .WithMany(u => u.classrooms)
                  .HasForeignKey(e => e.user_id)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.class_created_at).IsRequired();
            entity.HasIndex(e => e.user_id);
            entity.HasIndex(e => e.class_id);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.class_id);
            entity.HasKey(e => e.user_id);
            entity.Property(e => e.enrolled_at).IsRequired();
            entity.Property(e => e.enrolled_status).IsRequired().HasMaxLength(50);
        });


    }

}