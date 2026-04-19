using Microsoft.EntityFrameworkCore;

public class SpellService : ISpellService
{
    private readonly AppDbContext _db;
    private readonly ISharingService _sharing;

    public SpellService(AppDbContext db, ISharingService sharing)
    {
        _db = db;
        _sharing = sharing;
    }

    public async Task<IEnumerable<SpellSummaryDto>> GetAllAsync(int? level, int? schoolId, int? classId, int? requestingUserId = null, CancellationToken ct = default)
    {
        var accessible = requestingUserId.HasValue
            ? await _sharing.GetAccessibleContentIdsAsync(CustomContentType.Spell, requestingUserId.Value, ct)
            : [];

        var query = _db.Spells
            .Where(s => !s.IsCustom || s.IsPublic || s.CreatedByUserId == requestingUserId || accessible.Contains(s.Id))
            .Include(s => s.School)
            .AsQueryable();

        if (level.HasValue) query = query.Where(s => s.Level == level.Value);
        if (schoolId.HasValue) query = query.Where(s => s.SchoolId == schoolId.Value);
        if (classId.HasValue) query = query.Where(s => s.SpellClasses.Any(sc => sc.ClassId == classId.Value));

        return await query.Select(s => new SpellSummaryDto
        {
            Id = s.Id, Name = s.Name, Level = s.Level, School = s.School.Name,
            CastingTime = s.CastingTime, IsConcentration = s.IsConcentration,
            IsRitual = s.IsRitual, IsCustom = s.IsCustom
        }).ToListAsync(ct);
    }

    public async Task<SpellDetailDto> GetByIdAsync(int id, int? requestingUserId = null, CancellationToken ct = default)
    {
        var accessible = requestingUserId.HasValue
            ? await _sharing.GetAccessibleContentIdsAsync(CustomContentType.Spell, requestingUserId.Value, ct)
            : [];
        var spell = await _db.Spells
            .Where(s => s.Id == id && (!s.IsCustom || s.IsPublic || s.CreatedByUserId == requestingUserId || accessible.Contains(s.Id)))
            .Include(s => s.School)
            .Include(s => s.SpellClasses).ThenInclude(sc => sc.Class)
            .Include(s => s.Damages).ThenInclude(d => d.DamageType)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException($"Spell {id} not found or not accessible.");
        return MapToDto(spell);
    }

    public async Task<SpellDetailDto> CreateAsync(CreateSpellRequest request, int userId, CancellationToken ct = default)
    {
        var spell = new Spell
        {
            Name = request.Name, Level = request.Level, SchoolId = request.SchoolId,
            CastingTime = request.CastingTime, Range = request.Range, Components = request.Components,
            MaterialComponent = request.MaterialComponent, Duration = request.Duration,
            IsConcentration = request.IsConcentration, IsRitual = request.IsRitual,
            Description = request.Description, AtHigherLevels = request.AtHigherLevels,
            IsCustom = true, CreatedByUserId = userId
        };
        _db.Spells.Add(spell);
        await _db.SaveChangesAsync(ct);

        foreach (var classId in request.ClassIds)
            _db.SpellClasses.Add(new SpellClass { SpellId = spell.Id, ClassId = classId });

        await _db.SaveChangesAsync(ct);

        return await GetByIdAsync(spell.Id, userId, ct);
    }

    public async Task<SpellDetailDto> UpdateAsync(int id, CreateSpellRequest request, int userId, CancellationToken ct = default)
    {
        var spell = await _db.Spells.FindAsync([id], ct)
            ?? throw new NotFoundException($"Spell {id} not found.");
        if (spell.CreatedByUserId != userId) throw new UnauthorizedException("Not your spell.");

        spell.Name = request.Name; spell.Level = request.Level; spell.SchoolId = request.SchoolId;
        spell.CastingTime = request.CastingTime; spell.Range = request.Range; spell.Components = request.Components;
        spell.MaterialComponent = request.MaterialComponent; spell.Duration = request.Duration;
        spell.IsConcentration = request.IsConcentration; spell.IsRitual = request.IsRitual;
        spell.Description = request.Description; spell.AtHigherLevels = request.AtHigherLevels;

        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, userId, ct);
    }

    public async Task DeleteAsync(int id, int userId, CancellationToken ct = default)
    {
        var spell = await _db.Spells.FindAsync([id], ct)
            ?? throw new NotFoundException($"Spell {id} not found.");
        if (spell.CreatedByUserId != userId) throw new UnauthorizedException("Not your spell.");
        _db.Spells.Remove(spell);
        await _db.SaveChangesAsync(ct);
    }

    private static SpellDetailDto MapToDto(Spell s) => new()
    {
        Id = s.Id, Name = s.Name, Level = s.Level, SchoolId = s.SchoolId, School = s.School.Name,
        CastingTime = s.CastingTime, Range = s.Range, Components = s.Components,
        MaterialComponent = s.MaterialComponent, Duration = s.Duration,
        IsConcentration = s.IsConcentration, IsRitual = s.IsRitual,
        Description = s.Description, AtHigherLevels = s.AtHigherLevels, IsCustom = s.IsCustom,
        Classes = s.SpellClasses.Select(sc => sc.Class.Name).ToList(),
        Damages = s.Damages.Select(d => new SpellDamageDto { DamageType = d.DamageType.Name, DamageDice = d.DamageDice }).ToList()
    };
}
