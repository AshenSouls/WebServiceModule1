namespace UniversityApi.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Связь "Один ко многим": у студента может быть много записей на курсы
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}