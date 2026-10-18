using Microsoft.EntityFrameworkCore;
using UniversityApi.Models;

namespace UniversityApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка Student
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(s => s.LastName).IsRequired().HasMaxLength(50);
                entity.Property(s => s.Email).IsRequired().HasMaxLength(100);
            });

            // Настройка Teacher
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(t => t.LastName).IsRequired().HasMaxLength(50);
                entity.Property(t => t.Email).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Department).IsRequired().HasMaxLength(100);
            });

            // Настройка Course
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(150);
                entity.Property(c => c.Description).HasMaxLength(500);

                // Связь Course -> Teacher (1 к N)
                entity.HasOne(c => c.Teacher)
                      .WithMany(t => t.Courses)
                      .HasForeignKey(c => c.TeacherId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Настройка Enrollment
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Составной уникальный индекс (Правило №3: уникальная пара StudentId + CourseId)
                entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Enrollments)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Enrollments)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}