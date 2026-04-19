using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/campaigns/{campaignId}/[controller]")]
[Authorize]
public class EncounterController : ControllerBase
{
    private readonly IEncounterService _encounterService;

    public EncounterController(IEncounterService encounterService)
        => _encounterService = encounterService;

    private int UserId => int.Parse(User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EncounterSummaryDto>>> GetAll(int campaignId, CancellationToken ct)
        => Ok(await _encounterService.GetCampaignEncountersAsync(campaignId, UserId, ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<EncounterDetailDto>> GetById(int campaignId, int id, CancellationToken ct)
        => Ok(await _encounterService.GetByIdAsync(id, UserId, ct));

    [HttpPost]
    public async Task<ActionResult<EncounterDetailDto>> Create(int campaignId, [FromBody] CreateEncounterRequest request, CancellationToken ct)
    {
        var result = await _encounterService.CreateAsync(campaignId, request, UserId, ct);
        return CreatedAtAction(nameof(GetById), new { campaignId, id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int campaignId, int id, CancellationToken ct)
    {
        await _encounterService.DeleteAsync(id, UserId, ct);
        return NoContent();
    }

    [HttpPost("{id}/participants")]
    public async Task<ActionResult<EncounterDetailDto>> AddParticipant(int campaignId, int id, [FromBody] AddParticipantRequest request, CancellationToken ct)
        => Ok(await _encounterService.AddParticipantAsync(id, request, UserId, ct));

    [HttpDelete("{id}/participants/{participantId}")]
    public async Task<ActionResult<EncounterDetailDto>> RemoveParticipant(int campaignId, int id, int participantId, CancellationToken ct)
        => Ok(await _encounterService.RemoveParticipantAsync(id, participantId, UserId, ct));

    [HttpPost("{id}/participants/{participantId}/damage")]
    public async Task<ActionResult<EncounterDetailDto>> ApplyDamage(int campaignId, int id, int participantId, [FromBody] ApplyEncounterDamageRequest request, CancellationToken ct)
        => Ok(await _encounterService.ApplyDamageAsync(id, participantId, request, UserId, ct));

    [HttpPost("{id}/participants/{participantId}/conditions/{conditionId}")]
    public async Task<ActionResult<EncounterDetailDto>> ApplyCondition(int campaignId, int id, int participantId, int conditionId, CancellationToken ct)
        => Ok(await _encounterService.ApplyConditionAsync(id, participantId, conditionId, UserId, ct));

    [HttpDelete("{id}/participants/{participantId}/conditions/{conditionId}")]
    public async Task<ActionResult<EncounterDetailDto>> RemoveCondition(int campaignId, int id, int participantId, int conditionId, CancellationToken ct)
        => Ok(await _encounterService.RemoveConditionAsync(id, participantId, conditionId, UserId, ct));

    [HttpPost("{id}/advance-turn")]
    public async Task<ActionResult<EncounterDetailDto>> AdvanceTurn(int campaignId, int id, CancellationToken ct)
        => Ok(await _encounterService.AdvanceTurnAsync(id, UserId, ct));

    [HttpPost("{id}/next-round")]
    public async Task<ActionResult<EncounterDetailDto>> NextRound(int campaignId, int id, CancellationToken ct)
        => Ok(await _encounterService.NextRoundAsync(id, UserId, ct));
}
