# AiTutor

AiTutor is a children-focused AI learning assistant for Android tablets. Phase 0 only initializes the solution and project structure.

## Projects

- `src/AiTutor.Core`: domain models, enums, and interfaces.
- `src/AiTutor.Shared`: shared DTOs for API and MAUI.
- `src/AiTutor.Infrastructure`: infrastructure implementations such as EF Core, SQL Server, repositories, and AI providers.
- `src/AiTutor.Api`: ASP.NET Core WebAPI entry point.
- `src/AiTutor.Maui`: .NET MAUI Android tablet app.
- `tests/AiTutor.Tests`: test project.

## Run API

```powershell
dotnet run --project src/AiTutor.Api/AiTutor.Api.csproj
```

Open the development OpenAPI document at:

```text
http://localhost:5088/openapi/v1.json
```

## Build

```powershell
dotnet build AiTutor.sln
```

## Database

This project uses SQL Server. Phase 0 does not connect to a database, create tables, or generate migrations. Future development connection strings should be kept in `appsettings.Development.json`, user secrets, or environment variables, and real credentials must not be committed.
