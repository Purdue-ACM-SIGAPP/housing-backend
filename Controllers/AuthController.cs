using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleWebAppReact.Controllers;

/* A work in progress controller that wraps basic Okta provided endpoints, such as /token. will be only used for development */
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }
    
    
    [HttpGet("whoami")]
    [Authorize] // Ensure the user is authenticated
    public IActionResult GetWho()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(new
        {
            Message = "User info retrieved successfully.",
            Claims = claims
        });
    }
    
    [HttpGet("get-user-id")]
    [Authorize]
    public IActionResult GetUserId()
    {
        // Retrieve the `NameIdentifier` claim value
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(new { Message = "User ID not found in claims." });
        }

        return Ok(new
        {
            Message = "User ID retrieved successfully.",
            UserId = userId
        });
    }
    [Authorize]
    [HttpGet("debug-claims")]
    public IActionResult DebugClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(claims);
    }
    [HttpGet("roles")]
    [Authorize]
    public IActionResult GetUserRoles()
    {
        // Extract roles from the current user's identity
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role) // Use ClaimTypes.Role to fetch role claims
            .Select(c => c.Value)
            .ToList();

        if (roles.Count == 0)
        {
            return NotFound(new
            {
                Message = "No roles found for the user."
            });
        }

        return Ok(new
        {
            Message = "User roles retrieved successfully.",
            Roles = roles
        });
    }

    [HttpGet("email")]
    [Authorize]
    public IActionResult GetUserEmail()
    {
        var email = User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?
            .Value;

        if (email == null)
        {
            return NotFound(new
            {
                Message = "No email found for the user."
            });
        }

        return Ok(new
        {
            Message = "User email retrieved successfully.",
            Email = email
        });
    }
    
    [Authorize(Roles = "test")]
    [HttpGet("gatekeep-test")]
    public IActionResult GatekeepTest()
    {
        return Ok();
    }

}