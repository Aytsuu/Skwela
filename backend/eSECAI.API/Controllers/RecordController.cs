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
[Route("api/record")]

public class RecordController : ControllerBase
{

    public RecordController()
    {
        
    }

    public class CreateRecordRequest
    {
        public List<IFormFile>? files { get; set; }
        public Guid assId { get; set; }
    }

    // create record
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult> CreateRecord([FromForm] CreateRecordRequest request)
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

            var fileReqs = new List<RecordFileRequest>();
            if (request.files != null)
            {
                foreach (var file in request.files)
                {
                    fileReqs.Add(new RecordFileRequest(
                        file.OpenReadStream(),
                        file.FileName,
                        file.ContentType
                    ));
                }
            }

            return Ok(new { message = "Record processing started." });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, stack = e.StackTrace });
        }
    }

    // get all records for user
    
    
    // get record by id

    
    // update record

    
    // delete record

    
    // get all records for classroom
    

    
    // get all records for assignment
    
}