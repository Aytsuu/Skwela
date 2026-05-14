using System;
using System.IO;

namespace esecai.Application.DTOs;

public record AssessmentFileRequest(
    Stream stream,
    string fileName,
    string contentType
);

public record AssessmentData(
  Guid id,
  string title,
  string type,
  float totalPoints,
  string status,
  DateTime createdAt,
  DateTime updatedAt
);