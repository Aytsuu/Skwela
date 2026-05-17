using esecai.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace esecai.Domain.Entities;

public class Student
{
    public Guid student_id { get; set; }
    public string student_fname { get; set; } = default!;
    public string student_mname { get; set; } = string.Empty;
    public string student_lname { get; set; } = default!;
    public DateTime student_created_at { get; set; }
    public DateTime student_updated_at { get; set; }
    public Guid class_id { get; set; }

    [ForeignKey("class_id")]
    public Classroom? classroom { get; set; }

    public ICollection<Record> records { get; set; } = new List<Record>();

    public static Student Build(Guid classId, string? firstName, string? middleName, string? lastName)
    {
        if (classId == Guid.Empty)
            throw new DomainException("Student must belong to a classroom.");

        var sanitizedFirstName = NormalizeNamePart(firstName);
        var sanitizedMiddleName = NormalizeNamePart(middleName);
        var sanitizedLastName = NormalizeNamePart(lastName);

        if (string.IsNullOrWhiteSpace(sanitizedFirstName))
            throw new DomainException("Student first name is required.");

        if (string.IsNullOrWhiteSpace(sanitizedLastName))
            throw new DomainException("Student last name is required.");

        return new Student
        {
            student_id = Guid.NewGuid(),
            class_id = classId,
            student_fname = sanitizedFirstName,
            student_mname = sanitizedMiddleName,
            student_lname = sanitizedLastName
        };
    }

    public static string NormalizeNamePart(string? value)
    {
        return value?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}
