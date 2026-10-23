using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversityApi.Common;
using UniversityApi.DTO.Students;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;

namespace UniversityApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Требуем авторизацию для всех эндпоинтов
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentRepository repository, IMapper mapper, ILogger<StudentsController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")] // Просматривать всех студентов могут только Admin и Teacher
        public async Task<IActionResult> GetAll()
        {
            var students = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<StudentDto>>(students);
            return Ok(ReturnResult<IEnumerable<StudentDto>>.Success(dtos));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> GetById(int id)
        {
            // Студент может просматривать только свою карточку
            if (User.IsInRole("Student") && !User.IsInRole("Admin") && !User.IsInRole("Teacher"))
            {
                var studentIdClaim = User.FindFirst("StudentId")?.Value;
                if (string.IsNullOrEmpty(studentIdClaim) || studentIdClaim != id.ToString())
                {
                    return StatusCode(403, ReturnResult<StudentDto>.Failure(403, "FORBIDDEN", "Вы не можете просматривать данные других студентов"));
                }
            }

            var student = await _repository.GetByIdAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Попытка получить несуществующего студента с ID {Id}", id);
                return NotFound(ReturnResult<StudentDto>.Failure(404, "STUDENT_NOT_FOUND", $"Студент с ID {id} не найден"));
            }

            var dto = _mapper.Map<StudentDto>(student);
            return Ok(ReturnResult<StudentDto>.Success(dto));
        }

        [HttpGet("{id}/courses")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> GetStudentWithCourses(int id)
        {
            // Сценарий №9: Студент запрашивает чужие оценки -> 403 Forbidden
            if (User.IsInRole("Student") && !User.IsInRole("Admin") && !User.IsInRole("Teacher"))
            {
                var studentIdClaim = User.FindFirst("StudentId")?.Value;
                if (string.IsNullOrEmpty(studentIdClaim) || studentIdClaim != id.ToString())
                {
                    _logger.LogWarning("Студент попытался запросить оценки чужого студента с ID {Id}", id);
                    return StatusCode(403, ReturnResult<object>.Failure(403, "FORBIDDEN", "Вы не можете просматривать оценки и курсы других студентов"));
                }
            }

            var student = await _repository.GetByIdWithCoursesAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Попытка получить несуществующего студента с ID {Id}", id);
                return NotFound(ReturnResult<object>.Failure(404, "STUDENT_NOT_FOUND", $"Студент с ID {id} не найден"));
            }

            return Ok(ReturnResult<object>.Success(new
            {
                student.Id,
                student.FirstName,
                student.LastName,
                Courses = student.Enrollments.Select(e => new
                {
                    e.Course.Id,
                    e.Course.Name,
                    e.Grade
                })
            }));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Создавать студентов может только Admin
        public async Task<IActionResult> Create([FromBody] CreateStudentDto dto)
        {
            var student = _mapper.Map<Student>(dto);
            await _repository.AddAsync(student);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Создан студент {FirstName} {LastName} (ID: {Id})", student.FirstName, student.LastName, student.Id);

            var resultDto = _mapper.Map<StudentDto>(student);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, ReturnResult<StudentDto>.Success(resultDto, 201));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Обновлять данные может только Admin
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto dto)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Попытка обновить несуществующего студента с ID {Id}", id);
                return NotFound(ReturnResult<StudentDto>.Failure(404, "STUDENT_NOT_FOUND", $"Студент с ID {id} не найден"));
            }

            _mapper.Map(dto, student);
            _repository.Update(student);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Обновлен студент с ID {Id}", id);
            return Ok(ReturnResult<StudentDto>.Success(_mapper.Map<StudentDto>(student)));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Удалять студентов может только Admin
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Попытка удалить несуществующего студента с ID {Id}", id);
                return NotFound(ReturnResult<bool>.Failure(404, "STUDENT_NOT_FOUND", $"Студент с ID {id} не найден"));
            }

            _repository.Delete(student);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Удален студент с ID {Id}", id);
            return Ok(ReturnResult<bool>.Success(true));
        }
    }
}