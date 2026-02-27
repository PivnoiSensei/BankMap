# Завдання 2: Рефакторинг Backend під вертикальну архітектуру

## Мета завдання

Переструктурувати існуючий backend код відповідно до принципів вертикальної архітектури. Розділити код на окремі збірки, впровадити MediatR та багатошарову архітектуру.

---

## Частина 1: Структура Solution

### Розділення на збірки

```
BankMap.sln
├── src/
│   ├── BankMap.WebApi/              # Controllers, конфігурація
│   ├── BankMap.Application/         # MediatR handlers, сервіси, валідатори
│   ├── BankMap.Domain/              # Доменні моделі, інтерфейси
│   └── BankMap.Infrastructure/      # EF Core, менеджери для БД
```

### Залежності між проектами

```
┌─────────────────────────────────────────────────────────────┐
│                       BankMap.WebApi                        │
│                      (ASP.NET Core)                         │
└─────────────────┬───────────────────────┬───────────────────┘
                  │                       │
                  │ references            │ references
                  ▼                       ▼
┌─────────────────────────┐     ┌─────────────────────────────┐
│  BankMap.Application    │     │   BankMap.Infrastructure    │
│  (Handlers, Services)   │     │   (EF Core, Managers)       │
└───────────┬─────────────┘     └──────────────┬──────────────┘
            │                                  │
            │ references                       │ references
            ▼                                  │
┌─────────────────────────┐                    │
│    BankMap.Domain       │◄───────────────────┘
│  (Entities, Interfaces) │
└─────────────────────────┘
```

**Правило:** Domain не залежить від нічого. Кожен шар залежить тільки від шарів нижче.

---

## Частина 2: Доменні моделі (BankMap.Domain)

### Структурні моделі замість DataJson

Поточна модель зберігає дані у JSON полі. Потрібно створити повноцінну структуру сутностей.

**Приклад напрямку (не повна структура):**

```csharp
public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; }
    public BranchType Type { get; set; }
    public Address Address { get; set; }
    public ICollection<WorkSchedule> Schedules { get; set; }
    // ... інші поля визначити самостійно на основі JSON структури
}

public class Address
{
    public int Id { get; set; }
    public string City { get; set; }
    public double Latitude { get; set; }
    // ...
}

public enum BranchType
{
    Department,
    Atm,
    Terminal
}
```

Проаналізуй існуючу JSON структуру та створи відповідні сутності для: адреси, графіку роботи, перерв, кас тощо.

### Аудитні поля

Кожна сутність повинна мати аудитні поля для відстеження змін. Реалізувати через базовий клас:

```csharp
public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

Автоматизувати наповнення цих полів при збереженні в БД. Для цього перевизначити метод `SaveChangesAsync` у `DbContext` та відстежувати стан сутностей через `ChangeTracker`.

Документація: [ChangeTracker in EF Core](https://learn.microsoft.com/en-us/ef/core/change-tracking/)

---

## Частина 3: Багатошарова архітектура

### Шари та їх відповідальність

```
┌────────────┐
│ Controller │  ──►  Приймає HTTP запит, викликає MediatR
└─────┬──────┘
      │
      ▼
┌────────────┐
│  Handler   │  ──►  Обробляє команду/запит, викликає сервіс, повертає Result<T>
└─────┬──────┘
      │
      ▼
┌────────────┐
│  Service   │  ──►  Бізнес-логіка, оркеструє виклики менеджерів
└─────┬──────┘
      │
      ▼
┌────────────┐
│  Manager   │  ──►  Робота з БД через DbContext
└─────┬──────┘
      │
      ▼
┌────────────┐
│ DbContext  │
└────────────┘
```

### Розміщення по збірках

- **WebApi**: Controllers
- **Application**: Handlers, Services, Interfaces
- **Infrastructure**: Managers, DbContext

### Result Pattern

Handlers повертають результат загорнутий у wrapper:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    
    public static Result<T> Success(T value) => // ...
    public static Result<T> Failure(string error) => // ...
}
```

---

## Частина 4: MediatR та CQRS

### Структура Features

```
Application/
├── Features/
│   └── Branches/
│       ├── Queries/
│       │   └── GetAllBranches/
│       │       ├── GetAllBranchesQuery.cs
│       │       └── GetAllBranchesHandler.cs
│       └── Commands/
│           └── UpdateBranchStatus/
│               ├── UpdateBranchStatusCommand.cs
│               ├── UpdateBranchStatusHandler.cs
│               └── UpdateBranchStatusValidator.cs
├── Services/
│   └── IBranchService.cs
└── Common/
    └── Result.cs
```

### Приклад Handler

```csharp
public class GetAllBranchesHandler : IRequestHandler<GetAllBranchesQuery, Result<List<BranchDto>>>
{
    private readonly IBranchService _branchService;

    public async Task<Result<List<BranchDto>>> Handle(
        GetAllBranchesQuery request, 
        CancellationToken cancellationToken)
    {
        List<BranchDto> branches = await _branchService.GetAllAsync(cancellationToken);
        return Result<List<BranchDto>>.Success(branches);
    }
}
```

---

## Частина 5: Базовий контролер

Створити `ApiControllerBase` з:
- Lazy injection для `IMediator` та `ILogger`
- Методом для відправки запитів з логуванням
- Використанням стандартного `Microsoft.Extensions.Logging`

```csharp
public abstract class ApiControllerBase : ControllerBase
{
    protected ISender Mediator => // lazy injection
    protected ILogger Logger => // lazy injection

    protected async Task<IActionResult> SendAsync<T>(IRequest<Result<T>> request, CancellationToken ct)
    {
        // Логування запиту
        // Виклик Mediator.Send
        // Обробка Result (Success/Failure)
        // Логування результату
    }
}
```

---

## Частина 6: Пакети та реєстрація

### NuGet пакети

**Application:**
- `MediatR` — реалізація патерну Mediator для CQRS
- `FluentValidation` — валідація запитів (альтернатива: `System.ComponentModel.DataAnnotations`)

**Infrastructure:**
- `Microsoft.EntityFrameworkCore.SqlServer` — провайдер для SQL Server

### Реєстрація сервісів

Кожна збірка має клас `DependencyInjection` з методом розширення:

```csharp
// Application/DependencyInjection.cs
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    // Реєстрація MediatR, валідаторів, сервісів
}

// Infrastructure/DependencyInjection.cs  
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
{
    // Реєстрація DbContext, менеджерів
}
```

Документація:
- [Dependency injection in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [MediatR Registration](https://github.com/jbogard/MediatR/wiki)

---

## Частина 7: Оновлення Docker

Оновити `Dockerfile` для роботи з multi-project структурою (копіювання всіх .csproj, restore, build).

---

## Критерії виконання

- [ ] Solution з 4 проектами (WebApi, Application, Domain, Infrastructure)
- [ ] Доменні моделі замість DataJson (Branch, Address, WorkSchedule, тощо)
- [ ] Аудитні поля з автоматичним наповненням
- [ ] Шари: Handler → Service → Manager
- [ ] Result<T> wrapper для відповідей handlers
- [ ] Базовий контролер з логуванням
- [ ] MediatR handlers для всіх операцій
- [ ] Валідатори для Commands
- [ ] Docker Compose працює
- [ ] Код на GitHub

---

## Матеріали для вивчення

### Архітектура
- [Vertical Slice Architecture](https://www.jimmybogard.com/vertical-slice-architecture/) — Jimmy Bogard
- [Clean Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

### MediatR
- [MediatR Wiki](https://github.com/jbogard/MediatR/wiki)
- [CQRS Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)

### Entity Framework
- [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [ChangeTracker in EF Core](https://learn.microsoft.com/en-us/ef/core/change-tracking/)

### Validation
- [FluentValidation Docs](https://docs.fluentvalidation.net/)
