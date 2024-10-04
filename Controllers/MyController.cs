using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using dotenv.net;

namespace SimpleWebAppReact.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MyController : ControllerBase
    {
        [HttpGet("message")] // Define the route for this action
        public IActionResult GetMessage()
        {
            return Ok(new { message = "Hello from ASP.NET Core!" });
        }

        // Send Twillio Verification Email
        [HttpGet("send-verification-email")]
        public async Task<IActionResult> SendVerificationEmail()
        {
            DotEnv.Load();

            string accountSid = Environment.GetEnvironmentVariable("TWILLIO_ACCOUNT_SID");

            Console.WriteLine(accountSid);

            string authToken = Environment.GetEnvironmentVariable("TWILLIO_AUTH_TOKEN");
            string testRecipient = Environment.GetEnvironmentVariable("TEST_RECIPIENT");
            string templateId = Environment.GetEnvironmentVariable("TWILLIO_TEMPLATE_ID");
            string testName = Environment.GetEnvironmentVariable("TEST_NAME");
            string serviceSid = Environment.GetEnvironmentVariable("TWILLIO_SERVICE_SID");

            TwilioClient.Init(accountSid, authToken);

            var verification = await VerificationResource.CreateAsync(
                channel: "email",
                to: testRecipient,
                channelConfiguration: new Dictionary<
                    string,
                    Object>() { { "template_id", templateId }, { "from", testRecipient }, { "from_name", testName } },
                pathServiceSid: serviceSid);

            Console.WriteLine(verification.Sid);
            if (verification.Status == "pending")
            {
                return Ok(new { message = "Verification email sent successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to send verification email" });
            }
        }
    }


}