using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversityApi.Common;
using UniversityApi.Data;
using UniversityApi.DTO.Auth;
using UniversityApi.Models;

namespace UniversityApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            ApplicationDbContext dbContext,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest(ReturnResult<object>.Failure(400, "USER_EXISTS", "Пользователь с таким именем уже существует"));
            }

            if (dto.Role is not ("Student" or "Teacher"))
            {
                return BadRequest(ReturnResult<object>.Failure(400, "INVALID_ROLE", "При регистрации доступны только роли Student и Teacher"));
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(ReturnResult<object>.Failure(400, "REGISTRATION_FAILED", errors));
            }

            if (dto.Role == "Student")
            {
                var student = new Student
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email
                };
                _dbContext.Students.Add(student);
                await _dbContext.SaveChangesAsync();
                user.StudentId = student.Id;
            }
            else
            {
                var teacher = new Teacher
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Department = string.Empty
                };
                _dbContext.Teachers.Add(teacher);
                await _dbContext.SaveChangesAsync();
                user.TeacherId = teacher.Id;
            }

            await _userManager.AddToRoleAsync(user, dto.Role);
            await _userManager.UpdateAsync(user);
            _logger.LogInformation("Зарегистрирован новый пользователь {Username}", user.UserName);

            return Ok(ReturnResult<string>.Success("Регистрация успешно завершена"));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
            {
                return Unauthorized(ReturnResult<object>.Failure(401, "INVALID_CREDENTIALS", "Неверное имя пользователя или пароль"));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (result.IsLockedOut)
            {
                return StatusCode(423, ReturnResult<object>.Failure(423, "ACCOUNT_LOCKED", "Учетная запись заблокирована из-за частых неудачных попыток"));
            }

            if (!result.Succeeded)
            {
                return Unauthorized(ReturnResult<object>.Failure(401, "INVALID_CREDENTIALS", "Неверное имя пользователя или пароль"));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _tokenService.GenerateTokenAsync(user, roles);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = user.UserName!,
                Email = user.Email!,
                Roles = roles.ToList(),
                Expiration = _tokenService.GetExpiration()
            };

            _logger.LogInformation("Пользователь {Username} успешно вошел в систему", user.UserName);
            return Ok(ReturnResult<AuthResponseDto>.Success(response));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ReturnResult<object>.Failure(401, "UNAUTHORIZED", "Пользователь не авторизован"));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ReturnResult<object>.Failure(404, "USER_NOT_FOUND", "Пользователь не найден"));
            }

            var roles = await _userManager.GetRolesAsync(user);

            var response = new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.StudentId,
                user.TeacherId,
                Roles = roles
            };

            return Ok(ReturnResult<object>.Success(response));
        }
    }
}