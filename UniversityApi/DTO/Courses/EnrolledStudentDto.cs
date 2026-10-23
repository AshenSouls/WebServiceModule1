namespace UniversityApi.DTO.Courses
{
    public class EnrolledStudentDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public double? Grade { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
