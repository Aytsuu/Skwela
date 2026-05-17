using esecai.Application.DTOs;
using System.Text;

namespace esecai.Application.UseCases.Classrooms;

public static class StudentCsvParser
{
    public static async Task<IReadOnlyList<CreateStudentRequest>> ParseAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        using var reader = new StreamReader(
            stream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            leaveOpen: true);

        var rows = new List<string[]>();

        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rawLine = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(rawLine))
            {
                continue;
            }

            rows.Add(rawLine.Split(',').Select(cell => cell.Trim()).ToArray());
        }

        if (rows.Count == 0)
        {
            throw new InvalidOperationException("CSV file is empty.");
        }

        var header = rows[0]
            .Select(cell => cell.Trim().Trim('"').ToLowerInvariant())
            .ToArray();

        var firstNameIndex = Array.IndexOf(header, "fname");
        var middleNameIndex = Array.IndexOf(header, "mname");
        var lastNameIndex = Array.IndexOf(header, "lname");

        if (firstNameIndex < 0 || lastNameIndex < 0)
        {
            throw new InvalidOperationException("CSV must include fname, mname, and lname headers.");
        }

        var students = new List<CreateStudentRequest>();

        foreach (var row in rows.Skip(1))
        {
            var firstName = GetValue(row, firstNameIndex);
            var middleName = middleNameIndex >= 0 ? GetValue(row, middleNameIndex) : string.Empty;
            var lastName = GetValue(row, lastNameIndex);

            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                throw new InvalidOperationException("Each CSV row must include fname and lname values.");
            }

            students.Add(new CreateStudentRequest(Guid.Empty, firstName, middleName, lastName));
        }

        return students;
    }

    private static string GetValue(IReadOnlyList<string> row, int index)
    {
        if (index < 0 || index >= row.Count)
        {
            return string.Empty;
        }

        return row[index].Trim().Trim('"');
    }
}
