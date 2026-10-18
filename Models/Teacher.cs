namespace UniversityApi.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        // Связь "Один ко многим": преподаватель ведет несколько курсов
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}