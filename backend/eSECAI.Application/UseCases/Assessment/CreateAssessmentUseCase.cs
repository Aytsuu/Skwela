using esecai.Application.Interfaces;
using esecai.Domain.Entities;
using esecai.Application.DTOs;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;

namespace esecai.Application.UseCases.Assessments;

public class CreateAssessmentUseCase
{
    private readonly IAssessmentRepository _asmtRepo;
    private readonly IQuestionRepository _qstnRepo;
    private readonly IPdfService _pdfService;
    private readonly IAIClientService _aiService;
    private readonly IMinioFileService _minioFileService;
    private readonly IUnitOfWork _uow;

    public CreateAssessmentUseCase(
        IAssessmentRepository asmtRepo, 
        IQuestionRepository qstnRepo, 
        IPdfService pdfService, 
        IAIClientService aiService, 
        IMinioFileService minioFileService,
        IUnitOfWork uow)
    {
        _asmtRepo = asmtRepo;
        _qstnRepo = qstnRepo;
        _pdfService = pdfService;
        _aiService = aiService;
        _minioFileService = minioFileService;
        _uow = uow;
    }

    public async Task ExecuteCreateAsync(IEnumerable<AssessmentFileRequest> files, Guid classId)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        foreach (var file in files)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                // Detect text
                var text_presence = _pdfService.DetectTextPresence(file.stream);

                if (!text_presence.HasEmbeddedText)
                {
                    throw new InvalidOperationException();
                }

                // Reset stream position before reading again
                file.stream.Position = 0;

                // Extract text
                var extractraction_result = _pdfService.ExtractFromDigitalPdf(file.stream);
                
                // Format string exactly like saving to and reading from a text file
                // Let's use a MemoryStream to replicate the UTF-8 file I/O behavior in-memory
                string cleanedText;
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream, System.Text.Encoding.UTF8, leaveOpen: true))
                    {
                        await writer.WriteAsync(extractraction_result.NormalizedText);
                        await writer.FlushAsync(); // Ensure everything is written to the stream
                    }
                    memoryStream.Position = 0;
                    using (var reader = new StreamReader(memoryStream, System.Text.Encoding.UTF8))
                    {
                        cleanedText = await reader.ReadToEndAsync();
                    }
                }

                var structured_data_string = await _aiService.GenerateAsync(cleanedText, 8192);
                var structured_data = JsonNode.Parse(structured_data_string); 

                if (structured_data == null) continue;
                
                var sections = structured_data["sections"]!.AsArray();

                // Store file in minio storage
                string pdfUrl = await _minioFileService.UploadFileAsync(
                    file.stream, 
                    file.fileName, 
                    file.contentType
                );

                Assessment assessment = Assessment.Build(
                    structured_data["title"]?.GetValue<string>() ?? "Untitled Assessment",
                    structured_data["type"]?.GetValue<string>() ?? "exam",
                    pdfUrl,
                    structured_data["inst"]?.GetValue<string>() ?? "",
                    structured_data["t_pts"] != null ? float.Parse(structured_data["t_pts"]!.ToString()) : 0f,
                    classId
                );
                
                // Store the assessment metadata
                await _asmtRepo.CreateAssessmentAsync(assessment);

                if (sections != null)
                {
                    var allQuestions = new List<Question>();
                    // Store questions
                    foreach (var sec in sections)
                    {
                        if (sec == null) continue;

                        var qts = sec["qts"]?.AsArray();
                        
                        if (qts == null) continue;

                        foreach (var qt in qts)
                        {
                            if (qt == null) continue;

                            Question question = Question.Build(
                                qt["num"] != null ? int.Parse(qt["num"]!.ToString()) : 0,
                                qt["type"]?.GetValue<string>() ?? "unknown",
                                qt["txt"]?.GetValue<string>() ?? "",
                                qt["ans"]?.ToJsonString() ?? "null",
                                qt["rub"]?.ToJsonString() ?? "null",
                                qt["pts"] != null ? float.Parse(qt["pts"]!.ToString()) : 0f,
                                qt["conf"] != null ? float.Parse(qt["conf"]!.ToString()) : 0f,
                                assessment.ass_id
                            );

                            allQuestions.Add(question);
                        
                        }
                    }

                    await _qstnRepo.CreateQuestionsBulkAsync(allQuestions);
                }

                // Commit the transaction after successfully processing the file
                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        stopwatch.Stop();
        Console.WriteLine($"Processing Time: {stopwatch.ElapsedMilliseconds} ms");
    }
}