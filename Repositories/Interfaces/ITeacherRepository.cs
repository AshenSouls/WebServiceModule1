using UniversityApi.Models;

namespace UniversityApi.Repositories.Interfaces
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<Teacher>> GetAllAsync();
        Task<Teacher?> GetByIdAsync(int id);
        Task AddAsync(Teacher teacher);
        void Update(Teacher teacher);
        void Delete(Teacher teacher);
        Task<bool> SaveChangesAsync();
    }
}
