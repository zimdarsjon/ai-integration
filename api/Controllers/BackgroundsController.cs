using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BackgroundsController : ControllerBase
{
    private readonly IBackgroundService _backgroundService;

    public BackgroundsController(IBackgroundService backgroundService)
        => _backgroundService = backgroundService;

    private int? UserId
    {
        get
        {
            var value = User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            return value != null ? int.Parse(value) : null;
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<BackgroundSummaryDto>>> GetAll(CancellationToken ct)
        => Ok(await _backgroundService.GetAllAsync(UserId, ct));

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<BackgroundDetailDto>> GetById(int id, CancellationToken ct)
        => Ok(await _backgroundService.GetByIdAsync(id, UserId, ct));

    [HttpPost]
    public async Task<ActionResult<BackgroundDetailDto>> Create([FromBody] CreateBackgroundRequest request, CancellationToken ct)
    {
        var result = await _backgroundService.CreateAsync(request, UserId!.Value, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BackgroundDetailDto>> Update(int id, [FromBody] UpdateBackgroundRequest request, CancellationToken ct)
        => Ok(await _backgroundService.UpdateAsync(id, request, UserId!.Value, ct));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _backgroundService.DeleteAsync(id, UserId!.Value, ct);
        return NoContent();
    }
}
