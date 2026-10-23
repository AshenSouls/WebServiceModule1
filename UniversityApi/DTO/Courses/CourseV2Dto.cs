namespace UniversityApi.DTO.Courses
{
    public class CourseV2Dto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public TeacherDto? Teacher { get; set; }
        public List<EnrolledStudentDto> EnrolledStudents { get; set; } = new();
    }
}