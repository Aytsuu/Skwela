
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skwela.Application.Interfaces;
using Skwela.Application.UseCases.Classrooms;

namespace Skwela.API.Controllers;

[ApiController]
[Route("api/classroom")]
public class ClassroomsController : ControllerBase
{
    private readonly CreateClassroomUseCase _createUseCase;
    private readonly GetClassroomUseCase _getUseCase;

    public ClassroomsController(CreateClassroomUseCase createUseCase, GetClassroomUseCase getUseCase)
    {
        _createUseCase = createUseCase;
        _getUseCase = getUseCase;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateClassroom(CreateClassroomDto request)
    {
        try
        {
            var classId = await _createUseCase.ExecuteAsync(request);

            return Ok(new { classId });
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
}