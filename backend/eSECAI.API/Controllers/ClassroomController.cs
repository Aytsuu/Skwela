
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using esecai.Application.UseCases.Classrooms;
using esecai.Application.DTOs;
using System.Security.Claims;

namespace esecai.API.Controllers;

/// <summary>
/// Classrooms Controller
/// Handles classroom creation, retrieval, and deletion operations
/// Only authenticated users (teachers/admins) can access these endpoints
/// </summary>
[ApiController]
[Route("api/classroom")]
public class ClassroomController : ControllerBase
{
    private readonly CreateClassroomUseCase _createUseCase;
    private readonly GetClassroomUseCase _getUseCase;
    private readonly DeleteClassroomUseCase _deleteUseCase;
    private readonly UpdateClassroomUseCase _updateUseCase;
    private readonly ManageStudentUseCase _manageStudentUseCase;

    /// <summary>
    /// Initializes the ClassroomsController with required use cases
    /// </summary>
    /// <param name="createUseCase">Use case for creating classrooms</param>
    /// <param name="getUseCase">Use case for retrieving classroom data</param>
    /// <param name="deleteUseCase">Use case for deleting classrooms</param>
    public ClassroomController(
        CreateClassroomUseCase createUseCase, 
        GetClassroomUseCase getUseCase, 
        DeleteClassroomUseCase deleteUseCase,
        UpdateClassroomUseCase updateUseCase,
        ManageStudentUseCase manageStudentUseCase
    )
    {
        _createUseCase = createUseCase;
        _getUseCase = getUseCase;
        _deleteUseCase = deleteUseCase;
        _updateUseCase = updateUseCase;
        _manageStudentUseCase = manageStudentUseCase;
    }

    // This record only lives in the API/Controller layer (Must be placed here since we need IFormFile)
    public record CreateClassroomRequestWeb(
        string name,
        string? description,
        IFormFile? bannerFile
    );

    public record UpdateClassroomRequestWeb(
        string? name,
        string? description,
        IFormFile? bannerFile
    );

    public record CreateStudentRequestWeb(
        string fname,
        string? mname,
        string lname
    );

    public record UpdateStudentRequestWeb(
        string fname,
        string? mname,
        string lname
    );

    /// <summary>
    /// Creates a new classroom
    /// Only authenticated users can create classrooms
    /// </summary>
    /// <param name="request">ClassroomRequestWeb containing classroom name, description and file</param>
    /// <returns>The created Classroom object</returns>
    /// <response code="200">Classroom successfully created</response>
    /// <response code="400">Invalid classroom data or creation failed</response>
    /// <response code="401">User is not authenticated</response>
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateClassroom([FromForm] CreateClassroomRequestWeb request)
    {
        try
        {
            // Extract userId string from access token
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Safely parse string to type Guid
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            Stream? stream = null;
            if (request.bannerFile != null)
            {
                stream = request.bannerFile.OpenReadStream();
            }

            // Map the Web Request to the Application DTO
            var applicationRequest = new CreateClassroomRequest(
                userId,
                request.name,
                request.description,
                stream,
                request.bannerFile?.ContentType,
                request.bannerFile?.FileName
            );

            var classroom = await _createUseCase.ExecuteCreateClassroomAsync(applicationRequest);

            return Ok(classroom);
        }
        catch (InvalidOperationException ioEx)
        {
            return BadRequest(ioEx.Message);
        }
    }

    /// <summary>
    /// Update a classroom
    /// </summary>
    /// <param name="request">ClassroomRequestWeb containing classroom name, description and file</param>
    /// <returns>List of Classroom objects created by the user</returns>
    /// <response code="200">Classrooms successfully retrieved</response>
    /// <response code="400">Invalid user ID or retrieval failed</response>
    /// <response code="401">User is not authenticated</response>
    [Authorize]
    [HttpPatch("patch/{classId}")]
    public async Task<IActionResult> UpdateClassroom(Guid classId,[FromForm] UpdateClassroomRequestWeb request)
    {
        try
        {
            Stream? stream = null;
            if (request.bannerFile != null)
            {
                stream = request.bannerFile.OpenReadStream();
            }

            // Map the Web Request to the Application DTO
            var applicationRequest = new UpdateClassroomRequest(
                classId,
                request.name,
                request.description,
                stream,
                request.bannerFile?.ContentType,
                request.bannerFile?.FileName
            );

            var classroom = await _updateUseCase.ExecuteUpdateClassroomAsync(applicationRequest);

            return Ok(classroom);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    

    /// <summary>
    /// Retrieves all classrooms created by a specific user (teacher)
    /// </summary>
    /// <param name="userId">The ID of the user (teacher) whose classrooms to retrieve</param>
    /// <returns>List of Classroom objects created by the user</returns>
    /// <response code="200">Classrooms successfully retrieved</response>
    /// <response code="400">Invalid user ID or retrieval failed</response>
    /// <response code="401">User is not authenticated</response>
    [Authorize]
    [HttpGet("get")]
    public async Task<IActionResult> GetCreatedClassrooms()
    {
        try
        {
            // Extract userId string from access token
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Safely parse to string to type Guid
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            var classrooms = await _getUseCase.ExecuteGetByCreatorAsync(userId);
            return Ok(classrooms);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific classroom
    /// Performs authorization checks based on user role:
    /// - Teachers can only access their own classrooms
    /// </summary>
    /// <param name="classId">The ID of the classroom to retrieve</param>
    /// <param name="userId">The ID of the user requesting access</param>
    /// <param name="role">The role of the user (teacher or student)</param>
    /// <returns>ClassroomResponse with classroom details and teacher information</returns>
    /// <response code="200">Classroom data successfully retrieved</response>
    /// <response code="403">User doesn't have permission to access this classroom</response>
    /// <response code="404">Classroom not found</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">User is not authenticated</response>
    [Authorize]
    [HttpGet("get/{classId}")]
    public async Task<IActionResult> GetClassroomData(Guid classId)
    {
        try
        {
            // Extract userId string from access token
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Safely parse to string to type Guid
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            var classroom = await _getUseCase.ExecuteGetClassroomDataAsync(classId, userId);
            return Ok(classroom);
        }
        catch (KeyNotFoundException knfEx)
        {
            return NotFound(knfEx.Message);
        }
        catch (UnauthorizedAccessException uaEx)
        {
            return Unauthorized(uaEx.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("{classId}/students")]
    public async Task<IActionResult> CreateStudent(Guid classId, [FromBody] CreateStudentRequestWeb request)
    {
        try
        {
            var student = await _manageStudentUseCase.CreateStudentAsync(new CreateStudentRequest(
                classId,
                request.fname,
                request.mname,
                request.lname));

            return Ok(student);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("{classId}/students/import")]
    public async Task<IActionResult> ImportStudents(Guid classId, IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("CSV file is required.");
            }

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only CSV files are supported.");
            }

            await using var stream = file.OpenReadStream();
            var students = await _manageStudentUseCase.ImportStudentsAsync(classId, stream, cancellationToken);

            return Ok(students);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("{classId}/students/{studentId}")]
    public async Task<IActionResult> UpdateStudent(Guid classId, Guid studentId, [FromBody] UpdateStudentRequestWeb request)
    {
        try
        {
            var student = await _manageStudentUseCase.UpdateStudentAsync(new UpdateStudentRequest(
                classId,
                studentId,
                request.fname,
                request.mname,
                request.lname));

            return Ok(student);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{classId}/students/{studentId}")]
    public async Task<IActionResult> DeleteStudent(Guid classId, Guid studentId)
    {
        try
        {
            await _manageStudentUseCase.DeleteStudentAsync(classId, studentId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a classroom
    /// </summary>
    /// <param name="classId">The ID of the classroom to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="200">Classroom successfully deleted</response>
    /// <response code="400">Invalid request or deletion failed</response>
    /// <response code="401">User is not authenticated</response>
    [Authorize]
    [HttpDelete("delete/{classId}")]
    public async Task<IActionResult> DeleteClassroom(Guid classId)
    {
        try 
        {
            await _deleteUseCase.ExecuteDeleteClassroomAsync(classId);
            return Ok();
        }
        catch (InvalidOperationException ioEx)
        {
            return Conflict(ioEx.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
