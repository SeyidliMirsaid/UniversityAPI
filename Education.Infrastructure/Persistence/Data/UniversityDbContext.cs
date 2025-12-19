using Education.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Education.Infrastructure.Persistence.Data
{
    public class UniversityDbContext(DbContextOptions models) : DbContext(models)
    {
        public DbSet<MyUser> MyUsers { get; set; }
        public DbSet<MyRole> MyRoles { get; set; }
        public DbSet<MyUserRole> MyUserRoles { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Token> StudentDisciplines { get; set; }
        public DbSet<Token> Tokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MyUser>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Student>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Teacher>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Course>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<StudentCourse>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<MyRole>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<MyUserRole>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Token>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<StudentDiscipline>().HasQueryFilter(e => !e.IsDeleted);


            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniversityDbContext).Assembly);

             
            /* Manual apply (əgər auto işləmirsə)
            modelBuilder.ApplyConfiguration(new MyUserConfiguration());
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new TeacherConfiguration());
            modelBuilder.ApplyConfiguration(new CourseConfiguration());
            modelBuilder.ApplyConfiguration(new StudentCourseConfiguration());
            modelBuilder.ApplyConfiguration(new MyRoleConfiguration());
            modelBuilder.ApplyConfiguration(new MyUserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new TokenConfiguration());
            modelBuilder.ApplyConfiguration(new StudentDisciplineConfiguration());
            */
        }

    }
}
