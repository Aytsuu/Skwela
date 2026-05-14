
using esecai.Application.UseCases.Assessments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using esecai.Application.DTOs;

namespace esecai.API.Controllers;

[ApiController]
[Route("api/assessment")]
public class AssessmentController : ControllerBase
{
    private readonly CreateAssessmentUseCase _createUseCase;
    private readonly GetAssessmentUseCase _getUseCase;

    public AssessmentController(CreateAssessmentUseCase createUseCase, GetAssessmentUseCase getUseCase)
    {
        _createUseCase = createUseCase;
        _getUseCase = getUseCase;
    }

    public class CreateAssessmentRequest
    {
        public List<IFormFile>? files { get; set; }
        public Guid classId { get; set; }
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateAssessment([FromForm] CreateAssessmentRequest request)
    {
        try 
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Safely parse string to type Guid
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            if (request == null)
            {
                return BadRequest(new { message = "Request payload is missing or malformed." });
            }

            var fileReqs = new List<AssessmentFileRequest>();
            if (request.files != null)
            {
                foreach (var file in request.files)
                {
                    fileReqs.Add(new AssessmentFileRequest(
                        file.OpenReadStream(),
                        file.FileName,
                        file.ContentType
                    ));
                }
            }

            // Await execution
            await _createUseCase.ExecuteCreateAsync(fileReqs, request.classId);
            return Ok(new { message = "Assessment processing started." });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, stack = e.StackTrace });
        }
    }

    [Authorize]
    [HttpGet("get")]
    public async Task<IActionResult> GetAssessmentList([FromQuery] Guid classId)
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Safely parse string to type Guid
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            var assessments = await _getUseCase.ExecuteGetAssessmentListAsync(classId);
            
            return Ok(assessments);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, stack = e.StackTrace });
        }
    }
}   