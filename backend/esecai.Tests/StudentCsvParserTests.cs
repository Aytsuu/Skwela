using esecai.Application.UseCases.Classrooms;
using System.Text;

namespace esecai.Tests;

public class StudentCsvParserTests
{
    [Fact]
    public async Task ParseAsync_ReturnsStudents_FromHeaderedCsv()
    {
        var csv = """
                  fname,mname,lname
                  Ada,Lovelace,Byron
                  Grace,B.,Hopper
                  """;

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var students = await StudentCsvParser.ParseAsync(stream, CancellationToken.None);

        Assert.Collection(
            students,
            student =>
            {
                Assert.Equal("Ada", student.fname);
                Assert.Equal("Lovelace", student.mname);
                Assert.Equal("Byron", student.lname);
            },
            student =>
            {
                Assert.Equal("Grace", student.fname);
                Assert.Equal("B.", student.mname);
                Assert.Equal("Hopper", student.lname);
            });
    }

    [Fact]
    public async Task ParseAsync_AllowsMissingMiddleName()
    {
        var csv = """
                  fname,mname,lname
                  Ada,,Byron
                  """;

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var students = await StudentCsvParser.ParseAsync(stream, CancellationToken.None);

        var student = Assert.Single(students);
        Assert.Equal("Ada", student.fname);
        Assert.Equal(string.Empty, student.mname);
        Assert.Equal("Byron", student.lname);
    }

    [Fact]
    public async Task ParseAsync_Throws_WhenRequiredColumnsAreMissing()
    {
        var csv = """
                  first,last
                  Ada,Byron
                  """;

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            StudentCsvParser.ParseAsync(stream, CancellationToken.None));
    }
}
