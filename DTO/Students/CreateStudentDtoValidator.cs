using FluentValidation;

namespace UniversityApi.DTO.Students
{
    public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
    {
        public CreateStudentDtoValidator()
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
        }
    }
}