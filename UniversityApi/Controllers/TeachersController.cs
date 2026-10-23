using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityApi.Common;
using UniversityApi.DTO.Teachers;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;

namespace UniversityApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Teacher")] // Просматривать список и детальную информацию могут Admin и Teacher
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<TeachersController> _logger;

        public TeachersController(ITeacherRepository repository, IMapper mapper, ILogger<TeachersController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teachers = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<TeacherDto>>(teachers);
            return Ok(ReturnResult<IEnumerable<TeacherDto>>.Success(dtos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var teacher = await _repository.GetByIdAsync(id);
            if (teacher == null)
            {
                _logger.LogWarning("Попытка получить несуществующего преподавателя с ID {Id}", id);
                return NotFound(ReturnResult<TeacherDto>.Failure(404, "TEACHER_NOT_FOUND", $"Преподаватель с ID {id} не найден"));
            }

            return Ok(ReturnResult<TeacherDto>.Success(_mapper.Map<TeacherDto>(teacher)));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Создавать преподавателей может только Admin
        public async Task<IActionResult> Create([FromBody] CreateTeacherDto dto)
        {
            var teacher = _mapper.Map<Teacher>(dto);
            await _repository.AddAsync(teacher);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Создан преподаватель {FirstName} {LastName} (ID: {Id})", teacher.FirstName, teacher.LastName, teacher.Id);

            var resultDto = _mapper.Map<TeacherDto>(teacher);
            return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, ReturnResult<TeacherDto>.Success(resultDto, 201));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Обновлять данные преподавателя может только Admin
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTeacherDto dto)
        {
            var teacher = await _repository.GetByIdAsync(id);
            if (teacher == null)
            {
                _logger.LogWarning("Попытка обновить несуществующего преподавателя с ID {Id}", id);
                return NotFound(ReturnResult<TeacherDto>.Failure(404, "TEACHER_NOT_FOUND", $"Преподаватель с ID {id} не найден"));
            }

            _mapper.Map(dto, teacher);
            _repository.Update(teacher);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Обновлен преподаватель с ID {Id}", id);
            return Ok(ReturnResult<TeacherDto>.Success(_mapper.Map<TeacherDto>(teacher)));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Удалять преподавателей может только Admin
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _repository.GetByIdAsync(id);
            if (teacher == null)
            {
                _logger.LogWarning("Попытка удалить несуществующего преподавателя с ID {Id}", id);
                return NotFound(ReturnResult<bool>.Failure(404, "TEACHER_NOT_FOUND", $"Преподаватель с ID {id} не найден"));
            }

            _repository.Delete(teacher);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Удален преподаватель с ID {Id}", id);
            return Ok(ReturnResult<bool>.Success(true));
        }
    }
}