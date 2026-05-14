
namespace esecai.Application.DTOs;

public record QuestionData(
  Guid id,
  int num,
  string type,
  string text,
  string answer,
  string rubric,
  float maxPoints,
  float aiConfidence
);