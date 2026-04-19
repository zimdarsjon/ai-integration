using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/sharing")]
[Authorize]
public class SharingController : ControllerBase
{
    private readonly ISharingService _sharingService;

    public SharingController(ISharingService sharingService)
        => _sharingService = sharingService;

    private int UserId => int.Parse(User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)!);

    [HttpGet("{contentType}/{contentId}")]
    public async Task<ActionResult<IEnumerable<ContentShareDto>>> GetShares(CustomContentType contentType, int contentId, CancellationToken ct)
        => Ok(await _sharingService.GetSharesAsync(contentType, contentId, UserId, ct));

    [HttpPost("user")]
    public async Task<ActionResult<ContentShareDto>> ShareWithUser([FromBody] ShareWithUserRequest request, CancellationToken ct)
        => Ok(await _sharingService.ShareWithUserAsync(request, UserId, ct));

    [HttpPost("campaign")]
    public async Task<ActionResult<ContentShareDto>> ShareWithCampaign([FromBody] ShareWithCampaignRequest request, CancellationToken ct)
        => Ok(await _sharingService.ShareWithCampaignAsync(request, UserId, ct));

    [HttpDelete("{shareId}")]
    public async Task<IActionResult> RevokeShare(int shareId, CancellationToken ct)
    {
        await _sharingService.RevokeShareAsync(shareId, UserId, ct);
        return NoContent();
    }

    [HttpPut("{contentType}/{contentId}/visibility")]
    public async Task<IActionResult> SetPublic(CustomContentType contentType, int contentId, [FromBody] SetPublicRequest request, CancellationToken ct)
    {
        await _sharingService.SetPublicAsync(contentType, contentId, request.IsPublic, UserId, ct);
        return NoContent();
    }
}
