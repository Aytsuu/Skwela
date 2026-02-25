
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skwela.Application.UseCases.Classrooms;
using Skwela.Domain.Enums;

namespace Skwela.API.Controllers;

/// <summary>
/// Classrooms Controller
/// Handles classroom creation, retrieval, and deletion operations
/// Only authenticated users (teachers/admins) can access these endpoints
/// </summary>
[ApiController]
[Route("api/classroom")]
public class ClassroomsController : ControllerBase
{
    private readonly CreateClassroomUseCase _createUseCase;
    private readonly GetClassroomUseCase _getUseCase;
    private readonly DeleteClassroomUseCase _deleteUseCase;

    /// <summary>
    /// Initializes the ClassroomsController with required use cases
    /// </summary>
    /// <param name="createUseCase">Use case for creating classrooms</param>
    /// <param name="getUseCase">Use case for retrieving classroom data</param>
    /// <param name="deleteUseCase">Use case for deleting classrooms</param>
    public ClassroomsController(CreateClassroomUseCase createUseCase, GetClassroomUseCase getUseCase, DeleteClassroomUseCase deleteUseCase)
    {
        _createUseCase = createUseCase;
        _getUseCase = getUseCase;
        _deleteUseCase = deleteUseCase;
    }

    /// <summary>
    /// Creates a new classroom
    /// Only authenticated users can create classrooms
    /// </summary>
    /// <param name="request">CreateClassroomDto containing classroom name and description</param>
    /// <returns>The created Classroom object</returns>
    /// <response code="200">Classroom successfully created</response>
    /// <response code="400">Invalid classroom data or creation failed</response>
    /// <response code="401">User is not authenticated</response>
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateClassroom(CreateClassroomDto request)
    {
        try
        {
            var classroom = await _createUseCase.ExecuteAsync(request);

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
    [HttpGet("get/{userId}")]
    public async Task<IActionResult> GetClassroomsByUserId(Guid userId)
    {
        try
        {
            var classrooms = await _getUseCase.ExecuteGetByUserIdAsync(userId);
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
    /// - Students can only access classrooms they are enrolled in
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
    [HttpGet("get/{classId}/{userId}/{role}")]
    public async Task<IActionResult> GetClassroomData(Guid classId, Guid userId, UserRole role)
    {
        try
        {
            var classroom = await _getUseCase.ExecuteGetClassroomDataAsync(classId, userId, role);
            return Ok(classroom);
        }
        catch (KeyNotFoundException knfEx)
        {
            return NotFound(knfEx.Message);
        }
        catch (UnauthorizedAccessException uaEx)
        {
            return Forbid(uaEx.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a classroom
    /// A classroom can only be deleted if it has no active enrolled students
    /// </summary>
    /// <param name="classId">The ID of the classroom to delete</param>
    /// <returns>No content on successful deletion</returns>
    /// <response code="200">Classroom successfully deleted</response>
    /// <response code="409">Cannot delete classroom because it has active students</response>
    /// <response code="400">Invalid request or deletion failed</response>
    /// <response code="401">User is not authenticated</response>
    [Authorize]
    [HttpDelete("delete/{classId}")]
    public async Task<IActionResult> DeleteClassroom(Guid classId)
    {
        try 
        {
            await _deleteUseCase.ExecuteRemoveClassroomAsync(classId);
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