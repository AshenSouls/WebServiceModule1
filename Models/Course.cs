namespace UniversityApi.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Внешний ключ на Преподавателя
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        // Связь "Один ко многим": на курс зачислено много студентов
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}