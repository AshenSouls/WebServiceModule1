namespace UniversityClient.Models;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public T? Result { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? TraceId { get; set; }
}

public class ApiCallResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public static ApiCallResult<T> Ok(T data) => new() { Success = true, Data = data };
    public static ApiCallResult<T> Fail(string message) => new() { Success = false, ErrorMessage = message };
}

public class CourseItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;

    // Поля v2
    public string? DifficultyLevel { get; set; }
    public string? Prerequisites { get; set; }
}   