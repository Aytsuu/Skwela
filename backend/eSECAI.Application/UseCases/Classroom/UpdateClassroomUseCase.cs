using esecai.Application.DTOs;
using esecai.Application.Interfaces;

namespace esecai.Application.UseCases.Classrooms;

public class UpdateClassroomUseCase
{
    private readonly IClassroomRepository _repository;
    private readonly IMinioFileService _minioFileService;

    public UpdateClassroomUseCase(IClassroomRepository repository, IMinioFileService minioFileService)
    {
    _repository = repository;
    _minioFileService = minioFileService;
    }

    public async Task<ClassroomDataResponse> ExecuteUpdateClassroomAsync(UpdateClassroomRequest request)
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

        // Get classroom instance
        var classroom = await _repository.GetClassroomDataAsync(request.classId);

        if (classroom == null)
        {
            throw new KeyNotFoundException("Classroom not found");
        }

        if (classroom.user == null)
        {
            throw new InvalidOperationException($"The creator data for classroom {classroom.class_id} was not loaded from the database.");
        }

        // Map non-null values to its corresponding classroom attributes
        if (request.name != null)
        {
            classroom.class_name = request.name;
        }
        if (request.description != null)
        {
            classroom.class_description = request.description;
        }
        if (imageUrl != null)
        {
            classroom.class_banner = imageUrl;
        }

        // Update classroom
        await _repository.UpdateClassroomAsync();

        return new ClassroomDataResponse(
            classroom.class_id,
            classroom.class_name,
            classroom.class_description,
            classroom.class_banner,
            classroom.class_created_at,
            new UserData(
                classroom.user.user_id,
                classroom.user.email,
                classroom.user.display_name,
                classroom.user.display_image
            ),
            classroom.students
                .OrderBy(student => student.student_lname)
                .ThenBy(student => student.student_fname)
                .Select(student => new StudentDataResponse(
                    student.student_id,
                    student.student_fname,
                    student.student_mname,
                    student.student_lname))
                .ToList()
        );
    }
}
