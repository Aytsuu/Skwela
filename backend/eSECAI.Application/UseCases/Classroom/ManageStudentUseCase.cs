using esecai.Application.DTOs;
using esecai.Application.Interfaces;
using esecai.Domain.Entities;

namespace esecai.Application.UseCases.Classrooms;

public class ManageStudentUseCase
{
    private readonly IClassroomRepository _classroomRepository;

    public ManageStudentUseCase(IClassroomRepository classroomRepository)
    {
        _classroomRepository = classroomRepository;
    }

    public async Task<StudentDataResponse> CreateStudentAsync(CreateStudentRequest request)
    {
        await EnsureClassroomExistsAsync(request.classId);
        await EnsureStudentDoesNotExistAsync(request.classId, request.fname, request.mname, request.lname, null);

        var student = Student.Build(request.classId, request.fname, request.mname, request.lname);
        var createdStudent = await _classroomRepository.AddStudentAsync(student);

        return MapStudent(createdStudent);
    }

    public async Task<IReadOnlyList<StudentDataResponse>> ImportStudentsAsync(
        Guid classId,
        Stream csvStream,
        CancellationToken cancellationToken)
    {
        await EnsureClassroomExistsAsync(classId);

        var parsedStudents = await StudentCsvParser.ParseAsync(csvStream, cancellationToken);
        if (parsedStudents.Count == 0)
        {
            throw new InvalidOperationException("CSV file does not contain any student rows.");
        }

        var normalizedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var newStudents = new List<Student>();

        foreach (var parsedStudent in parsedStudents)
        {
            var key = BuildStudentKey(parsedStudent.fname, parsedStudent.mname, parsedStudent.lname);
            if (!normalizedKeys.Add(key))
            {
                throw new InvalidOperationException($"Duplicate student found in CSV: {parsedStudent.fname} {parsedStudent.lname}");
            }

            await EnsureStudentDoesNotExistAsync(classId, parsedStudent.fname, parsedStudent.mname, parsedStudent.lname, null);
            newStudents.Add(Student.Build(classId, parsedStudent.fname, parsedStudent.mname, parsedStudent.lname));
        }

        var createdStudents = await _classroomRepository.AddStudentsAsync(newStudents);
        return createdStudents.Select(MapStudent).ToList();
    }

    public async Task<StudentDataResponse> UpdateStudentAsync(UpdateStudentRequest request)
    {
        await EnsureStudentDoesNotExistAsync(request.classId, request.fname, request.mname, request.lname, request.studentId);

        var student = await _classroomRepository.GetStudentAsync(request.classId, request.studentId);
        student.student_fname = Student.NormalizeNamePart(request.fname);
        student.student_mname = Student.NormalizeNamePart(request.mname);
        student.student_lname = Student.NormalizeNamePart(request.lname);

        await _classroomRepository.UpdateClassroomAsync();

        return MapStudent(student);
    }

    public async Task DeleteStudentAsync(Guid classId, Guid studentId)
    {
        var student = await _classroomRepository.GetStudentAsync(classId, studentId);
        await _classroomRepository.DeleteStudentAsync(student);
    }

    private async Task EnsureClassroomExistsAsync(Guid classId)
    {
        await _classroomRepository.GetClassroomDataAsync(classId);
    }

    private async Task EnsureStudentDoesNotExistAsync(
        Guid classId,
        string firstName,
        string? middleName,
        string lastName,
        Guid? excludeStudentId)
    {
        var exists = await _classroomRepository.StudentExistsAsync(
            classId,
            Student.NormalizeNamePart(firstName),
            Student.NormalizeNamePart(middleName),
            Student.NormalizeNamePart(lastName),
            excludeStudentId);

        if (exists)
        {
            throw new InvalidOperationException("A student with the same full name already exists in this classroom.");
        }
    }

    private static string BuildStudentKey(string firstName, string? middleName, string lastName)
    {
        return $"{Student.NormalizeNamePart(firstName)}|{Student.NormalizeNamePart(middleName)}|{Student.NormalizeNamePart(lastName)}";
    }

    private static StudentDataResponse MapStudent(Student student)
    {
        return new StudentDataResponse(
            student.student_id,
            student.student_fname,
            student.student_mname,
            student.student_lname);
    }
}
