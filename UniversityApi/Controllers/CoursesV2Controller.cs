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
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/courses")]
    [Authorize]
    public class CoursesV2Controller : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CoursesV2Controller> _logger;

        public CoursesV2Controller(
            ICourseRepository courseRepository,
            IMapper mapper,
            ILogger<CoursesV2Controller> logger)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? nameFilter)
        {
            var courses = await _courseRepository.GetAllWithDetailsAsync(nameFilter);
            var dtos = _mapper.Map<IEnumerable<CourseV2Dto>>(courses);
            return Ok(ReturnResult<IEnumerable<CourseV2Dto>>.Success(dtos));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _courseRepository.GetByIdWithDetailsAsync(id);
            if (course == null)
            {
                _logger.LogWarning("Попытка получить несуществующий курс v2 с ID {Id}", id);
                return NotFound(ReturnResult<CourseV2Dto>.Failure(404, "COURSE_NOT_FOUND", $"Курс с ID {id} не найден"));
            }

            return Ok(ReturnResult<CourseV2Dto>.Success(_mapper.Map<CourseV2Dto>(course)));
        }

    }
}