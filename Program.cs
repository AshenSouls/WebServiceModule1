using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.Middleware;
using UniversityApi.Repositories.Implementations;
using UniversityApi.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Регистрация DbContext с подключением к SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Регистрация AutoMapper
builder.Services.AddAutoMapper(typeof(UniversityApi.Mapping.MappingProfile));

// 3. Регистрация FluentValidation (автоматически видит валидаторы во всех папках)
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 4. Регистрация репозиториев (Dependency Injection)
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

// 5. Регистрация контроллеров
builder.Services.AddControllers();

// 6. Настройка Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Глобальная обработка ошибок через кастомную Middleware (Пункт 18 задания)
app.UseMiddleware<ExceptionMiddleware>();

// Настройка HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();