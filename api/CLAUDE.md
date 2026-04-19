# API Agent CLAUDE.md

## Stack
- .NET 8
- Entity Framework Core 8 (Code-First)
- SQL Server
- AutoMapper
- JWT Authentication (via `Microsoft.AspNetCore.Authentication.JwtBearer`)
- Swagger / Swashbuckle for OpenAPI generation

## Project Structure
```
api/
├── Controllers/         ← Thin controllers only — no business logic
├── Services/
│   ├── Interfaces/      ← IUserService, IProductService, etc.
│   └── Implementations/ ← UserService.cs, ProductService.cs, etc.
├── Repositories/
│   ├── Interfaces/      ← IRepository<T>, IUserRepository, etc.
│   └── Implementations/ ← Generic + specific repository implementations
├── Models/
│   ├── Entities/        ← EF entity classes (never sent to client)
│   └── Dtos/            ← Request and response DTOs
├── Mapping/             ← AutoMapper profiles (one profile per module)
├── Data/
│   └── AppDbContext.cs  ← Single DbContext for the application
├── Middleware/          ← Custom middleware (error handling, logging)
└── Program.cs           ← App bootstrap, DI registration
```

## Architecture Rules

### Controllers
- Controllers are **thin** — validate input, call a service, return a result
- Never inject `AppDbContext` directly into a controller — always go through a service
- Always return `ActionResult<T>` with explicit status codes
- Prefix all routes with `[Route("api/[controller]")]`

```csharp
// CORRECT
[HttpGet("{id}")]
public async Task<ActionResult<UserDto>> GetUser(int id, CancellationToken ct)
{
    var user = await _userService.GetByIdAsync(id, ct);
    if (user is null) return NotFound();
    return Ok(user);
}

// WRONG — business logic in controller
[HttpGet("{id}")]
public async Task<ActionResult<UserDto>> GetUser(int id)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    ...
}
```

### Services
- All business logic lives in `/Services/Implementations/`
- Services depend on repository interfaces, not EF directly
- Every public method must be async and accept a `CancellationToken`
- Services return DTOs, never entities

### Repositories
- Use a generic `IRepository<T>` for standard CRUD
- Create specific repository interfaces only when custom queries are needed
- Repositories are the only layer that touches `AppDbContext`

### DTOs
- Separate Request and Response DTOs where shapes differ:
  - `CreateUserRequest`, `UpdateUserRequest`
  - `UserDto` (response)
- DTOs live in `/Models/Dtos/` in a subfolder per module: `/Models/Dtos/Users/`
- Never include navigation properties or EF-specific fields in DTOs

### AutoMapper
- One profile per module in `/Mapping/`: `UserMappingProfile.cs`
- Register all profiles in `Program.cs` via `AddAutoMapper(typeof(Program))`
- Map entities → DTOs in profiles; never map manually in services

```csharp
// UserMappingProfile.cs
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<CreateUserRequest, User>();
    }
}
```

## Entity Framework Conventions
- All entities inherit from a `BaseEntity` with `Id`, `CreatedAt`, `UpdatedAt`
- Use Fluent API for configuration — no data annotations on entities
- Configuration classes live alongside entities or in a `/Data/Configurations/` folder
- **Always** create a new migration after changing entities:
  ```bash
  dotnet ef migrations add <DescriptiveMigrationName>
  dotnet ef database update
  ```
- Migration names use PascalCase and describe the change: `AddLastLoginAtToUsers`
- Never modify an existing migration — always add a new one

## Authentication
- JWT secret, issuer, and audience configured in `appsettings.json` under `"Jwt"` key
- `AuthService` handles login and token generation
- Token validation configured in `Program.cs`
- Protect endpoints with `[Authorize]` attribute
- Public endpoints explicitly marked with `[AllowAnonymous]`

## Error Handling
- Global exception handling middleware in `/Middleware/ExceptionMiddleware.cs`
- Services throw typed exceptions (e.g., `NotFoundException`, `ValidationException`)
- Middleware maps exceptions to appropriate HTTP status codes
- Never return raw exception messages to the client in production

## OpenAPI / Swagger
- Swagger UI available at `/swagger` in development only
- After any endpoint changes, regenerate the spec for the frontend:
  ```bash
  dotnet swagger tofile --output ../contracts/openapi.yaml your-api.dll v1
  ```
- XML comments on controllers and DTOs are included in the spec

## Dependency Injection Registration Pattern
Register services in `Program.cs` grouped by layer:
```csharp
// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
```
