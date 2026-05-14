using System.ComponentModel.DataAnnotations.Schema;
using esecai.Domain.Entities;

namespace esecai.Domain.Entities;

public class RecordAnswer
{
    public Guid ra_id { get; set; }

    [Column(TypeName = "jsonb")]
    public string ra_student_ans { get; set; } = "null";

    public float ra_awarded_pts { get; set; }
    public float ra_ai_confidence { get; set; }
    public string ra_feedback { get; set; } = default!;
    public bool ra_teacher_rev { get; set; } // teacher review required
    public float ra_teacher_op { get; set; } // teacher override points
    
    [Column(TypeName = "jsonb")]
    public string ra_raw_response { get; set; } = "null";

    public Guid quest_id { get; set; }
    public Guid rec_id { get; set; }

    [ForeignKey("quest_id")]
    public Question? question { get; set; }

    [ForeignKey("rec_id")]
    public Record? record { get; set; }

    public static RecordAnswer Build(
        string studentAnswer,
        float awardedPts,
        float aiConfidence,
        string feedback,
        float teacherOP,
        string rawResponse,
        Guid questId,
        Guid recId
    )
    {
        return new RecordAnswer
        {
            ra_id = Guid.NewGuid(),
            ra_student_ans = string.IsNullOrWhiteSpace(studentAnswer) ? "null" : studentAnswer,
            ra_awarded_pts = awardedPts,
            ra_ai_confidence = aiConfidence,
            ra_feedback = feedback,
            ra_teacher_op = teacherOP,
            ra_raw_response = string.IsNullOrWhiteSpace(rawResponse) ? "null" : rawResponse,
            quest_id = questId,
            rec_id = recId
        };
    }
}