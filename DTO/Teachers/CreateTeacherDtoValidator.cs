using FluentValidation;

namespace UniversityApi.DTO.Teachers
{
    public class CreateTeacherDtoValidator : AbstractValidator<CreateTeacherDto>
    {
        public CreateTeacherDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Имя обязательно для заполнения")
                .MaximumLength(50).WithMessage("Имя не должно превышать 50 символов");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Фамилия обязательна для заполнения")
                .MaximumLength(50).WithMessage("Фамилия не должна превышать 50 символов");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен для заполнения")
                .EmailAddress().WithMessage("Некорректный формат Email")
                .MaximumLength(100).WithMessage("Email не должен превышать 100 символов");

            RuleFor(x => x.Department)
                .NotEmpty().WithMessage("Кафедра обязательна для заполнения")
                .MaximumLength(100).WithMessage("Название кафедры не должно превышать 100 символов");
        }
    }
}