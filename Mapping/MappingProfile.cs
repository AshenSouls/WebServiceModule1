using AutoMapper;
using UniversityApi.DTO.Courses;
using UniversityApi.DTO.Enrollments;
using UniversityApi.DTO.Students;
using UniversityApi.DTO.Teachers;
using UniversityApi.Models;

namespace UniversityApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Student mappings
            CreateMap<Student, StudentDto>();
            CreateMap<CreateStudentDto, Student>();
            CreateMap<UpdateStudentDto, Student>();

            // Teacher mappings
            CreateMap<Teacher, TeacherDto>();
            CreateMap<CreateTeacherDto, Teacher>();
            CreateMap<UpdateTeacherDto, Teacher>();

            // Course mappings
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.TeacherName,
                    opt => opt.MapFrom(src => $"{src.Teacher.FirstName} {src.Teacher.LastName}"));
            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>();

            // Enrollment mappings
            CreateMap<Enrollment, EnrollmentDto>()
                .ForMember(dest => dest.StudentName,
                    opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
                .ForMember(dest => dest.CourseName,
                    opt => opt.MapFrom(src => src.Course.Name));
            CreateMap<CreateEnrollmentDto, Enrollment>();
        }
    }
}
