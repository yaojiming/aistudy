# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AiTutor is a children-focused AI learning assistant for Android tablets, targeting elementary school students. The stack is .NET 10 with an ASP.NET Core Web API backend and a .NET MAUI Android frontend.

## Build & Run

```powershell
# Build entire solution
dotnet build AiTutor.sln

# Run API (starts on http://localhost:5088, Swagger at /swagger)
dotnet run --project src/AiTutor.Api/AiTutor.Api.csproj

# Run tests
dotnet test tests/AiTutor.Tests/AiTutor.Tests.csproj

# EF Core migrations
dotnet ef migrations add <Name> --project src/AiTutor.Infrastructure --startup-project src/AiTutor.Api
dotnet ef migrations script --project src/AiTutor.Infrastructure --startup-project src/AiTutor.Api -o scripts/sqlserver/<Name>.sql
```

MAUI is currently configured with `RuntimeIdentifier=android-x64` for x86_64 emulator. Target device is API 23 / 32-bit ARM — this RID mismatch must be resolved with `android-arm` before real device deployment.

## Solution Structure

```
src/AiTutor.Core        — Domain models, enums, interfaces. No EF, no HTTP, no SDK deps.
src/AiTutor.Shared      — DTOs shared between API and MAUI (AgentRequest/Response, etc.)
src/AiTutor.Infrastructure — EF Core DbContext, repositories, Agent implementations, Providers, Services
src/AiTutor.Api         — ASP.NET Core Web API. Controllers only handle HTTP; business logic in Infrastructure.
src/AiTutor.Maui        — .NET MAUI Android app. References only Shared. MVVM.
tests/AiTutor.Tests     — xUnit test project
```

**Project references:** Infrastructure → Core + Shared. Api → Core + Shared + Infrastructure. Maui → Shared. Tests → Core + Infrastructure.

## Architecture: Request Flow

```
MAUI Client (ApiClientService) ──HTTP──▶ AgentController.AskAsync()
  ▶ AgentService (creates session, question, messages)
  ▶ AgentRouter.Route(request.Mode, request.InputType)  — rule-based, not LLM
  ▶ IAgent.ExecuteAsync()  — ChatAgent / VisionAgent / HomeworkCheckAgent / etc.
  ▶ ITextModelProvider / IVisionModelProvider  — Mock or Real (DeepSeek / GLM)
  ▶ Response + logs (SessionMessage, AnswerRecord, AgentRouteLog, ModelCallLog)
```

## Agent Routing Rules

All routing is rule-based (see `AgentRouter.Route()`):
1. `Mode=check_homework` → HomeworkCheckAgent (uses vision model)
2. `Mode=wrong_review` → WrongBookAgent
3. `Mode=textbook_qa` → TextbookRagAgent
4. `InputType=image` → VisionAgent
5. Default → ChatAgent
6. `voice_chat` → VoiceAgent (placeholder)
7. `avatar_explain` → AvatarAgent (placeholder)

## AI Provider Configuration

Providers switch via `AiProviders:UseMock` in appsettings:
- `true` → MockTextModelProvider / MockVisionModelProvider (no API Key needed)
- `false` → DeepSeekTextModelProvider / GlmVisionModelProvider (requires ApiKey from config)

## Critical Constraints

1. **API keys MUST NOT be committed.** Currently violated — see next section.
2. **Target device is fixed:** Android 6.0.1 / API 23 / 32-bit ARM. Do not upgrade to higher minimum API without explicit approval.
3. **Do not refactor or introduce abstractions beyond the task.** The project follows strict phase-by-phase development (see PLANS.md).
4. **Do not delete docs/ files.**
5. **SQL Server only, not SQLite.**
6. **MAUI client must NOT call AI models directly or store API Keys.**
7. **Ids are `NVARCHAR(64)`, generated via `Guid.NewGuid().ToString("N")`.**
8. **All entities have `CreatedTime`; mutable entities have `UpdatedTime`.**
9. **Every Agent request must save:** LearningSession, SessionMessage, QuestionRecord, AnswerRecord, AgentRouteLog, ModelCallLog.
10. **Prompts are centralized** in `PromptTemplateService`; Controllers never write prompts.
11. **Controller must not operate on DbContext directly** — use Services.

## API Keys in Config — Known Issue

`src/AiTutor.Api/appsettings.json` currently contains real DeepSeek and Zhipu API keys. This file is tracked in git. These keys are exposed and must be rotated. The intended design is:
- `appsettings.json` — placeholders or empty strings
- `appsettings.Development.json` — local keys (excluded via `.gitignore` but may already be tracked)
- User secrets / environment variables — production keys

`.gitignore` already excludes `appsettings.Development.json` and `*.Development.json`.

## Database

- SQL Server, database name: `AiTutorDb`
- Default connection string: `Server=.;Database=AiTutorDb;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;`
- 30 tables configured in `AiTutorDbContext` with full indexes, column comments, and enum conversions
- Two migrations exist: `InitialCreate` and `AddTableAndColumnComments`
- **Never execute DROP DATABASE / DROP TABLE / TRUNCATE TABLE.**
- Generate SQL script alongside every migration into `scripts/sqlserver/`.

## MAUI Conventions

- MVVM: Views in `Views/`, ViewModels in `ViewModels/`, Services in `Services/`, Controls in `Controls/`
- Navigation: Shell-based, routes registered in `AppShell.xaml.cs`
- Backend URL: configurable via SettingsPage, stored in `Preferences`, default from `Resources/Raw/appsettings.json`
- `FormattedAnswerView` uses a WebView for Markdown/LaTeX rendering (MathJax CDN). No JSBridge framework exists yet.
- OCR: Android MLKit via `AndroidOcrService`, iOS/macOS not implemented.
