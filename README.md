# University Course Management System

Учебное решение на платформах **.NET 9**, состоящее из двух независимых приложений: REST Web API (**`UniversityApi`**) и клиентского MVC-приложения (**`UniversityClient`**). 

Проект разработан для управления университетскими курсами, студентами, преподавателями и записями на курсы с поддержкой авторизации, разграничения прав доступа на основе ролей и проверки владения ресурсом.

---

##  Архитектура решения

Решение объединяет два компонента:

1. **`UniversityApi`** — Backend-сервис на ASP.NET Core Web API:
   - Аутентификация и авторизация на основе **ASP.NET Core Identity** и **JWT Bearer Token**.
   - Ролевая модель: `Admin`, `Teacher`, `Student`.
   - Проверка прав доступа и владения ресурсом на уровне контроллеров.
   - Версионирование API (`v1` и `v2`) с разделением DTO.
   - Поддержка CORS, интеграция Swagger UI с поддержкой авторизации по токену.
   - Автоматическая обработка ошибок через `ExceptionMiddleware` с единым форматом ответа `ReturnResult<T>`.
   - Хранение секретных ключей подписи JWT вне исходного кода с помощью **User Secrets**.

2. **`UniversityClient`** — Frontend-клиент на ASP.NET Core MVC:
   - Взаимодействует с Web API через HTTP с использованием `IHttpClientFactory` и типизированного клиента `CoursesApiClient`.
   - Не имеет прямого подключения к базе данных.
   - Отображает каталог курсов, детализацию и обрабатывает ошибки недоступности API.

---

##  Технологический стек

- **Платформа:** .NET 9.0 (ASP.NET Core Web API / ASP.NET Core MVC)
- **База данных:** Microsoft SQL Server (LocalDB / Express), Entity Framework Core 9 (Code First)
- **Безопасность:** ASP.NET Core Identity, JWT Bearer Token, User Secrets, CORS
- **Паттерны и архитектура:** Repository Pattern, Dependency Injection, DTO
- **Библиотеки и инструменты:**
  - AutoMapper — маппинг сущностей и DTO
  - FluentValidation — валидация входных данных
  - Asp.Versioning — версионирование API
  - Swashbuckle (Swagger/OpenAPI) — документация API
  - `ILogger` — логирование операций

---

##  Требования к окружению

- **.NET SDK 9.0** или выше
- **Microsoft SQL Server** (LocalDB или обычный сервер)
- Глобальный инструмент **Entity Framework Core CLI**:
  ```bash
  dotnet tool install --global dotnet-ef