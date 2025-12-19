using Education.Domain.Entities;
using Education.Infrastructure.Persistence.Data;
using Education.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Education.Infrastructure.Persistence.Repositories.Impl
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UniversityDbContext _database;

        private readonly IGenericRepository<MyUser> _myUsers;
        private readonly IGenericRepository<Student> _students;
        private readonly IGenericRepository<Teacher> _teachers;
        private readonly IGenericRepository<Course> _courses;
        private readonly IGenericRepository<StudentCourse> _studentCourses;
        private readonly IGenericRepository<MyRole> _myRoles;
        private readonly IGenericRepository<MyUserRole> _myUserRoles;
        private readonly IGenericRepository<Token> _tokens;
        private readonly IGenericRepository<StudentDiscipline> _studentDisciplines;
        public UnitOfWork(UniversityDbContext database)
        {
            _database = database;
            _myUsers = new GenericRepository<MyUser>(_database);
            _students = new GenericRepository<Student>(_database);
            _teachers = new GenericRepository<Teacher>(_database);
            _courses = new GenericRepository<Course>(_database);
            _studentCourses = new GenericRepository<StudentCourse>(_database);
            _myRoles = new GenericRepository<MyRole>(_database);
            _myUserRoles = new GenericRepository<MyUserRole>(_database);
            _tokens = new GenericRepository<Token>(_database);
            _studentDisciplines = new GenericRepository<StudentDiscipline>(_database);
        }
        public IGenericRepository<MyUser> MyUsers => _myUsers;
        public IGenericRepository<Student> Students => _students;
        public IGenericRepository<Teacher> Teachers => _teachers;
        public IGenericRepository<Course> Courses => _courses;
        public IGenericRepository<StudentCourse> StudentCourses => _studentCourses;
        public IGenericRepository<MyRole> MyRoles => _myRoles;
        public IGenericRepository<MyUserRole> MyUserRoles => _myUserRoles;
        public IGenericRepository<Token> Tokens => _tokens;
        public IGenericRepository<StudentDiscipline> StudentDisciplines => _studentDisciplines;

        public async Task<int> SaveChangesAsync()
        {
            return await _database.SaveChangesAsync();
        }

        public void Dispose()
        {
            _database.Dispose();
             GC.SuppressFinalize(this);
        }
    }
}
