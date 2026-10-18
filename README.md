# University Course Management API

Учебный REST Web API на ASP.NET Core для управления университетскими курсами, студентами, преподавателями и записями на курсы.

## Требования

- .NET SDK 9.0 или выше
- Microsoft SQL Server
- Инструмент Entity Framework Core CLI

При необходимости установите инструмент Entity Framework Core CLI:

```bash
dotnet tool install --global dotnet-ef
```

## Локальный запуск

### 1. Клонирование репозитория

```bash
git clone https://github.com/AshenSouls/WebServiceModule1
cd addway_module_1\UniversityApi
```

### 2. Настройка подключения к базе данных

Откройте файл `UniversityApi/appsettings.json` и укажите параметры подключения к Microsoft SQL Server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=UniversityDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

При необходимости замените значение `Server` на адрес и порт вашего SQL Server.

### 3. Применение миграций

Выполните команду из каталога `UniversityApi`:

```bash
dotnet ef database update
```

Команда создаст базу данных `UniversityDb` и необходимые таблицы на указанном SQL Server.

### 4. Запуск приложения

```bash
dotnet run
```

### 5. Открытие Swagger UI

После запуска откройте в браузере:

```text
http://localhost:5180/swagger
```

Также приложение поддерживает HTTPS по адресу `https://localhost:7249/swagger`.

## Структура проекта

- `Controllers/` — REST-контроллеры для работы со студентами, преподавателями, курсами и записями на курсы.
- `Models/` — доменные сущности Entity Framework Core: `Student`, `Teacher`, `Course` и `Enrollment`.
- `DTO/` — объекты передачи данных и валидаторы FluentValidation, сгруппированные по сущностям.
- `Data/` — контекст базы данных `ApplicationDbContext`, конфигурация связей и индексов.
- `Repositories/` — реализация паттерна Repository:
  - `Interfaces/` — интерфейсы репозиториев.
  - `Implementations/` — реализации репозиториев на основе Entity Framework Core.
- `Mapping/` — профиль AutoMapper для преобразования между сущностями и DTO.
- `Middleware/` — глобальная обработка исключений в `ExceptionMiddleware`.
- `Common/` — общая обертка ответов API `ReturnResult<T>`.
- `Migrations/` — миграции Entity Framework Core.

## Используемые технологии

- .NET 9.0
- ASP.NET Core Web API
- Microsoft SQL Server
- Entity Framework Core
- Repository Pattern
- Dependency Injection
- AutoMapper
- FluentValidation
- Swashbuckle (Swagger / OpenAPI)
- `ILogger`

## Основные API-ресурсы

- `Students` — управление студентами
- `Teachers` — управление преподавателями
- `Courses` — управление курсами
- `Enrollments` — управление записями студентов на курсы