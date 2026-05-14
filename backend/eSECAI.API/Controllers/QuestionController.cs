using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using esecai.Application.UseCases.Questions;
using esecai.Application.DTOs;
using System.Security.Claims;

namespace esecai.API.Controllers;

[ApiController]
[Route("api/question")]
public class QuestionController : ControllerBase
{
	private readonly GetQuestionUseCase _getUseCase;

	public QuestionController(GetQuestionUseCase getUseCase)
	{
		_getUseCase = getUseCase;
	}

	[Authorize]
	[HttpGet("get")]
	public async Task<IActionResult> GetAssessmentQuestions([FromQuery] Guid assId)
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

			var questions = await _getUseCase.ExecuteGetAssessmentQuestionsAsync(assId);
			return Ok(questions);
		}
		catch (Exception e)
		{
			return BadRequest(new { message = e.Message, stack = e.StackTrace });
		}
	}
}