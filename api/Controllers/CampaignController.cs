using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CampaignController : ControllerBase
{
    private readonly ICampaignService _campaignService;

    public CampaignController(ICampaignService campaignService)
        => _campaignService = campaignService;

    private int UserId => int.Parse(User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CampaignSummaryDto>>> GetMyCampaigns(CancellationToken ct)
        => Ok(await _campaignService.GetUserCampaignsAsync(UserId, ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<CampaignDetailDto>> GetById(int id, CancellationToken ct)
        => Ok(await _campaignService.GetByIdAsync(id, UserId, ct));

    [HttpPost]
    public async Task<ActionResult<CampaignDetailDto>> Create([FromBody] CreateCampaignRequest request, CancellationToken ct)
    {
        var result = await _campaignService.CreateAsync(request, UserId, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CampaignDetailDto>> Update(int id, [FromBody] UpdateCampaignRequest request, CancellationToken ct)
        => Ok(await _campaignService.UpdateAsync(id, request, UserId, ct));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _campaignService.DeleteAsync(id, UserId, ct);
        return NoContent();
    }

    [HttpPost("{id}/members")]
    public async Task<IActionResult> AddMember(int id, [FromBody] AddMemberRequest request, CancellationToken ct)
    {
        await _campaignService.AddMemberAsync(id, request, UserId, ct);
        return NoContent();
    }

    [HttpDelete("{id}/members/{memberId}")]
    public async Task<IActionResult> RemoveMember(int id, int memberId, CancellationToken ct)
    {
        await _campaignService.RemoveMemberAsync(id, memberId, UserId, ct);
        return NoContent();
    }

    [HttpPut("{id}/members/{memberId}/character/{characterId}")]
    public async Task<IActionResult> AssignCharacter(int id, int memberId, int characterId, CancellationToken ct)
    {
        await _campaignService.AssignCharacterAsync(id, memberId, characterId, UserId, ct);
        return NoContent();
    }
}
