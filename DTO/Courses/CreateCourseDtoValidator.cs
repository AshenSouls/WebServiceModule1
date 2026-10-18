using FluentValidation;

namespace UniversityApi.DTO.Courses
{
    public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название курса обязательно для заполнения")
                .MaximumLength(150).WithMessage("Название курса не должно превышать 150 символов");

            RuleFor(x => x.Credits)
                .InclusiveBetween(1, 10).WithMessage("Количество кредитов должно быть от 1 до 10");

            RuleFor(x => x.TeacherId)
                .GreaterThan(0).WithMessage("Укажите корректный ID преподавателя");
        }
    }
}