using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityApi.Common;
using UniversityApi.DTO.Courses;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;

namespace UniversityApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/courses")]
    [Route("api/courses")]
    [Authorize]
    public class CoursesV1Controller : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CoursesV1Controller> _logger;

        public CoursesV1Controller(
            ICourseRepository courseRepository,
            ITeacherRepository teacherRepository,
            IMapper mapper,
            ILogger<CoursesV1Controller> logger)
        {
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? nameFilter)
        {
            var courses = await _courseRepository.GetAllAsync(nameFilter);
            var dtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
            return Ok(ReturnResult<IEnumerable<CourseDto>>.Success(dtos));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
            {
                _logger.LogWarning("Попытка получить несуществующий курс с ID {Id}", id);
                return NotFound(ReturnResult<CourseDto>.Failure(404, "COURSE_NOT_FOUND", $"Курс с ID {id} не найден"));
            }

            return Ok(ReturnResult<CourseDto>.Success(_mapper.Map<CourseDto>(course)));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
        {
            var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
            if (teacher == null)
            {
                _logger.LogWarning("Попытка привязать курс к несуществующему преподавателю {TeacherId}", dto.TeacherId);
                return BadRequest(ReturnResult<CourseDto>.Failure(400, "TEACHER_NOT_FOUND", $"Преподаватель с ID {dto.TeacherId} не существует"));
            }

            var course = _mapper.Map<Course>(dto);
            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();

            _logger.LogInformation("Создан курс {Name} (ID: {Id})", course.Name, course.Id);

            var createdCourse = await _courseRepository.GetByIdAsync(course.Id);
            var resultDto = _mapper.Map<CourseDto>(createdCourse);
            return CreatedAtAction(nameof(GetById), new { id = course.Id, version = "1.0" }, ReturnResult<CourseDto>.Success(resultDto, 201));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseDto dto)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
            {
                _logger.LogWarning("Попытка обновить несуществующий курс с ID {Id}", id);
                return NotFound(ReturnResult<CourseDto>.Failure(404, "COURSE_NOT_FOUND", $"Курс с ID {id} не найден"));
            }

            var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
            if (teacher == null)
            {
                _logger.LogWarning("Попытка привязать курс {CourseId} к несуществующему преподавателю {TeacherId}", id, dto.TeacherId);
                return BadRequest(ReturnResult<CourseDto>.Failure(400, "TEACHER_NOT_FOUND", $"Преподаватель с ID {dto.TeacherId} не существует"));
            }

            _mapper.Map(dto, course);
            _courseRepository.Update(course);
            await _courseRepository.SaveChangesAsync();

            _logger.LogInformation("Обновлен курс с ID {Id}", id);
            return Ok(ReturnResult<CourseDto>.Success(_mapper.Map<CourseDto>(course)));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
            {
                _logger.LogWarning("Попытка удалить несуществующий курс с ID {Id}", id);
                return NotFound(ReturnResult<bool>.Failure(404, "COURSE_NOT_FOUND", $"Курс с ID {id} не найден"));
            }

            _courseRepository.Delete(course);
            await _courseRepository.SaveChangesAsync();

            _logger.LogInformation("Удален курс с ID {Id}", id);
            return Ok(ReturnResult<bool>.Success(true));
        }
    }
}