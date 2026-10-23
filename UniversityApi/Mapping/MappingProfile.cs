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
            CreateMap<Teacher, UniversityApi.DTO.Teachers.TeacherDto>();
            CreateMap<Teacher, UniversityApi.DTO.Courses.TeacherDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));
            CreateMap<CreateTeacherDto, Teacher>();
            CreateMap<UpdateTeacherDto, Teacher>();

            // Course mappings (v1)
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.TeacherName,
                    opt => opt.MapFrom(src => src.Teacher != null 
                        ? $"{src.Teacher.FirstName} {src.Teacher.LastName}" 
                        : string.Empty));
            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>();

            // Course mappings (v2)
            CreateMap<Course, CourseV2Dto>()
                .ForMember(dest => dest.Teacher,
                    opt => opt.MapFrom(src => src.Teacher))
                .ForMember(dest => dest.EnrolledStudents,
                    opt => opt.MapFrom(src => src.Enrollments));
            CreateMap<Enrollment, EnrolledStudentDto>()
                .ForMember(dest => dest.StudentName,
                    opt => opt.MapFrom(src => src.Student != null
                        ? $"{src.Student.FirstName} {src.Student.LastName}"
                        : string.Empty))
                .ForMember(dest => dest.EnrolledAt,
                    opt => opt.MapFrom(src => src.EnrollmentDate));

            // Enrollment mappings
            CreateMap<Enrollment, EnrollmentDto>()
                .ForMember(dest => dest.StudentName,
                    opt => opt.MapFrom(src => src.Student != null 
                        ? $"{src.Student.FirstName} {src.Student.LastName}" 
                        : string.Empty))
                .ForMember(dest => dest.CourseName,
                    opt => opt.MapFrom(src => src.Course != null 
                        ? src.Course.Name 
                        : string.Empty));
            CreateMap<CreateEnrollmentDto, Enrollment>();
        }
    }
}