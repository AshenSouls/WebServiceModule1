using System.ComponentModel.DataAnnotations;

namespace UniversityApi.DTO.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        public string Password { get; set; } = string.Empty;
    }
}