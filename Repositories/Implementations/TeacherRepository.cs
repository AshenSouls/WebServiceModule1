using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;

namespace UniversityApi.Repositories.Implementations
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDbContext _context;

        public TeacherRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync() =>
            await _context.Teachers.ToListAsync();

        public async Task<Teacher?> GetByIdAsync(int id) =>
            await _context.Teachers.FindAsync(id);

        public async Task AddAsync(Teacher teacher) =>
            await _context.Teachers.AddAsync(teacher);

        public void Update(Teacher teacher) =>
            _context.Teachers.Update(teacher);

        public void Delete(Teacher teacher) =>
            _context.Teachers.Remove(teacher);

        public async Task<bool> SaveChangesAsync() =>
            await _context.SaveChangesAsync() > 0;
    }
}
