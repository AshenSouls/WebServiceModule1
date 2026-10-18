using FluentValidation;

namespace UniversityApi.DTO.Enrollments
{
    public class CreateEnrollmentDtoValidator : AbstractValidator<CreateEnrollmentDto>
    {
        public CreateEnrollmentDtoValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Укажите корректный ID студента");

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("Укажите корректный ID курса");
        }
    }
}