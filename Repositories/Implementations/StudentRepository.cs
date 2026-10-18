using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;

namespace UniversityApi.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync() =>
            await _context.Students.ToListAsync();

        public async Task<Student?> GetByIdAsync(int id) =>
            await _context.Students.FindAsync(id);

        public async Task<Student?> GetByIdWithCoursesAsync(int id) =>
            await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task AddAsync(Student student) =>
            await _context.Students.AddAsync(student);

        public void Update(Student student) =>
            _context.Students.Update(student);

        public void Delete(Student student) =>
            _context.Students.Remove(student);

        public async Task<bool> SaveChangesAsync() =>
            await _context.SaveChangesAsync() > 0;
    }
}
