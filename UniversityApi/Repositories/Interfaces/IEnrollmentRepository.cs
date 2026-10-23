using UniversityApi.Models;

namespace UniversityApi.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(int id);
        Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId);
        Task AddAsync(Enrollment enrollment);
        void Delete(Enrollment enrollment);
        Task<bool> SaveChangesAsync();
    }
}
