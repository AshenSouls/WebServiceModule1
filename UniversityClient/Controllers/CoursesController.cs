using Microsoft.AspNetCore.Mvc;
using UniversityClient.Models;
using UniversityClient.Services;

namespace UniversityClient.Controllers;

public class CoursesController : Controller
{
    private readonly CoursesApiClient _apiClient;

    public CoursesController(CoursesApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index(string version = "v1")
    {
        ViewBag.Version = version;
        ViewData["Version"] = version; // Добавляем явное сохранение

        var result = version == "v2" 
            ? await _apiClient.GetCoursesV2Async() 
            : await _apiClient.GetCoursesV1Async();

        if (!result.Success)
        {
            ViewBag.Error = result.ErrorMessage;
            return View(new List<CourseItem>());
        }

        return View(result.Data);
    }

    public async Task<IActionResult> Details(int id, string version = "v1")
    {
        ViewBag.Version = version;
        var result = await _apiClient.GetCourseByIdAsync(id, version);

        if (!result.Success)
        {
            ViewBag.Error = result.ErrorMessage;
            return View(null as CourseItem);
        }

        return View(result.Data);
    }
}