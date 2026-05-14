using System;
using System.IO;

namespace esecai.Application.DTOs;

public record RecordFileRequest(
    Stream stream,
    string fileName,
    string contentType
);