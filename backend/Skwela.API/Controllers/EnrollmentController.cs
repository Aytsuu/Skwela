
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Skwela.Application.UseCases.Enrollments;

namespace Skwela.API.Controllers;

/// <summary>
/// Enrollment Controller
/// Handles student enrollment in classrooms, retrieval of student enrollments,
/// and management of enrollment status
/// </summary>
[ApiController]
[Route("api/enrollment")]
public class EnrollmentController : ControllerBase
{
    private readonly CreateEnrollmentUseCase _createUseCase;
    private readonly GetEnrollmentUseCase _getUseCase;
    private readonly UpdateEnrollmentUseCase _updateUseCase;

    /// <summary>
    /// Initializes the EnrollmentController with required use cases
    /// </summary>
    /// <param name="createUseCase">Use case for creating/enrolling in classrooms</param>
    /// <param name="getUseCase">Use case for retrieving student enrollments</param>
    /// <param name="updateUseCase">Use case for updating enrollment status</param>
    public EnrollmentController(CreateEnrollmentUseCase createUseCase, [FromServices]  GetEnrollmentUseCase getUseCase, UpdateEnrollmentUseCase updateUseCase)
    {
        _createUseCase = createUseCase;
        _getUseCase = getUseCase;
        _updateUseCase = updateUseCase;
    }

    /// <summary>
    /// Enrolls a student in a classroom
    /// If the student is already enrolled (but inactive), reactivates the enrollment
    /// </summary>
    /// <param name="request">CreateEnrollmentDto containing classId and userId</param>
    /// <returns>The enrollment record (newly created or reactivated)</returns>
    /// <response code="200">Student successfully enrolled</response>
    /// <response code="400">Invalid enrollment data or enrollment failed</response>
    /// <response code="401">User is not authenticated</response>
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> EnrollStudent(CreateEnrollmentDto request)
    {
        try
        {
            var enrollment = await _createUseCase.ExecuteAsync(request);
            return Ok(enrollment);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves all active enrollments for a specific student
    /// Returns detailed information about each classroom the student is enrolled in
    /// </summary>
    /// <param name="userId">The ID of the student</param>
    /// <returns>List of EnrollmentDto objects with classroom details</returns>
    /// <response code="200">Enrollments successfully retrieved</response>
    /// <response code="400">Invalid user ID or retrieval failed</response>
    /// <response code="401">User is not authenticated</response>
    [Authorize]
    [HttpGet("get/{userId}")]
    public async Task<IActionResult> GetStudentEnrollments(Guid userId)
    {
        try
        {
            var enrollments = await _getUseCase.ExecuteGetStudentEnrollmentAsync(userId);
            return Ok(enrollments);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates the enrollment status for a student in a classroom
    /// Toggles between 'active' and 'inactive' states
    /// </summary>
    /// <param name="classId">The ID of the classroom</param>
    /// <param name="userId">The ID of the student</param>
    /// <returns>The updated enrollment record with new status</returns>
    /// <response code="200">Enrollment status successfully updated</response>
    /// <response code="404">Enrollment record not found</response>
    /// <response code="400">Invalid request parameters or update failed</response>
    /// <response code="401">User is not authenticated</response>
    [Authorize]
    [HttpPatch("update/status/{classId}/{userId}")]
    public async Task<IActionResult> UpdateEnrollmentStatus(Guid classId, Guid userId)
    {
        try
        {
            var update = await _updateUseCase.ExecuteUpdateEnrollmentStatusAsync(classId, userId);
            return Ok(update);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}