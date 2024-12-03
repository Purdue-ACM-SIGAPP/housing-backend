namespace SimpleWebAppReact.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

    [HttpPost("get-token")]
    public async Task<IActionResult> GetToken([FromBody] LoginRequest request)
    {
        var client = _httpClientFactory.CreateClient("Okta");

        var body = new StringBuilder();
        body.Append($"grant_type=password");
        body.Append($"&username={request.Username}");
        body.Append($"&password={request.Password}");
        body.Append($"&scope=openid");

        var content = new StringContent(body.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await client.PostAsync("v1/token", content);

        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = await response.Content.ReadAsStringAsync();
            return Ok(tokenResponse);
        }

        return BadRequest("Failed to retrieve token");
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}