
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skwela.Application.UseCases.Classrooms;
using Skwela.Domain.Enums;

namespace Skwela.API.Controllers;

[ApiController]
[Route("api/classroom")]
public class ClassroomsController : ControllerBase
{
    private readonly CreateClassroomUseCase _createUseCase;
    private readonly GetClassroomUseCase _getUseCase;
    private readonly DeleteClassroomUseCase _deleteUseCase;

    public ClassroomsController(CreateClassroomUseCase createUseCase, GetClassroomUseCase getUseCase, DeleteClassroomUseCase deleteUseCase)
    {
        _createUseCase = createUseCase;
        _getUseCase = getUseCase;
        _deleteUseCase = deleteUseCase;
    }

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