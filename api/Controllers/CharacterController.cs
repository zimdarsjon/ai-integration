using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _characterService;

    public CharacterController(ICharacterService characterService)
        => _characterService = characterService;

    private int UserId => int.Parse(User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterSummaryDto>>> GetMyCharacters(CancellationToken ct)
        => Ok(await _characterService.GetUserCharactersAsync(UserId, ct));

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<CharacterDetailDto>> GetById(int id, CancellationToken ct)
    {
        int requestingUser = User.Identity?.IsAuthenticated == true ? UserId : 0;
        return Ok(await _characterService.GetByIdAsync(id, requestingUser, ct));
    }

    [HttpPost]
    public async Task<ActionResult<CharacterDetailDto>> Create([FromBody] CreateCharacterRequest request, CancellationToken ct)
    {
        var result = await _characterService.CreateAsync(request, UserId, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CharacterDetailDto>> Update(int id, [FromBody] UpdateCharacterRequest request, CancellationToken ct)
        => Ok(await _characterService.UpdateAsync(id, request, UserId, ct));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _characterService.DeleteAsync(id, UserId, ct);
        return NoContent();
    }

    [HttpPost("{id}/damage")]
    public async Task<ActionResult<CharacterDetailDto>> ApplyDamage(int id, [FromBody] ApplyDamageRequest request, CancellationToken ct)
        => Ok(await _characterService.ApplyDamageAsync(id, request.Amount, UserId, ct));

    [HttpPost("{id}/heal")]
    public async Task<ActionResult<CharacterDetailDto>> Heal(int id, [FromBody] HealRequest request, CancellationToken ct)
        => Ok(await _characterService.HealAsync(id, request.Amount, UserId, ct));

    [HttpPut("{id}/spell-slots")]
    public async Task<IActionResult> UpdateSpellSlots(int id, [FromBody] UpdateSpellSlotsRequest request, CancellationToken ct)
    {
        await _characterService.UpdateSpellSlotsAsync(id, request, UserId, ct);
        return NoContent();
    }

    [HttpPost("{id}/spell-slots/recalculate")]
    public async Task<IActionResult> RecalculateSpellSlots(int id, CancellationToken ct)
    {
        await _characterService.RecalculateSpellSlotsAsync(id, UserId, ct);
        return NoContent();
    }

    [HttpPut("{id}/currency")]
    public async Task<IActionResult> UpdateCurrency(int id, [FromBody] UpdateCurrencyRequest request, CancellationToken ct)
    {
        await _characterService.UpdateCurrencyAsync(id, request, UserId, ct);
        return NoContent();
    }

    [HttpPost("{id}/inventory")]
    public async Task<IActionResult> AddInventoryItem(int id, [FromBody] AddInventoryItemRequest request, CancellationToken ct)
    {
        await _characterService.AddInventoryItemAsync(id, request, UserId, ct);
        return NoContent();
    }

    [HttpDelete("{id}/inventory/{inventoryItemId}")]
    public async Task<IActionResult> RemoveInventoryItem(int id, int inventoryItemId, CancellationToken ct)
    {
        await _characterService.RemoveInventoryItemAsync(id, inventoryItemId, UserId, ct);
        return NoContent();
    }

    [HttpPost("{id}/conditions")]
    public async Task<IActionResult> ApplyCondition(int id, [FromBody] ApplyConditionRequest request, CancellationToken ct)
    {
        await _characterService.ApplyConditionAsync(id, request, UserId, ct);
        return NoContent();
    }

    [HttpDelete("{id}/conditions/{conditionId}")]
    public async Task<IActionResult> RemoveCondition(int id, int conditionId, CancellationToken ct)
    {
        await _characterService.RemoveConditionAsync(id, conditionId, UserId, ct);
        return NoContent();
    }

    [HttpPost("{id}/spells/{spellId}")]
    public async Task<IActionResult> AddSpell(int id, int spellId, CancellationToken ct)
    {
        await _characterService.AddSpellAsync(id, spellId, UserId, ct);
        return NoContent();
    }

    [HttpPost("{id}/level-up")]
    public async Task<ActionResult<CharacterDetailDto>> LevelUp(int id, [FromBody] LevelUpRequest request, CancellationToken ct)
    {
        var result = await _characterService.LevelUpAsync(id, request, UserId, ct);
        return Ok(result);
    }

    [HttpPut("{id}/spells/{spellId}/prepare")]
    public async Task<IActionResult> PrepareSpell(int id, int spellId, [FromBody] PrepareSpellRequest request, CancellationToken ct)
    {
        await _characterService.PrepareSpellAsync(id, spellId, request.IsPrepared, UserId, ct);
        return NoContent();
    }

    [HttpDelete("{id}/spells/{spellId}")]
    public async Task<IActionResult> RemoveSpell(int id, int spellId, CancellationToken ct)
    {
        await _characterService.RemoveSpellAsync(id, spellId, UserId, ct);
        return NoContent();
    }
}
