using Microsoft.EntityFrameworkCore;
using SchoolManager.Data.Entities;

namespace SchoolManager.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseDiscipline> CourseDisciplines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.Code)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentNumber)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.EmployeeNumber)
                .IsUnique();

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.Email)
                .IsUnique();

            modelBuilder.Entity<Discipline>()
                .HasIndex(d => d.Code)
                .IsUnique();

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.DisciplineId })
                .IsUnique();

            modelBuilder.Entity<CourseDiscipline>()
                .HasIndex(cd => new { cd.CourseId, cd.DisciplineId })
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Discipline>()
                .HasOne(d => d.Teacher)
                .WithMany(t => t.Disciplines)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Discipline)
                .WithMany(d => d.Enrollments)
                .HasForeignKey(e => e.DisciplineId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseDiscipline>()
                .HasOne(cd => cd.Course)
                .WithMany(c => c.CourseDisciplines)
                .HasForeignKey(cd => cd.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseDiscipline>()
                .HasOne(cd => cd.Discipline)
                .WithMany(d => d.CourseDisciplines)
                .HasForeignKey(cd => cd.DisciplineId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.FinalGrade)
                .HasPrecision(4, 2);
        }
    }
}