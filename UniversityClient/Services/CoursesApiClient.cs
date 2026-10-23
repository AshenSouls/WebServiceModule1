using System.Net;
using System.Text.Json;
using UniversityClient.Models;

namespace UniversityClient.Services;

public class CoursesApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<CoursesApiClient> _logger;

    public CoursesApiClient(HttpClient http, ILogger<CoursesApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public Task<ApiCallResult<List<CourseItem>>> GetCoursesV1Async() =>
        SendAsync<List<CourseItem>>("api/v1/courses");

    public Task<ApiCallResult<List<CourseItem>>> GetCoursesV2Async() =>
        SendAsync<List<CourseItem>>("api/v2/courses");

    public Task<ApiCallResult<CourseItem>> GetCourseByIdAsync(int id, string version = "v1") =>
        SendAsync<CourseItem>($"api/{version}/courses/{id}");

    private async Task<ApiCallResult<T>> SendAsync<T>(string path) where T : class
    {
        try
        {
            using var response = await _http.GetAsync(path);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(options);

            if (response.IsSuccessStatusCode && body?.Result is not null)
            {
                return ApiCallResult<T>.Ok(body.Result);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ApiCallResult<T>.Fail("Запрошенные данные не найдены (404)");
            }

            var errorMsg = body?.ErrorMessage ?? response.ReasonPhrase ?? "Ошибка сервера";
            return ApiCallResult<T>.Fail($"API вернул код {(int)response.StatusCode}: {errorMsg}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "API недоступен по адресу {BaseAddress}", _http.BaseAddress);
            return ApiCallResult<T>.Fail("API недоступен. Убедитесь, что бэкенд UniversityApi запущен.");
        }
        catch (TaskCanceledException)
        {
            return ApiCallResult<T>.Fail("Превышено время ожидания ответа от API (Timeout).");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Ошибка парсинга JSON");
            return ApiCallResult<T>.Fail("API вернул ответ в некорректном формате.");
        }
    }
}