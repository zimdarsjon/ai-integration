using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly AgentService _agentService;

    public AgentController(AgentService agentService)
    {
        _agentService = agentService;
    }

    [HttpPost("run")]
    public async Task<IActionResult> Run([FromBody] AgentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Message cannot be empty.");

        var systemPrompt = "You are a helpful assistant for a software development team.";

        var response = await _agentService.RunAsync(systemPrompt, request.Message);

        return Ok(new { response });
    }
}

// Request model
public record AgentRequest(string Message);