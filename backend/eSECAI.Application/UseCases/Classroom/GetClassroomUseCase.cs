using esecai.Domain.Entities;
using esecai.Application.Interfaces;
using esecai.Application.DTOs;

namespace esecai.Application.UseCases.Classrooms;

/// <summary>
/// Use case for retrieving classroom information
/// Handles both listing classrooms and fetching detailed information with authorization checks
/// </summary>
public class GetClassroomUseCase
{
    private readonly IClassroomRepository _classroomRepo;
    private readonly IMinioFileService _minioFileService;

    /// <summary>
    /// Initializes the GetClassroomUseCase with the classroom repository
    /// </summary>
    /// <param name="repository">Repository for classroom data operations</param>
    public GetClassroomUseCase(IClassroomRepository classroomRepo, IMinioFileService minioFileService)
    {
        _classroomRepo = classroomRepo;
        _minioFileService = minioFileService;
    }

    /// <summary>
    /// Retrieves all classrooms created by a specific user (teacher)
    /// </summary>
    /// <param name="userId">The ID of the user whose classrooms to retrieve</param>
    /// <returns>Collection of Classroom entities created by the user</returns>
    public async Task<IEnumerable<ClassroomDataResponse>> ExecuteGetByCreatorAsync(Guid userId)
    {
        var classrooms = await _classroomRepo.GetClassroomsByCreatorAsync(userId);
        
        var responseList = new List<ClassroomDataResponse>();
        foreach (var c in classrooms)
        {
            string signedLink = string.IsNullOrWhiteSpace(c.class_banner) 
                ? c.class_banner 
                : await _minioFileService.GetSignedLink(c.class_banner);
                
            responseList.Add(new ClassroomDataResponse(
                c.class_id,
                c.class_name,
                c.class_description,
                signedLink,
                c.class_created_at,
                null
            ));
        }
        return responseList;
    }

    /// <summary>
    /// Retrieves detailed information about a specific classroom
    /// </summary>
    /// <param name="classId">The ID of the classroom to retrieve</param>
    /// <param name="userId">The ID of the user requesting access</param>
    /// <returns>ClassroomResponse with classroom details and teacher information</returns>
    /// <exception cref="KeyNotFoundException">Thrown if classroom does not exist</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown if user is not authorized to access the classroom</exception>
    public async Task<ClassroomDataResponse> ExecuteGetClassroomDataAsync(Guid classId, Guid userId)
    {
        // Retrieve classroom with authorization checks
        var classroom = await _classroomRepo.GetClassroomDataAsync(classId);

        if (classroom == null)
        {
            throw new KeyNotFoundException("Classroom not found");
        }
        
        if (classroom.user == null)
        {
            throw new InvalidOperationException($"The creator data for classroom {classroom.class_id} was not loaded from the database.");
        }

        string signedLink = string.Empty;

        if (!string.IsNullOrWhiteSpace(classroom.class_banner))
        {
            signedLink = await _minioFileService.GetSignedLink(classroom.class_banner);
        }

        // Build and return response DTO
        return new ClassroomDataResponse(
            classroom.class_id,
            classroom.class_name,
            classroom.class_description,
            signedLink,
            classroom.class_created_at,
            new UserData(
                classroom.user.user_id,
                classroom.user.email,
                classroom.user.display_name,
                classroom.user.display_image
            )         
        );
    }
}