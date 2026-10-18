using UniversityApi.Models;

namespace UniversityApi.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync(string? nameFilter = null);
        Task<Course?> GetByIdAsync(int id);
        Task AddAsync(Course course);
        void Update(Course course);
        void Delete(Course course);
        Task<bool> SaveChangesAsync();
    }
}
