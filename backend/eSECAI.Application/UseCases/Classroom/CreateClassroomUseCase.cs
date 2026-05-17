using esecai.Application.Interfaces;
using esecai.Domain.Entities;
using esecai.Application.DTOs;

namespace esecai.Application.UseCases.Classrooms;

/// <summary>
/// Use case for creating new classrooms
/// Handles validation and persistence of classroom data
/// </summary>
public class CreateClassroomUseCase
{
    private readonly IClassroomRepository _repository;
    private readonly IMinioFileService _minioFileService;
    
    /// <summary>
    /// Initializes the CreateClassroomUseCase with the classroom repository
    /// </summary>
    /// <param name="repository">Repository for classroom data operations</param>
    public CreateClassroomUseCase(IClassroomRepository repository, IMinioFileService minioFileService)
    {
        _repository = repository;
        _minioFileService = minioFileService;
    }

    /// <summary>
    /// Executes the classroom creation process
    /// Validates classroom data through domain entity and persists to database
    /// </summary>
    /// <param name="request">CreateClassroomRequest containing userId, classroom name, and description</param>
    /// <returns>The newly created Classroom entity with generated ID and timestamp</returns>
    /// <exception cref="DomainException">Thrown if classroom data fails domain validation</exception>
    public async Task<ClassroomDataResponse> ExecuteCreateClassroomAsync(CreateClassroomRequest request)
    {
        string? imageUrl = null;

        // 1. If a file was uploaded, send it to MinIO
        if (
            request.bannerStream != null && 
            request.fileName != null &&
            request.contentType != null
        )
        {
            imageUrl = await _minioFileService.UploadFileAsync(
                request.bannerStream, 
                request.fileName, 
                request.contentType
            );
        }

        // Create classroom with domain validation
        var classroom = Classroom.Build(request.userId, request.name, request.description, imageUrl);
        var instance = await _repository.AddAsync(classroom);
        
        // Persist to database
        return new ClassroomDataResponse(
            instance.class_id,
            instance.class_name,
            instance.class_description,
            instance.class_banner,
            instance.class_created_at,
            null,
            []
        );
    }
}
