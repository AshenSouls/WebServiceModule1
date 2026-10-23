using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;

namespace UniversityApi.Repositories.Implementations
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllAsync(string? nameFilter = null)
        {
            var query = _context.Courses
                .Include(c => c.Teacher)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                query = query.Where(c => c.Name.Contains(nameFilter));
            }

            return await query.ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int id) =>
            await _context.Courses
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Course>> GetAllWithDetailsAsync(string? nameFilter = null)
        {
            var query = _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                query = query.Where(c => c.Name.Contains(nameFilter));
            }

            return await query.ToListAsync();
        }

        public async Task<Course?> GetByIdWithDetailsAsync(int id) =>
            await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task AddAsync(Course course) =>
            await _context.Courses.AddAsync(course);

        public void Update(Course course) =>
            _context.Courses.Update(course);

        public void Delete(Course course) =>
            _context.Courses.Remove(course);

        public async Task<bool> SaveChangesAsync() =>
            await _context.SaveChangesAsync() > 0;
    }
}
