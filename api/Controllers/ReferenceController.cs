using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ReferenceController : ControllerBase
{
    private readonly IReferenceService _referenceService;

    public ReferenceController(IReferenceService referenceService)
        => _referenceService = referenceService;

    private int? OptionalUserId => User.Identity?.IsAuthenticated == true
        ? int.Parse(User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)!)
        : null;
    private int UserId => OptionalUserId!.Value;

    [HttpGet("ability-scores")]
    public async Task<ActionResult<IEnumerable<AbilityScoreDto>>> GetAbilityScores(CancellationToken ct)
        => Ok(await _referenceService.GetAbilityScoresAsync(OptionalUserId, ct));

    [HttpPost("ability-scores")]
    [Authorize]
    public async Task<ActionResult<AbilityScoreDto>> CreateCustomAbilityScore([FromBody] CreateAbilityScoreRequest request, CancellationToken ct)
        => Ok(await _referenceService.CreateCustomAbilityScoreAsync(request, UserId, ct));

    [HttpGet("skills")]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetSkills(CancellationToken ct)
        => Ok(await _referenceService.GetSkillsAsync(ct));

    [HttpGet("damage-types")]
    public async Task<ActionResult<IEnumerable<ReferenceItemDto>>> GetDamageTypes(CancellationToken ct)
        => Ok(await _referenceService.GetDamageTypesAsync(ct));

    [HttpGet("conditions")]
    public async Task<ActionResult<IEnumerable<ReferenceItemDto>>> GetConditions(CancellationToken ct)
        => Ok(await _referenceService.GetConditionsAsync(ct));

    [HttpGet("magic-schools")]
    public async Task<ActionResult<IEnumerable<ReferenceItemDto>>> GetMagicSchools(CancellationToken ct)
        => Ok(await _referenceService.GetMagicSchoolsAsync(ct));

    [HttpGet("languages")]
    public async Task<ActionResult<IEnumerable<ReferenceItemDto>>> GetLanguages(CancellationToken ct)
        => Ok(await _referenceService.GetLanguagesAsync(ct));

    [HttpGet("alignments")]
    public async Task<ActionResult<IEnumerable<ReferenceItemDto>>> GetAlignments(CancellationToken ct)
        => Ok(await _referenceService.GetAlignmentsAsync(ct));

    [HttpGet("creature-types")]
    public async Task<ActionResult<IEnumerable<ReferenceItemDto>>> GetCreatureTypes(CancellationToken ct)
        => Ok(await _referenceService.GetCreatureTypesAsync(ct));

    [HttpGet("races")]
    public async Task<ActionResult<IEnumerable<RaceDto>>> GetRaces(CancellationToken ct)
        => Ok(await _referenceService.GetRacesAsync(OptionalUserId, ct));

    [HttpGet("races/{id}")]
    public async Task<ActionResult<RaceDto>> GetRaceById(int id, CancellationToken ct)
        => Ok(await _referenceService.GetRaceByIdAsync(id, OptionalUserId, ct));

    [HttpPost("races")]
    [Authorize]
    public async Task<ActionResult<RaceDto>> CreateCustomRace([FromBody] CreateRaceRequest request, CancellationToken ct)
        => Ok(await _referenceService.CreateCustomRaceAsync(request, UserId, ct));

    [HttpGet("classes")]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetClasses(CancellationToken ct)
        => Ok(await _referenceService.GetClassesAsync(OptionalUserId, ct));

    [HttpGet("classes/{id}")]
    public async Task<ActionResult<ClassDto>> GetClassById(int id, CancellationToken ct)
        => Ok(await _referenceService.GetClassByIdAsync(id, OptionalUserId, ct));

    [HttpPost("classes")]
    [Authorize]
    public async Task<ActionResult<ClassDto>> CreateCustomClass([FromBody] CreateClassRequest request, CancellationToken ct)
        => Ok(await _referenceService.CreateCustomClassAsync(request, UserId, ct));

    [HttpGet("items")]
    public async Task<ActionResult<IEnumerable<ItemSummaryDto>>> GetItems([FromQuery] ItemType? type, CancellationToken ct)
        => Ok(await _referenceService.GetItemsAsync(type, OptionalUserId, ct));

    [HttpGet("items/{id}")]
    public async Task<ActionResult<ItemDetailDto>> GetItemById(int id, CancellationToken ct)
        => Ok(await _referenceService.GetItemByIdAsync(id, OptionalUserId, ct));

    [HttpPost("items")]
    [Authorize]
    public async Task<ActionResult<ItemDetailDto>> CreateCustomItem([FromBody] CreateItemRequest request, CancellationToken ct)
        => Ok(await _referenceService.CreateCustomItemAsync(request, UserId, ct));

    [HttpGet("spells")]
    public async Task<ActionResult<IEnumerable<SpellSummaryDto>>> GetSpells([FromQuery] int? level, [FromQuery] int? schoolId, [FromQuery] int? classId, CancellationToken ct)
    {
        var spellService = HttpContext.RequestServices.GetRequiredService<ISpellService>();
        return Ok(await spellService.GetAllAsync(level, schoolId, classId, OptionalUserId, ct));
    }

    [HttpGet("spells/{id}")]
    public async Task<ActionResult<SpellDetailDto>> GetSpellById(int id, CancellationToken ct)
    {
        var spellService = HttpContext.RequestServices.GetRequiredService<ISpellService>();
        return Ok(await spellService.GetByIdAsync(id, OptionalUserId, ct));
    }

    [HttpPost("spells")]
    [Authorize]
    public async Task<ActionResult<SpellDetailDto>> CreateCustomSpell([FromBody] CreateSpellRequest request, CancellationToken ct)
    {
        var spellService = HttpContext.RequestServices.GetRequiredService<ISpellService>();
        return Ok(await spellService.CreateAsync(request, UserId, ct));
    }

    [HttpGet("monsters")]
    public async Task<ActionResult<IEnumerable<MonsterSummaryDto>>> GetMonsters([FromQuery] decimal? maxCr, [FromQuery] int? creatureTypeId, CancellationToken ct)
    {
        var monsterService = HttpContext.RequestServices.GetRequiredService<IMonsterService>();
        return Ok(await monsterService.GetAllAsync(maxCr, creatureTypeId, OptionalUserId, ct));
    }

    [HttpGet("monsters/{id}")]
    public async Task<ActionResult<MonsterDetailDto>> GetMonsterById(int id, CancellationToken ct)
    {
        var monsterService = HttpContext.RequestServices.GetRequiredService<IMonsterService>();
        return Ok(await monsterService.GetByIdAsync(id, OptionalUserId, ct));
    }

    [HttpPost("monsters")]
    [Authorize]
    public async Task<ActionResult<MonsterDetailDto>> CreateCustomMonster([FromBody] CreateMonsterRequest request, CancellationToken ct)
    {
        var monsterService = HttpContext.RequestServices.GetRequiredService<IMonsterService>();
        return Ok(await monsterService.CreateAsync(request, UserId, ct));
    }
}
