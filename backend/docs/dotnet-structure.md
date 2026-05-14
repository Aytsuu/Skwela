# .NET Structure

## Scope

This is the current backend `.NET` folder and file structure from the live working tree.

- Included: source files, solution/project files, runtime config, migrations, docs, test assets, and backend-local tooling files.
- Excluded from the tree below: generated `bin/` and `obj/` directories.
- Listed separately: local IDE metadata under `.vs/`.

## Source Tree

```text
backend/
+-- .env
+-- AGENTS.md
+-- docker-compose.yml
+-- Dockerfile
+-- esecai.sln
+-- Makefile
+-- .config/
|   +-- dotnet-tools.json
+-- docs/
|   +-- backend-guidelines.md
|   +-- backend_entities_diagrams.md
|   +-- dotnet-structure.md
+-- esecai.API/
|   +-- appsettings.Development.json
|   +-- appsettings.json
|   +-- esecai.API.csproj
|   +-- esecai.API.csproj.user
|   +-- esecai.API.http
|   +-- Program.cs
|   +-- Controllers/
|   |   +-- AssessmentController.cs
|   |   +-- AuthController.cs
|   |   +-- ClassroomController.cs
|   |   +-- QuestionController.cs
|   |   +-- RecordController.cs
|   |   +-- rules.md
|   +-- Properties/
|       +-- launchSettings.json
+-- esecai.Application/
|   +-- Class1.cs
|   +-- DependencyInjection.cs
|   +-- esecai.Application.csproj
|   +-- DTOs/
|   |   +-- AIOptions.cs
|   |   +-- AssessmentDto.cs
|   |   +-- AuthDto.cs
|   |   +-- ClassroomDto.cs
|   |   +-- PdfDto.cs
|   |   +-- QuestionDto.cs
|   |   +-- rules.md
|   +-- Interfaces/
|   |   +-- IAIClientService.cs
|   |   +-- IAssessmentRepository.cs
|   |   +-- IAuthRepository.cs
|   |   +-- IAuthService.cs
|   |   +-- IClassroomRepository.cs
|   |   +-- IEmailService.cs
|   |   +-- IMinioFileService.cs
|   |   +-- INotificationService.cs
|   |   +-- IPdfService.cs
|   |   +-- IQuestionRepository.cs
|   |   +-- IRedisCacheService.cs
|   |   +-- IUnitOfWork.cs
|   +-- UseCases/
|       +-- rules.md
|       +-- Assessment/
|       |   +-- CreateAssessmentUseCase.cs
|       |   +-- GetAssessmentUseCase.cs
|       +-- Auth/
|       |   +-- CreateUserUseCase.cs
|       |   +-- GetUserUseCase.cs
|       |   +-- UpdateUserUseCase.cs
|       |   +-- VerifyUserUseCase.cs
|       +-- Classroom/
|       |   +-- CreateClassroomUseCase.cs
|       |   +-- DeleteClassroomUseCase.cs
|       |   +-- GetClassroomUseCase.cs
|       |   +-- UpdateClassroomUseCase.cs
|       +-- Question/
|           +-- GetQuestionUseCase.cs
+-- esecai.Domain/
|   +-- Class1.cs
|   +-- esecai.Domain.csproj
|   +-- Entities/
|   |   +-- Assessment.cs
|   |   +-- Classroom.cs
|   |   +-- Notification.cs
|   |   +-- Question.cs
|   |   +-- Record.cs
|   |   +-- RecordAnswer.cs
|   |   +-- rules.md
|   |   +-- User.cs
|   +-- Exceptions/
|       +-- DomainException.cs
|       +-- EmailAlreadyExistException.cs
|       +-- EmailNotVerifiedException.cs
+-- esecai.Infrastructure/
|   +-- DependencyInjection.cs
|   +-- esecai.Infrastructure.csproj
|   +-- Data/
|   |   +-- AppDbContext.cs
|   |   +-- UnitOfWork.cs
|   +-- Hubs/
|   |   +-- NotificationHub.cs
|   +-- Migrations/
|   |   +-- 20260331110217_CleanedEntites.cs
|   |   +-- 20260331110217_CleanedEntites.Designer.cs
|   |   +-- 20260331161402_AddedNewEntities.cs
|   |   +-- 20260331161402_AddedNewEntities.Designer.cs
|   |   +-- 20260404054457_RemoveRubricMetadata.cs
|   |   +-- 20260404054457_RemoveRubricMetadata.Designer.cs
|   |   +-- 20260404195528_AddedRubricInQuestion.cs
|   |   +-- 20260404195528_AddedRubricInQuestion.Designer.cs
|   |   +-- AppDbContextModelSnapshot.cs
|   +-- Repositories/
|   |   +-- AssessmentRepository.cs
|   |   +-- AuthRepository.cs
|   |   +-- ClassroomRepository.cs
|   |   +-- QuestionRepository.cs
|   |   +-- rules.md
|   +-- Services/
|       +-- AIClientService.cs
|       +-- AuthService.cs
|       +-- EmailService.cs
|       +-- MinioFileService.cs
|       +-- NotificationService.cs
|       +-- PdfService.cs
|       +-- RedisCacheService.cs
|       +-- rules.md
|       +-- Docs/
|           +-- assessment-extraction.md
|           +-- GeminiPrompts.cs
|           +-- OllamaPrompts.cs
+-- esecai.Tests/
    +-- AIServiceTests.cs
    +-- esecai.Tests.csproj
    +-- exam.pdf
    +-- PdfServiceTests.cs
    +-- UnitTest1.cs
    +-- TestOutput/
        +-- extracted_assessment.txt
        +-- structured_output_merged.json
        +-- Test_III__Fill_in_the_Blanks_(10_pts).json
        +-- Test_II__True_or_False_(10_pts).json
        +-- Test_IV__Matching_Type_(15_pts).json
        +-- Test_I__Multiple_Choice_(20_pts).json
        +-- Test_VII__Essay___Short_Answer_(10_pts).json
        +-- Test_VI__Programming___Problem_Solving_(15_pts).json
        +-- Test_V__Hand_Tracing___Code_Analysis_(20_pts).json
```

## Local IDE Metadata

These files are present locally under `backend/.vs/`:

```text
backend/.vs/
+-- ProjectEvaluation/
|   +-- skwela.metadata.v7.bin
|   +-- skwela.projects.v7.bin
+-- Skwela/
    +-- config/
    |   +-- applicationhost.config
    +-- DesignTimeBuild/
    |   +-- .dtbcache.v2
    +-- FileContentIndex/
    |   +-- 9ffc505a-d743-4995-82fc-6e00244bfe11.vsidx
    |   +-- a5bccb54-3dc5-4566-94c3-f0294f01fbc5.vsidx
    |   +-- bfb18282-2d9d-49a5-b48b-7a65d1e7d5a5.vsidx
    |   +-- e0ecbd36-8961-4b95-a6b6-4e404e63c3e0.vsidx
    |   +-- ea28df37-e77a-48d4-9881-70d97dffe6ce.vsidx
    +-- v17/
        +-- .futdcache.v2
        +-- .suo
        +-- .wsuo
```

## Generated Directories

Generated build output also exists under:

- `backend/bin/`
- `backend/obj/`
- `backend/esecai.API/bin/`
- `backend/esecai.API/obj/`
- `backend/esecai.Application/bin/`
- `backend/esecai.Application/obj/`
- `backend/esecai.Domain/bin/`
- `backend/esecai.Domain/obj/`
- `backend/esecai.Infrastructure/bin/`
- `backend/esecai.Infrastructure/obj/`
- `backend/esecai.Tests/bin/`
- `backend/esecai.Tests/obj/`

These were intentionally omitted from the main tree because they are generated artifacts, not source structure.
