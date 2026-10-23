namespace UniversityApi.DTO.Courses
{
    public class CreateCourseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int TeacherId { get; set; }
    }
}
