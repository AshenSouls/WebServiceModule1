using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;

namespace UniversityApi.Repositories.Implementations
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync() =>
            await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();

        public async Task<Enrollment?> GetByIdAsync(int id) =>
            await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId) =>
            await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        public async Task AddAsync(Enrollment enrollment) =>
            await _context.Enrollments.AddAsync(enrollment);

        public void Delete(Enrollment enrollment) =>
            _context.Enrollments.Remove(enrollment);

        public async Task<bool> SaveChangesAsync() =>
            await _context.SaveChangesAsync() > 0;
    }
}
