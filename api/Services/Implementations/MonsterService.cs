using Microsoft.EntityFrameworkCore;

public class MonsterService : IMonsterService
{
    private readonly AppDbContext _db;
    private readonly ISharingService _sharing;

    public MonsterService(AppDbContext db, ISharingService sharing)
    {
        _db = db;
        _sharing = sharing;
    }

    public async Task<IEnumerable<MonsterSummaryDto>> GetAllAsync(decimal? maxCr, int? creatureTypeId, int? requestingUserId = null, CancellationToken ct = default)
    {
        var accessible = requestingUserId.HasValue
            ? await _sharing.GetAccessibleContentIdsAsync(CustomContentType.Monster, requestingUserId.Value, ct)
            : [];

        var query = _db.Monsters
            .Where(m => !m.IsCustom || m.IsPublic || m.CreatedByUserId == requestingUserId || accessible.Contains(m.Id))
            .Include(m => m.CreatureType)
            .Include(m => m.Size)
            .AsQueryable();

        if (maxCr.HasValue) query = query.Where(m => m.ChallengeRating <= maxCr.Value);
        if (creatureTypeId.HasValue) query = query.Where(m => m.CreatureTypeId == creatureTypeId.Value);

        return await query.Select(m => new MonsterSummaryDto
        {
            Id = m.Id, Name = m.Name, ChallengeRating = m.ChallengeRating, XP = m.XP,
            CreatureType = m.CreatureType.Name, Size = m.Size.Name, AC = m.AC, MaxHP = m.MaxHP, IsCustom = m.IsCustom
        }).ToListAsync(ct);
    }

    public async Task<MonsterDetailDto> GetByIdAsync(int id, int? requestingUserId = null, CancellationToken ct = default)
    {
        var accessible = requestingUserId.HasValue
            ? await _sharing.GetAccessibleContentIdsAsync(CustomContentType.Monster, requestingUserId.Value, ct)
            : [];

        var monster = await LoadFullMonsterAsync(id, requestingUserId, accessible, ct)
            ?? throw new NotFoundException($"Monster {id} not found or not accessible.");
        return MapToDto(monster);
    }

    public async Task<MonsterDetailDto> CreateAsync(CreateMonsterRequest request, int userId, CancellationToken ct = default)
    {
        var monster = new Monster
        {
            Name = request.Name, CreatureTypeId = request.CreatureTypeId, SizeId = request.SizeId,
            AlignmentId = request.AlignmentId, AC = request.AC, ACSource = request.ACSource,
            MaxHP = request.MaxHP, HitDice = request.HitDice, WalkSpeed = request.WalkSpeed,
            ChallengeRating = request.ChallengeRating, XP = request.XP, ProficiencyBonus = request.ProficiencyBonus,
            IsCustom = true, CreatedByUserId = userId
        };
        _db.Monsters.Add(monster);
        await _db.SaveChangesAsync(ct);

        foreach (var (abilityScoreId, score) in request.AbilityScores)
            _db.MonsterAbilityScores.Add(new MonsterAbilityScore { MonsterId = monster.Id, AbilityScoreId = abilityScoreId, Score = score });

        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(monster.Id, userId, ct);
    }

    public async Task<MonsterDetailDto> UpdateAsync(int id, CreateMonsterRequest request, int userId, CancellationToken ct = default)
    {
        var monster = await _db.Monsters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Monster {id} not found.");
        if (monster.CreatedByUserId != userId) throw new UnauthorizedException("Not your monster.");

        monster.Name = request.Name; monster.AC = request.AC; monster.MaxHP = request.MaxHP;
        monster.ChallengeRating = request.ChallengeRating; monster.XP = request.XP;
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, userId, ct);
    }

    public async Task DeleteAsync(int id, int userId, CancellationToken ct = default)
    {
        var monster = await _db.Monsters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Monster {id} not found.");
        if (monster.CreatedByUserId != userId) throw new UnauthorizedException("Not your monster.");
        _db.Monsters.Remove(monster);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<Monster?> LoadFullMonsterAsync(int id, int? requestingUserId, HashSet<int> accessible, CancellationToken ct) =>
        await _db.Monsters
            .Where(m => m.Id == id && (!m.IsCustom || m.IsPublic || m.CreatedByUserId == requestingUserId || accessible.Contains(m.Id)))
            .Include(m => m.CreatureType).Include(m => m.Size).Include(m => m.Alignment)
            .Include(m => m.AbilityScores).ThenInclude(a => a.AbilityScore)
            .Include(m => m.SavingThrows).ThenInclude(s => s.AbilityScore)
            .Include(m => m.Skills).ThenInclude(s => s.Skill)
            .Include(m => m.DamageModifiers).ThenInclude(d => d.DamageType)
            .Include(m => m.ConditionImmunities).ThenInclude(c => c.Condition)
            .Include(m => m.Senses)
            .Include(m => m.Languages).ThenInclude(l => l.Language)
            .Include(m => m.Traits)
            .Include(m => m.Actions).ThenInclude(a => a.HitDamageType)
            .Include(m => m.Actions).ThenInclude(a => a.SaveAbilityScore)
            .FirstOrDefaultAsync(ct);

    private static MonsterDetailDto MapToDto(Monster m) => new()
    {
        Id = m.Id, Name = m.Name, CreatureType = m.CreatureType.Name, Size = m.Size.Name,
        Alignment = m.Alignment?.Name, AC = m.AC, ACSource = m.ACSource, MaxHP = m.MaxHP,
        HitDice = m.HitDice, WalkSpeed = m.WalkSpeed, SwimSpeed = m.SwimSpeed, FlySpeed = m.FlySpeed,
        ClimbSpeed = m.ClimbSpeed, BurrowSpeed = m.BurrowSpeed, ChallengeRating = m.ChallengeRating,
        XP = m.XP, ProficiencyBonus = m.ProficiencyBonus, IsLegendary = m.IsLegendary,
        LegendaryActionCount = m.LegendaryActionCount, IsCustom = m.IsCustom,
        AbilityScores = m.AbilityScores.Select(a => new MonsterAbilityScoreDto { Abbreviation = a.AbilityScore.Abbreviation, Score = a.Score }).ToList(),
        SavingThrows = m.SavingThrows.Select(s => s.AbilityScore.Abbreviation).ToList(),
        Skills = m.Skills.Select(s => s.Skill.Name).ToList(),
        DamageImmunities = m.DamageModifiers.Where(d => d.ModifierType == DamageModifierType.Immunity).Select(d => d.DamageType.Name).ToList(),
        DamageResistances = m.DamageModifiers.Where(d => d.ModifierType == DamageModifierType.Resistance).Select(d => d.DamageType.Name).ToList(),
        DamageVulnerabilities = m.DamageModifiers.Where(d => d.ModifierType == DamageModifierType.Vulnerability).Select(d => d.DamageType.Name).ToList(),
        ConditionImmunities = m.ConditionImmunities.Select(c => c.Condition.Name).ToList(),
        Languages = m.Languages.Select(l => l.Language.Name).ToList(),
        Senses = m.Senses.Select(s => new MonsterSenseDto { SenseType = s.SenseType.ToString(), RangeFt = s.RangeFt }).ToList(),
        Traits = m.Traits.Select(t => new TraitDto { Name = t.Name, Description = t.Description }).ToList(),
        Actions = m.Actions.Select(a => new MonsterActionDto
        {
            Name = a.Name, Description = a.Description, ActionType = a.ActionType, AttackBonus = a.AttackBonus,
            ReachOrRange = a.ReachOrRange, HitDamageDice = a.HitDamageDice, HitDamageType = a.HitDamageType?.Name,
            SaveDC = a.SaveDC, SaveAbility = a.SaveAbilityScore?.Abbreviation, LegendaryCost = a.LegendaryCost
        }).ToList()
    };
}
