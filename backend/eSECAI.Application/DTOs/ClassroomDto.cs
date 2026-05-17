
namespace esecai.Application.DTOs;

/// <summary>
/// Classroom
/// Everything should be in JS naming convention
/// </summary>
public record UserData(
  Guid userId,
  string email,
  string displayName,
  string displayImage
);

public record StudentDataResponse(
    Guid studentId,
    string fname,
    string mname,
    string lname
);

public record ClassroomDataResponse(
    Guid classId, 
    string className, 
    string classDescription, 
    string classBanner,
    DateTime classCreatedAt,
    UserData? creator,
    IReadOnlyList<StudentDataResponse> students
);

public record CreateClassroomRequest(
    Guid userId,
    string? name,
    string? description,
    Stream? bannerStream,
    string? contentType,
    string? fileName
);

public record UpdateClassroomRequest(
    Guid classId,
    string? name,
    string? description,
    Stream? bannerStream,
    string? contentType,
    string? fileName
);

public record CreateStudentRequest(
    Guid classId,
    string fname,
    string? mname,
    string lname
);

public record UpdateStudentRequest(
    Guid classId,
    Guid studentId,
    string fname,
    string? mname,
    string lname
);
