using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UniversityApi.Common;
using UniversityApi.DTO.Enrollments;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;

namespace UniversityApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EnrollmentsController> _logger;

        public EnrollmentsController(
            IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            IMapper mapper,
            ILogger<EnrollmentsController> logger)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var enrollments = await _enrollmentRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
            return Ok(ReturnResult<IEnumerable<EnrollmentDto>>.Success(dtos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null)
            {
                _logger.LogWarning("Попытка получить несуществующую запись с ID {Id}", id);
                return NotFound(ReturnResult<EnrollmentDto>.Failure(404, "ENROLLMENT_NOT_FOUND", $"Запись с ID {id} не найдена"));
            }

            return Ok(ReturnResult<EnrollmentDto>.Success(_mapper.Map<EnrollmentDto>(enrollment)));
        }

        [HttpPost]
        public async Task<IActionResult> EnrollStudent([FromBody] CreateEnrollmentDto dto)
        {
            // Правило №2: Нельзя записать несуществующего студента
            var student = await _studentRepository.GetByIdAsync(dto.StudentId);
            if (student == null)
            {
                _logger.LogWarning("Запись не удалась: студент {StudentId} не найден", dto.StudentId);
                return NotFound(ReturnResult<EnrollmentDto>.Failure(404, "STUDENT_NOT_FOUND", $"Студент с ID {dto.StudentId} не найден"));
            }

            // Правило №1: Нельзя записать студента на несуществующий курс
            var course = await _courseRepository.GetByIdAsync(dto.CourseId);
            if (course == null)
            {
                _logger.LogWarning("Запись не удалась: курс {CourseId} не найден", dto.CourseId);
                return NotFound(ReturnResult<EnrollmentDto>.Failure(404, "COURSE_NOT_FOUND", $"Курс с ID {dto.CourseId} не найден"));
            }

            // Правило №3: Один студент не может быть дважды записан на один и тот же курс
            var existingEnrollment = await _enrollmentRepository.GetByStudentAndCourseAsync(dto.StudentId, dto.CourseId);
            if (existingEnrollment != null)
            {
                _logger.LogWarning("Попытка повторной записи студента {StudentId} на курс {CourseId}", dto.StudentId, dto.CourseId);
                return Conflict(ReturnResult<EnrollmentDto>.Failure(409, "DUPLICATE_ENROLLMENT", "Студент уже записан на данный курс"));
            }

            var enrollment = _mapper.Map<Enrollment>(dto);
            await _enrollmentRepository.AddAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            _logger.LogInformation("Студент {StudentId} успешно записан на курс {CourseId}", dto.StudentId, dto.CourseId);

            var createdEnrollment = await _enrollmentRepository.GetByIdAsync(enrollment.Id);
            return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, ReturnResult<EnrollmentDto>.Success(_mapper.Map<EnrollmentDto>(createdEnrollment), 201));
        }

        [HttpPut("{id}/grade")]
        public async Task<IActionResult> UpdateGrade(int id, [FromBody] UpdateGradeDto dto)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null)
            {
                _logger.LogWarning("Попытка изменить оценку для несуществующей записи с ID {Id}", id);
                return NotFound(ReturnResult<EnrollmentDto>.Failure(404, "ENROLLMENT_NOT_FOUND", $"Запись с ID {id} не найдена"));
            }

            enrollment.Grade = dto.Grade;
            await _enrollmentRepository.SaveChangesAsync();

            _logger.LogInformation("Изменена оценка для записи {Id} на {Grade}", id, dto.Grade);
            return Ok(ReturnResult<EnrollmentDto>.Success(_mapper.Map<EnrollmentDto>(enrollment)));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Unenroll(int id)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null)
            {
                _logger.LogWarning("Попытка удалить несуществующую запись с ID {Id}", id);
                return NotFound(ReturnResult<bool>.Failure(404, "ENROLLMENT_NOT_FOUND", $"Запись с ID {id} не найдена"));
            }

            _enrollmentRepository.Delete(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            _logger.LogInformation("Удалена запись с ID {Id}", id);
            return Ok(ReturnResult<bool>.Success(true));
        }
    }
}