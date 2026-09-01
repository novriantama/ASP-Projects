using Microsoft.EntityFrameworkCore;
using ASPProjects.Models.Entities;

namespace ASPProjects.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Entity Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.Property(e => e.Username)
                  .HasColumnName("username")
                  .HasMaxLength(50)
                  .IsRequired();

            entity.HasIndex(e => e.Username)
                  .IsUnique();

            entity.Property(e => e.PasswordHash)
                  .HasColumnName("password_hash")
                  .HasMaxLength(255)
                  .IsRequired();

            entity.Property(e => e.Role)
                  .HasColumnName("role")
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Project Entity Configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("projects", t =>
            {
                t.HasCheckConstraint("chk_status", "[status] IN ('On Progress', 'Completed', 'Overdue')");
                t.HasCheckConstraint("chk_progress", "[progress_percentage] >= 0.00 AND [progress_percentage] <= 100.00");
            });

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.ProjectName)
                  .HasColumnName("project_name")
                  .HasMaxLength(255)
                  .IsRequired();

            entity.Property(e => e.Description)
                  .HasColumnName("description")
                  .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Status)
                  .HasColumnName("status")
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(e => e.StartDate)
                  .HasColumnName("start_date")
                  .HasColumnType("date");

            entity.Property(e => e.EndDate)
                  .HasColumnName("end_date")
                  .HasColumnType("date");

            entity.Property(e => e.ProgressPercentage)
                  .HasColumnName("progress_percentage")
                  .HasColumnType("decimal(5,2)")
                  .HasDefaultValue(0.00m);

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
