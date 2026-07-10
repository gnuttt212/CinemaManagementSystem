using Cinema.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IClaudeService _claudeService;

        // Controller chỉ phụ thuộc vào interface -> dễ mock khi unit test
        public ChatController(IClaudeService claudeService)
        {
            _claudeService = claudeService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return BadRequest(new { message = "Prompt cannot be empty" });
            }

            try 
            {
                var answer = await _claudeService.GetCompletionAsync(prompt);
                return Ok(new { answer });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi gọi AI: " + ex.Message });
            }
        }
    }
}
