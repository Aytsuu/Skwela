public record CreateClassroomDto(
    Guid userId,
    string? name = null,
    string? description = null
);