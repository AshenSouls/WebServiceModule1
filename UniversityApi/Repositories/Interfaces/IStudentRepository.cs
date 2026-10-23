using UniversityApi.Models;

namespace UniversityApi.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByIdWithCoursesAsync(int id);
        Task AddAsync(Student student);
        void Update(Student student);
        void Delete(Student student);
        Task<bool> SaveChangesAsync();
    }
}
