using Microsoft.EntityFrameworkCore;

public class ReferenceService : IReferenceService
{
    private readonly AppDbContext _db;
    private readonly ISharingService _sharing;

    public ReferenceService(AppDbContext db, ISharingService sharing)
    {
        _db = db;
        _sharing = sharing;
    }

    public async Task<IEnumerable<AbilityScoreDto>> GetAbilityScoresAsync(int? requestingUserId = null, CancellationToken ct = default)
    {
        var accessible = requestingUserId.HasValue
            ? await _sharing.GetAccessibleContentIdsAsync(CustomContentType.AbilityScore, requestingUserId.Value, ct)
            : [];

        return await _db.AbilityScores
            .Where(a => !a.IsCustom || a.IsPublic || a.CreatedByUserId == requestingUserId || accessible.Contains(a.Id))
            .Select(a => new AbilityScoreDto
            {
                Id = a.Id, Name = a.Name, Abbreviation = a.Abbreviation, Description = a.Description, IsCustom = a.IsCustom
            }).ToListAsync(ct);
    }

    public async Task<AbilityScoreDto> CreateCustomAbilityScoreAsync(CreateAbilityScoreRequest request, int userId, CancellationToken ct = default)
    {
        var score = new AbilityScore { Name = request.Name, Abbreviation = request.Abbreviation, Description = request.Description, IsCustom = true, CreatedByUserId = userId };
        _db.AbilityScores.Add(score);
        await _db.SaveChangesAsync(ct);
        return new AbilityScoreDto { Id = score.Id, Name = score.Name, Abbreviation = score.Abbreviation, Description = score.Description, IsCustom = true };
    }

    public async Task<IEnumerable<SkillDto>> GetSkillsAsync(CancellationToken ct = default) =>
        await _db.Skills.Include(s => s.AbilityScore).Select(s => new SkillDto
        {
            Id = s.Id, Name = s.Name, AbilityScoreId = s.AbilityScoreId, AbilityAbbreviation = s.AbilityScore.Abbreviation
        }).ToListAsync(ct);

    public async Task<IEnumerable<ReferenceItemDto>> GetDamageTypesAsync(CancellationToken ct = default) =>
        await _db.DamageTypes.Select(d => new ReferenceItemDto { Id = d.Id, Name = d.Name, Description = d.Description }).ToListAsync(ct);

    public async Task<IEnumerable<ReferenceItemDto>> GetConditionsAsync(CancellationToken ct = default) =>
        await _db.Conditions.Select(c => new ReferenceItemDto { Id = c.Id, Name = c.Name, Description = c.Description }).ToListAsync(ct);

    public async Task<IEnumerable<ReferenceItemDto>> GetMagicSchoolsAsync(CancellationToken ct = default) =>
        await _db.MagicSchools.Select(m => new ReferenceItemDto { Id = m.Id, Name = m.Name, Description = m.Description }).ToListAsync(ct);

    public async Task<IEnumerable<ReferenceItemDto>> GetLanguagesAsync(CancellationToken ct = default) =>
        await _db.Languages.Select(l => new ReferenceItemDto { Id = l.Id, Name = l.Name }).ToListAsync(ct);

    public async Task<IEnumerable<ReferenceItemDto>> GetAlignmentsAsync(CancellationToken ct = default) =>
        await _db.Alignments.Select(a => new ReferenceItemDto { Id = a.Id, Name = a.Name, Description = a.Description }).ToListAsync(ct);

    public async Task<IEnumerable<ReferenceItemDto>> GetCreatureTypesAsync(CancellationToken ct = default) =>
        await _db.CreatureTypes.Select(c => new ReferenceItemDto { Id = c.Id, Name = c.Name }).ToListAsync(ct);

    public async Task<IEnumerable<RaceDto>> GetRacesAsync(int? requestingUserId = null, CancellationToken ct = default)
    {
        var accessible = requestingUserId.HasValue
            ? await _sharing.GetAccessibleContentIdsAsync(CustomContentType.Race, requestingUserId.Value, ct)
            : [];

        var races = await _db.Races
            .Where(r => !r.IsCustom || r.IsPublic || r.CreatedByUserId == requestingUserId || accessible.Contains(r.Id))
            .Include(r => r.Size)
            .Include(r => r.AbilityBonuses).ThenInclude(b => b.AbilityScore)
            .Include(r => r.Traits)
            .Include(r => r.Languages).ThenInclude(l => l.Language)
            .Include(r => r.Subraces).ThenInclude(s => s.AbilityBonuses).ThenInclude(b => b.AbilityScore)
            .Include(r => r.Subraces).ThenInclude(s => s.Traits)
            .AsSplitQuery()
            .ToListAsync(ct);
        return races.Select(MapRaceToDto);
    }

    public async Task<RaceDto> GetRaceByIdAsync(int id, int? requestingUserId = null, CancellationToken ct = default)
    {
        var accessible = requestingUserId.HasValue
            ? await _sharing.GetAccessibleContentIdsAsync(CustomContentType.Race, requestingUserId.Value, ct)
            : [];

        var race = await _db.Races
            .Where(r => r.Id == id && (!r.IsCustom || r.IsPublic || r.CreatedByUserId == requestingUserId || accessible.Contains(r.Id)))
            .Include(r => r.Size)
            .Include(r => r.AbilityBonuses).ThenInclude(b => b.AbilityScore)
            .Include(r => r.Traits)
            .Include(r => r.Languages).ThenInclude(l => l.Language)
            .Include(r => r.Subraces).ThenInclude(s => s.AbilityBonuses).ThenInclude(b => b.AbilityScore)
            .Include(r => r.Subraces).ThenInclude(s => s.Traits)
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException($"Race {id} not found or not accessible.");
        return MapRaceToDto(race);
    }

    public async Task<RaceDto> CreateCustomRaceAsync(CreateRaceRequest request, int userId, CancellationToken ct = default)
    {
        var race = new Race { Name = request.Name, Description = request.Description, Speed = request.Speed, SizeId = request.SizeId, IsCustom = true, CreatedByUserId = userId };
        _db.Races.Add(race);
        await _db.SaveChangesAsync(ct);

        foreach (var bonus in request.AbilityBonuses)
            _db.RaceAbilityBonuses.Add(new RaceAbilityBonus { RaceId = race.Id, AbilityScoreId = bonus.AbilityScoreId, Bonus = bonus.Bonus });
        foreach (var trait in request.Traits)
            _db.RaceTraits.Add(new RaceTrait { RaceId = race.Id, Name = trait.Name, Description = trait.Description });
        foreach (var langId in request.LanguageIds)
            _db.RaceLanguages.Add(new RaceLanguage { RaceId = race.Id, LanguageId = langId });

        await _db.SaveChangesAsync(ct);
        return await GetRaceByIdAsync(race.Id, userId, ct);
    }

    public async Task<IEnumerable<ClassDto>> GetClassesAsync(int? requestingUserId = null, CancellationToken ct = default)
    {
        var accessible = requestingUserId.HasValue
            ? await _sharing.GetAccessibleContentIdsAsync(CustomContentType.Class, requestingUserId.Value, ct)
            : [];

        var classes = await _db.Classes
            .Where(c => !c.IsCustom || c.IsPublic || c.CreatedByUserId == requestingUserId || accessible.Contains(c.Id))
            .Include(c => c.SpellcastingAbilityScore)
            .Include(c => c.SavingThrows).ThenInclude(st => st.AbilityScore)
            .Include(c => c.Features)
            .Include(c => c.Subclasses).ThenInclude(sc => sc.Features)
            .Include(c => c.SpellSlotProgressions)
            .Include(c => c.SpellLimitProgressions)
            .Include(c => c.SkillChoices).ThenInclude(sc => sc.Skill).ThenInclude(s => s.AbilityScore)
            .AsSplitQuery()
            .ToListAsync(ct);
        return classes.Select(MapClassToDto);
    }

    public async Task<ClassDto> GetClassByIdAsync(int id, int? requestingUserId = null, CancellationToken ct = default)
    {
        var accessible = requestingUserId.HasValue
            ? await _sharing.GetAccessibleContentIdsAsync(CustomContentType.Class, requestingUserId.Value, ct)
            : [];

        var dndClass = await _db.Classes
            .Where(c => c.Id == id && (!c.IsCustom || c.IsPublic || c.CreatedByUserId == requestingUserId || accessible.Contains(c.Id)))
            .Include(c => c.SpellcastingAbilityScore)
            .Include(c => c.SavingThrows).ThenInclude(st => st.AbilityScore)
            .Include(c => c.Features)
            .Include(c => c.Subclasses).ThenInclude(sc => sc.Features)
            .Include(c => c.SpellSlotProgressions)
            .Include(c => c.SpellLimitProgressions)
            .Include(c => c.SkillChoices).ThenInclude(sc => sc.Skill).ThenInclude(s => s.AbilityScore)
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException($"Class {id} not found or not accessible.");
        return MapClassToDto(dndClass);
    }

    public async Task<ClassDto> CreateCustomClassAsync(CreateClassRequest request, int userId, CancellationToken ct = default)
    {
        var dndClass = new DndClass
        {
            Name = request.Name, Description = request.Description, HitDie = request.HitDie,
            SpellcastingAbilityScoreId = request.SpellcastingAbilityScoreId, CasterType = request.CasterType,
            IsCustom = true, CreatedByUserId = userId
        };
        _db.Classes.Add(dndClass);
        await _db.SaveChangesAsync(ct);

        foreach (var abilityId in request.SavingThrowAbilityIds)
            _db.ClassSavingThrows.Add(new ClassSavingThrow { ClassId = dndClass.Id, AbilityScoreId = abilityId });

        await _db.SaveChangesAsync(ct);
        return await GetClassByIdAsync(dndClass.Id, userId, ct);
    }

    public async Task<IEnumerable<ItemSummaryDto>> GetItemsAsync(ItemType? type, int? requestingUserId = null, CancellationToken ct = default)
    {
        var accessible = requestingUserId.HasValue
            ? await _sharing.GetAccessibleContentIdsAsync(CustomContentType.Item, requestingUserId.Value, ct)
            : [];

        var query = _db.Items
            .Where(i => !i.IsCustom || i.IsPublic || i.CreatedByUserId == requestingUserId || accessible.Contains(i.Id))
            .AsQueryable();
        if (type.HasValue) query = query.Where(i => i.ItemType == type.Value);
        return await query.Select(i => new ItemSummaryDto
        {
            Id = i.Id, Name = i.Name, ItemType = i.ItemType, WeightLbs = i.WeightLbs, CostCp = i.CostCp, IsCustom = i.IsCustom
        }).ToListAsync(ct);
    }

    public async Task<ItemDetailDto> GetItemByIdAsync(int id, int? requestingUserId = null, CancellationToken ct = default)
    {
        var accessible = requestingUserId.HasValue
            ? await _sharing.GetAccessibleContentIdsAsync(CustomContentType.Item, requestingUserId.Value, ct)
            : [];

        var item = await _db.Items
            .Where(i => i.Id == id && (!i.IsCustom || i.IsPublic || i.CreatedByUserId == requestingUserId || accessible.Contains(i.Id)))
            .Include(i => i.Weapon).ThenInclude(w => w!.DamageType)
            .Include(i => i.Weapon).ThenInclude(w => w!.PropertyAssignments).ThenInclude(pa => pa.WeaponProperty)
            .Include(i => i.Armor)
            .Include(i => i.MagicItem)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException($"Item {id} not found or not accessible.");
        return MapItemToDto(item);
    }

    public async Task<ItemDetailDto> CreateCustomItemAsync(CreateItemRequest request, int userId, CancellationToken ct = default)
    {
        var item = new Item { Name = request.Name, Description = request.Description, WeightLbs = request.WeightLbs, CostCp = request.CostCp, ItemType = request.ItemType, IsCustom = true, CreatedByUserId = userId };
        _db.Items.Add(item);
        await _db.SaveChangesAsync(ct);

        if (request.Weapon != null)
        {
            var weapon = new Weapon { ItemId = item.Id, DamageDice = request.Weapon.DamageDice, DamageTypeId = request.Weapon.DamageTypeId, IsRanged = request.Weapon.IsRanged, IsMartial = request.Weapon.IsMartial, NormalRangeFt = request.Weapon.NormalRangeFt, LongRangeFt = request.Weapon.LongRangeFt };
            _db.Weapons.Add(weapon);
            await _db.SaveChangesAsync(ct);
            foreach (var propId in request.Weapon.PropertyIds)
                _db.WeaponPropertyAssignments.Add(new WeaponPropertyAssignment { WeaponId = weapon.Id, WeaponPropertyId = propId });
        }

        if (request.Armor != null)
            _db.Armors.Add(new ArmorItem { ItemId = item.Id, ArmorType = request.Armor.ArmorType, BaseAC = request.Armor.BaseAC, MaxDexBonus = request.Armor.MaxDexBonus, MinStrength = request.Armor.MinStrength, StealthDisadvantage = request.Armor.StealthDisadvantage });

        await _db.SaveChangesAsync(ct);
        return await GetItemByIdAsync(item.Id, userId, ct);
    }

    private async Task<List<DndClass>> LoadClassesQueryAsync(CancellationToken ct) =>
        await _db.Classes
            .Include(c => c.SpellcastingAbilityScore)
            .Include(c => c.SavingThrows).ThenInclude(st => st.AbilityScore)
            .Include(c => c.Features)
            .Include(c => c.Subclasses).ThenInclude(sc => sc.Features)
            .Include(c => c.SpellSlotProgressions)
            .Include(c => c.SpellLimitProgressions)
            .Include(c => c.SkillChoices).ThenInclude(sc => sc.Skill).ThenInclude(s => s.AbilityScore)
            .AsSplitQuery()
            .ToListAsync(ct);

    private static RaceDto MapRaceToDto(Race r) => new()
    {
        Id = r.Id, Name = r.Name, Description = r.Description, Speed = r.Speed, Size = r.Size.Name, IsCustom = r.IsCustom,
        AbilityBonuses = r.AbilityBonuses.Select(b => new AbilityBonusDto { AbilityScoreName = b.AbilityScore.Name, Abbreviation = b.AbilityScore.Abbreviation, Bonus = b.Bonus }).ToList(),
        Traits = r.Traits.Select(t => new TraitDto { Name = t.Name, Description = t.Description }).ToList(),
        Languages = r.Languages.Select(l => l.Language.Name).ToList(),
        Subraces = r.Subraces.Select(sr => new SubraceDto
        {
            Id = sr.Id, Name = sr.Name, Description = sr.Description,
            AbilityBonuses = sr.AbilityBonuses.Select(b => new AbilityBonusDto { AbilityScoreName = b.AbilityScore.Name, Abbreviation = b.AbilityScore.Abbreviation, Bonus = b.Bonus }).ToList(),
            Traits = sr.Traits.Select(t => new TraitDto { Name = t.Name, Description = t.Description }).ToList()
        }).ToList()
    };

    private static ClassDto MapClassToDto(DndClass c) => new()
    {
        Id = c.Id, Name = c.Name, Description = c.Description, HitDie = c.HitDie,
        SpellcastingAbility = c.SpellcastingAbilityScore?.Abbreviation, CasterType = c.CasterType, IsCustom = c.IsCustom,
        SavingThrows = c.SavingThrows.Select(st => st.AbilityScore.Abbreviation).ToList(),
        Features = c.Features.OrderBy(f => f.Level).Select(f => new ClassFeatureDto { Name = f.Name, Description = f.Description, Level = f.Level }).ToList(),
        Subclasses = c.Subclasses.Select(sc => new SubclassDto
        {
            Id = sc.Id, Name = sc.Name, Description = sc.Description, ChoiceLevel = sc.ChoiceLevel,
            Features = sc.Features.OrderBy(f => f.Level).Select(f => new ClassFeatureDto { Name = f.Name, Description = f.Description, Level = f.Level }).ToList()
        }).ToList(),
        SpellSlotProgression = c.SpellSlotProgressions.OrderBy(sp => sp.Level).Select(sp => new SpellSlotRowDto
        {
            Level = sp.Level, Slots = [sp.Slot1, sp.Slot2, sp.Slot3, sp.Slot4, sp.Slot5, sp.Slot6, sp.Slot7, sp.Slot8, sp.Slot9]
        }).ToList(),
        SpellLimitProgression = c.SpellLimitProgressions.OrderBy(sl => sl.Level).Select(sl => new SpellLimitRowDto
        {
            Level = sl.Level, CantripsKnown = sl.CantripsKnown, SpellsKnown = sl.SpellsKnown, StartingSpells = sl.StartingSpells
        }).ToList(),
        SkillChoices = c.SkillChoices.Any() ? new ClassSkillChoiceDto
        {
            NumberOfChoices = c.SkillChoices.First().NumberOfChoices,
            AvailableSkills = c.SkillChoices.Select(sc => new SkillDto
            {
                Id = sc.Skill.Id,
                Name = sc.Skill.Name,
                AbilityScoreId = sc.Skill.AbilityScoreId,
                AbilityAbbreviation = sc.Skill.AbilityScore.Abbreviation
            }).ToList()
        } : null
    };

    private static ItemDetailDto MapItemToDto(Item i) => new()
    {
        Id = i.Id, Name = i.Name, Description = i.Description, ItemType = i.ItemType, WeightLbs = i.WeightLbs, CostCp = i.CostCp, IsCustom = i.IsCustom,
        Weapon = i.Weapon == null ? null : new WeaponDetailDto { DamageDice = i.Weapon.DamageDice, DamageType = i.Weapon.DamageType.Name, NormalRangeFt = i.Weapon.NormalRangeFt, LongRangeFt = i.Weapon.LongRangeFt, IsRanged = i.Weapon.IsRanged, IsMartial = i.Weapon.IsMartial, MagicBonus = i.Weapon.MagicBonus, Properties = i.Weapon.PropertyAssignments.Select(pa => pa.WeaponProperty.Name).ToList() },
        Armor = i.Armor == null ? null : new ArmorDetailDto { ArmorType = i.Armor.ArmorType, BaseAC = i.Armor.BaseAC, MaxDexBonus = i.Armor.MaxDexBonus, MinStrength = i.Armor.MinStrength, StealthDisadvantage = i.Armor.StealthDisadvantage, MagicBonus = i.Armor.MagicBonus },
        MagicItem = i.MagicItem == null ? null : new MagicItemDetailDto { Rarity = i.MagicItem.Rarity, RequiresAttunement = i.MagicItem.RequiresAttunement, AttunementRequirements = i.MagicItem.AttunementRequirements, Properties = i.MagicItem.Properties }
    };
}
