using Microsoft.EntityFrameworkCore;

public class CharacterService : ICharacterService
{
    private readonly AppDbContext _db;

    public CharacterService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<CharacterSummaryDto>> GetUserCharactersAsync(int userId, CancellationToken ct = default)
    {
        var characters = await _db.Characters
            .Where(c => c.UserId == userId)
            .Include(c => c.Race)
            .Include(c => c.Classes).ThenInclude(cc => cc.Class)
            .ToListAsync(ct);

        return characters.Select(c => new CharacterSummaryDto
        {
            Id = c.Id,
            Name = c.Name,
            RaceName = c.Race?.Name,
            Classes = string.Join("/", c.Classes.Select(cc => $"{cc.Class.Name} {cc.Level}")),
            TotalLevel = c.TotalLevel,
            CurrentHP = c.CurrentHP,
            MaxHP = c.MaxHP,
            IsPublic = c.IsPublic,
            PortraitUrl = c.PortraitUrl
        });
    }

    public async Task<CharacterDetailDto> GetByIdAsync(int id, int requestingUserId, CancellationToken ct = default)
    {
        var character = await LoadFullCharacterAsync(id, ct)
            ?? throw new NotFoundException($"Character {id} not found.");

        if (!character.IsPublic && character.UserId != requestingUserId)
            throw new UnauthorizedException("You do not have access to this character.");

        return await MapToDetailDtoAsync(character, ct);
    }

    public async Task<CharacterDetailDto> CreateAsync(CreateCharacterRequest request, int userId, CancellationToken ct = default)
    {
        var character = new Character
        {
            UserId = userId,
            Name = request.Name,
            RaceId = request.RaceId,
            SubraceId = request.SubraceId,
            AlignmentId = request.AlignmentId,
            BackgroundId = request.BackgroundId
        };

        _db.Characters.Add(character);
        await _db.SaveChangesAsync(ct);

        foreach (var cs in request.Classes)
        {
            _db.CharacterClasses.Add(new CharacterClass
            {
                CharacterId = character.Id,
                ClassId = cs.ClassId,
                SubclassId = cs.SubclassId,
                Level = cs.Level
            });
        }

        foreach (var (abilityScoreId, score) in request.AbilityScores)
        {
            _db.CharacterAbilityScores.Add(new CharacterAbilityScore
            {
                CharacterId = character.Id,
                AbilityScoreId = abilityScoreId,
                BaseScore = score
            });
        }

        foreach (var skillId in request.SkillIds)
            _db.CharacterSkillProficiencies.Add(new CharacterSkillProficiency
                { CharacterId = character.Id, SkillId = skillId, ProficiencyType = ProficiencyType.Proficient });

        foreach (var languageId in request.LanguageIds)
            _db.CharacterLanguages.Add(new CharacterLanguage { CharacterId = character.Id, LanguageId = languageId });

        // Apply class saving throw proficiencies automatically
        var classIds = request.Classes.Select(c => c.ClassId).ToList();
        var classSavingThrows = await _db.ClassSavingThrows
            .Where(st => classIds.Contains(st.ClassId))
            .Include(st => st.AbilityScore)
            .ToListAsync(ct);
        foreach (var st in classSavingThrows.DistinctBy(st => st.AbilityScoreId))
            _db.CharacterSavingThrows.Add(new CharacterSavingThrow
                { CharacterId = character.Id, AbilityScoreId = st.AbilityScoreId, IsProficient = true });

        _db.CharacterCurrencies.Add(new CharacterCurrency { CharacterId = character.Id });

        character.TotalLevel = request.Classes.Sum(c => c.Level);
        character.MaxHP = await CalculateStartingHpAsync(request, ct);
        character.CurrentHP = character.MaxHP;

        await _db.SaveChangesAsync(ct);
        await RecalculateSpellSlotsInternalAsync(character.Id, ct);

        return await GetByIdAsync(character.Id, userId, ct);
    }

    public async Task<CharacterDetailDto> UpdateAsync(int id, UpdateCharacterRequest request, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        if (request.Name != null) character.Name = request.Name;
        if (request.AlignmentId.HasValue) character.AlignmentId = request.AlignmentId;
        if (request.BackgroundId.HasValue) character.BackgroundId = request.BackgroundId;
        if (request.Age.HasValue) character.Age = request.Age.Value;
        if (request.Height != null) character.Height = request.Height;
        if (request.Weight != null) character.Weight = request.Weight;
        if (request.Eyes != null) character.Eyes = request.Eyes;
        if (request.Skin != null) character.Skin = request.Skin;
        if (request.Hair != null) character.Hair = request.Hair;
        if (request.Backstory != null) character.Backstory = request.Backstory;
        if (request.PersonalityTraits != null) character.PersonalityTraits = request.PersonalityTraits;
        if (request.Ideals != null) character.Ideals = request.Ideals;
        if (request.Bonds != null) character.Bonds = request.Bonds;
        if (request.Flaws != null) character.Flaws = request.Flaws;
        if (request.MaxHP.HasValue) character.MaxHP = request.MaxHP.Value;
        if (request.AC.HasValue) character.AC = request.AC.Value;
        if (request.Speed.HasValue) character.Speed = request.Speed.Value;
        if (request.Inspiration.HasValue) character.Inspiration = request.Inspiration.Value;
        if (request.IsPublic.HasValue) character.IsPublic = request.IsPublic.Value;
        if (request.PortraitUrl != null) character.PortraitUrl = request.PortraitUrl;

        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, userId, ct);
    }

    public async Task DeleteAsync(int id, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        _db.Characters.Remove(character);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<CharacterDetailDto> ApplyDamageAsync(int id, int damage, int requestingUserId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");

        int tempRemaining = Math.Max(0, character.TempHP - damage);
        int damageToHP = damage - (character.TempHP - tempRemaining);
        character.TempHP = tempRemaining;
        character.CurrentHP = Math.Max(0, character.CurrentHP - damageToHP);

        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, requestingUserId, ct);
    }

    public async Task<CharacterDetailDto> HealAsync(int id, int amount, int requestingUserId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");

        character.CurrentHP = Math.Min(character.MaxHP, character.CurrentHP + amount);
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, requestingUserId, ct);
    }

    public async Task UpdateSpellSlotsAsync(int id, UpdateSpellSlotsRequest request, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        var slot = await _db.CharacterSpellSlots
            .FirstOrDefaultAsync(s => s.CharacterId == id && s.SlotLevel == request.SlotLevel, ct);

        if (slot == null) throw new NotFoundException($"No spell slot level {request.SlotLevel} for this character.");

        slot.UsedSlots = Math.Clamp(request.UsedSlots, 0, slot.TotalSlots);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RecalculateSpellSlotsAsync(int id, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        await RecalculateSpellSlotsInternalAsync(id, ct);
    }

    public async Task UpdateCurrencyAsync(int id, UpdateCurrencyRequest request, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        var currency = await _db.CharacterCurrencies.FirstOrDefaultAsync(c => c.CharacterId == id, ct)
            ?? new CharacterCurrency { CharacterId = id };

        currency.CP = request.CP;
        currency.SP = request.SP;
        currency.EP = request.EP;
        currency.GP = request.GP;
        currency.PP = request.PP;

        if (currency.Id == 0) _db.CharacterCurrencies.Add(currency);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AddInventoryItemAsync(int id, AddInventoryItemRequest request, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        var item = await _db.Items.FindAsync([request.ItemId], ct)
            ?? throw new NotFoundException($"Item {request.ItemId} not found.");

        _db.CharacterInventory.Add(new CharacterInventoryItem
        {
            CharacterId = id,
            ItemId = request.ItemId,
            Quantity = request.Quantity,
            Notes = request.Notes
        });
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveInventoryItemAsync(int id, int inventoryItemId, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        var inv = await _db.CharacterInventory.FindAsync([inventoryItemId], ct)
            ?? throw new NotFoundException($"Inventory item not found.");
        if (inv.CharacterId != id) throw new ValidationException("Item does not belong to this character.");

        _db.CharacterInventory.Remove(inv);
        await _db.SaveChangesAsync(ct);
    }

    public async Task ApplyConditionAsync(int id, ApplyConditionRequest request, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        var alreadyHas = await _db.CharacterConditions.AnyAsync(c => c.CharacterId == id && c.ConditionId == request.ConditionId, ct);
        if (!alreadyHas)
        {
            _db.CharacterConditions.Add(new CharacterCondition
            {
                CharacterId = id,
                ConditionId = request.ConditionId,
                Notes = request.Notes
            });
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task RemoveConditionAsync(int id, int conditionId, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        var condition = await _db.CharacterConditions
            .FirstOrDefaultAsync(c => c.CharacterId == id && c.ConditionId == conditionId, ct);
        if (condition != null)
        {
            _db.CharacterConditions.Remove(condition);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task AddSpellAsync(int id, int spellId, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        var alreadyKnows = await _db.CharacterSpells.AnyAsync(s => s.CharacterId == id && s.SpellId == spellId, ct);
        if (!alreadyKnows)
        {
            _db.CharacterSpells.Add(new CharacterSpell { CharacterId = id, SpellId = spellId });
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task RemoveSpellAsync(int id, int spellId, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        var spell = await _db.CharacterSpells.FirstOrDefaultAsync(s => s.CharacterId == id && s.SpellId == spellId, ct);
        if (spell != null)
        {
            _db.CharacterSpells.Remove(spell);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<CharacterDetailDto> LevelUpAsync(int id, LevelUpRequest request, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        var characterClass = await _db.CharacterClasses
            .Include(cc => cc.Class)
            .FirstOrDefaultAsync(cc => cc.CharacterId == id && cc.ClassId == request.ClassId, ct)
            ?? throw new NotFoundException("Class not found on this character.");

        var newLevel = characterClass.Level + 1;
        characterClass.Level = newLevel;

        // Grant class features for the new level
        var newFeatures = await _db.ClassFeatures
            .Where(f => f.ClassId == request.ClassId && f.Level == newLevel)
            .ToListAsync(ct);
        foreach (var feat in newFeatures)
        {
            _db.CharacterFeatures.Add(new CharacterFeature
            {
                CharacterId = id,
                Source = FeatureSource.Class,
                Name = feat.Name,
                Description = feat.Description,
                Level = feat.Level
            });
        }

        // Apply subclass if provided and character doesn't already have one for this class
        if (request.SubclassId.HasValue && characterClass.SubclassId == null)
        {
            var subclass = await _db.Subclasses
                .Include(s => s.Features)
                .FirstOrDefaultAsync(s => s.Id == request.SubclassId.Value && s.ClassId == request.ClassId, ct)
                ?? throw new NotFoundException("Subclass not found for this class.");
            characterClass.SubclassId = subclass.Id;
            foreach (var sf in subclass.Features.Where(f => f.Level <= newLevel))
            {
                _db.CharacterFeatures.Add(new CharacterFeature
                {
                    CharacterId = id,
                    Source = FeatureSource.Subclass,
                    Name = sf.Name,
                    Description = sf.Description,
                    Level = sf.Level
                });
            }
        }
        else if (characterClass.SubclassId.HasValue)
        {
            // Grant subclass features for the new level if the character already has a subclass
            var subclassFeatures = await _db.SubclassFeatures
                .Where(sf => sf.SubclassId == characterClass.SubclassId.Value && sf.Level == newLevel)
                .ToListAsync(ct);
            foreach (var sf in subclassFeatures)
            {
                _db.CharacterFeatures.Add(new CharacterFeature
                {
                    CharacterId = id,
                    Source = FeatureSource.Subclass,
                    Name = sf.Name,
                    Description = sf.Description,
                    Level = sf.Level
                });
            }
        }

        // Update HP
        character.MaxHP += request.HpIncrease;
        character.CurrentHP += request.HpIncrease;

        // Update hit dice total for this class
        var hitDice = await _db.CharacterHitDice
            .FirstOrDefaultAsync(hd => hd.CharacterId == id && hd.ClassId == request.ClassId, ct);
        if (hitDice != null) hitDice.Total++;

        // Add newly chosen spells (for Known spellcasters)
        if (request.SpellIds.Count > 0)
        {
            var existingSpellIds = (await _db.CharacterSpells
                .Where(cs => cs.CharacterId == id)
                .Select(cs => cs.SpellId)
                .ToListAsync(ct)).ToHashSet();

            var spellsToAdd = await _db.Spells
                .Where(s => request.SpellIds.Contains(s.Id) && !existingSpellIds.Contains(s.Id))
                .ToListAsync(ct);

            foreach (var spell in spellsToAdd)
                _db.CharacterSpells.Add(new CharacterSpell { CharacterId = id, SpellId = spell.Id, IsPrepared = false });
        }

        await _db.SaveChangesAsync(ct);
        await RecalculateSpellSlotsInternalAsync(id, ct);

        var full = await LoadFullCharacterAsync(id, ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        return await MapToDetailDtoAsync(full, ct);
    }

    public async Task PrepareSpellAsync(int id, int spellId, bool isPrepared, int userId, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Character {id} not found.");
        if (character.UserId != userId) throw new UnauthorizedException("Not your character.");

        var entry = await _db.CharacterSpells.FirstOrDefaultAsync(s => s.CharacterId == id && s.SpellId == spellId, ct);
        if (entry == null)
        {
            // Auto-add for prepared casters browsing their full class list
            entry = new CharacterSpell { CharacterId = id, SpellId = spellId, IsPrepared = isPrepared };
            _db.CharacterSpells.Add(entry);
        }
        else
        {
            entry.IsPrepared = isPrepared;
        }
        await _db.SaveChangesAsync(ct);
    }

    private async Task<int> CalculateStartingHpAsync(CreateCharacterRequest request, CancellationToken ct)
    {
        var conId = await _db.AbilityScores
            .Where(a => a.Abbreviation == "CON")
            .Select(a => a.Id)
            .FirstOrDefaultAsync(ct);

        request.AbilityScores.TryGetValue(conId, out var conBase);

        var racialConBonus = 0;
        if (request.RaceId.HasValue)
            racialConBonus += await _db.RaceAbilityBonuses
                .Where(b => b.RaceId == request.RaceId && b.AbilityScoreId == conId)
                .Select(b => b.Bonus).FirstOrDefaultAsync(ct);
        if (request.SubraceId.HasValue)
            racialConBonus += await _db.SubraceAbilityBonuses
                .Where(b => b.SubraceId == request.SubraceId && b.AbilityScoreId == conId)
                .Select(b => b.Bonus).FirstOrDefaultAsync(ct);

        var conMod = (conBase + racialConBonus - 10) / 2;

        var classIds = request.Classes.Select(c => c.ClassId).ToList();
        var hitDies = await _db.Classes
            .Where(c => classIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.HitDie, ct);

        var maxHp = 0;
        foreach (var cls in request.Classes)
        {
            var hitDie = hitDies.GetValueOrDefault(cls.ClassId, 8);
            var avg = hitDie / 2 + 1;
            maxHp += hitDie + conMod;                        // level 1: max die
            maxHp += (cls.Level - 1) * (avg + conMod);      // subsequent levels: average
        }

        return Math.Max(1, maxHp);
    }

    private async Task RecalculateSpellSlotsInternalAsync(int characterId, CancellationToken ct)
    {
        var classes = await _db.CharacterClasses
            .Include(cc => cc.Class)
            .Where(cc => cc.CharacterId == characterId)
            .ToListAsync(ct);

        var casterInfo = classes.Select(cc => (cc.Class.CasterType, cc.Level));
        var slots = SpellSlotCalculator.CalculateSlots(casterInfo);

        var existing = await _db.CharacterSpellSlots
            .Where(s => s.CharacterId == characterId)
            .ToListAsync(ct);

        _db.CharacterSpellSlots.RemoveRange(existing);

        for (int i = 0; i < 9; i++)
        {
            if (slots[i] > 0)
            {
                _db.CharacterSpellSlots.Add(new CharacterSpellSlot
                {
                    CharacterId = characterId,
                    SlotLevel = i + 1,
                    TotalSlots = slots[i],
                    UsedSlots = 0
                });
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    private async Task<Character?> LoadFullCharacterAsync(int id, CancellationToken ct) =>
        await _db.Characters
            .Include(c => c.Race).ThenInclude(r => r!.AbilityBonuses).ThenInclude(b => b.AbilityScore)
            .Include(c => c.Subrace).ThenInclude(s => s!.AbilityBonuses).ThenInclude(b => b.AbilityScore)
            .Include(c => c.Alignment)
            .Include(c => c.Background)
            .Include(c => c.Classes).ThenInclude(cc => cc.Class).ThenInclude(cls => cls.SpellcastingAbilityScore)
            .Include(c => c.Classes).ThenInclude(cc => cc.Subclass)
            .Include(c => c.AbilityScores).ThenInclude(a => a.AbilityScore)
            .Include(c => c.SkillProficiencies).ThenInclude(s => s.Skill).ThenInclude(s => s.AbilityScore)
            .Include(c => c.SavingThrows).ThenInclude(s => s.AbilityScore)
            .Include(c => c.SpellSlots)
            .Include(c => c.Spells).ThenInclude(s => s.Spell).ThenInclude(s => s.School)
            .Include(c => c.Conditions).ThenInclude(cond => cond.Condition)
            .Include(c => c.Inventory).ThenInclude(i => i.Item)
            .Include(c => c.Currency)
            .Include(c => c.Languages).ThenInclude(l => l.Language)
            .Include(c => c.Features)
            .Include(c => c.HitDice).ThenInclude(hd => hd.Class)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    private async Task<CharacterDetailDto> MapToDetailDtoAsync(Character c, CancellationToken ct)
    {
        var profBonus = SpellSlotCalculator.ProficiencyBonusForLevel(c.TotalLevel);

        // Build ability score totals including racial bonuses
        var raceBonuses = c.Race?.AbilityBonuses.ToDictionary(b => b.AbilityScoreId, b => b.Bonus) ?? [];
        var subraceBonuses = c.Subrace?.AbilityBonuses.ToDictionary(b => b.AbilityScoreId, b => b.Bonus) ?? [];

        var abilityScores = c.AbilityScores.Select(a =>
        {
            raceBonuses.TryGetValue(a.AbilityScoreId, out var rb);
            subraceBonuses.TryGetValue(a.AbilityScoreId, out var srb);
            int total = a.BaseScore + rb + srb;
            return new AbilityScoreSheetDto
            {
                AbilityScoreId = a.AbilityScoreId,
                Name = a.AbilityScore.Name,
                Abbreviation = a.AbilityScore.Abbreviation,
                BaseScore = a.BaseScore,
                RacialBonus = rb + srb,
                TotalScore = total,
                Modifier = SpellSlotCalculator.AbilityModifier(total)
            };
        }).ToList();

        var abilityMap = abilityScores.ToDictionary(a => a.AbilityScoreId);

        var allSkills = await _db.Skills.Include(s => s.AbilityScore).OrderBy(s => s.Name).ToListAsync(ct);
        var skillProfMap = c.SkillProficiencies.ToDictionary(sp => sp.SkillId, sp => sp.ProficiencyType);
        var skills = allSkills.Select(s =>
        {
            var abilityMod = abilityMap.TryGetValue(s.AbilityScoreId, out var a) ? a.Modifier : 0;
            skillProfMap.TryGetValue(s.Id, out var profType);
            int bonus = profType switch
            {
                ProficiencyType.Expertise => abilityMod + profBonus * 2,
                ProficiencyType.Proficient => abilityMod + profBonus,
                ProficiencyType.Half => abilityMod + profBonus / 2,
                _ => abilityMod
            };
            return new SkillSheetDto
            {
                SkillId = s.Id,
                Name = s.Name,
                AbilityAbbreviation = s.AbilityScore.Abbreviation,
                ProficiencyType = profType,
                Bonus = bonus
            };
        }).ToList();

        var allAbilityScores = await _db.AbilityScores.OrderBy(a => a.Id).ToListAsync(ct);
        var stProfMap = c.SavingThrows.ToDictionary(st => st.AbilityScoreId, st => st.IsProficient);
        var savingThrows = allAbilityScores.Select(a =>
        {
            var abilityMod = abilityMap.TryGetValue(a.Id, out var score) ? score.Modifier : 0;
            var isProficient = stProfMap.GetValueOrDefault(a.Id);
            return new SavingThrowSheetDto
            {
                AbilityScoreId = a.Id,
                Name = a.Name,
                Abbreviation = a.Abbreviation,
                IsProficient = isProficient,
                Bonus = isProficient ? abilityMod + profBonus : abilityMod
            };
        }).ToList();

        var strScore = abilityScores.FirstOrDefault(a => a.Abbreviation == "STR")?.TotalScore ?? 10;
        var carryingCapacity = strScore * 15m;
        var currentWeight = c.Inventory.Sum(i => i.Item.WeightLbs * i.Quantity);

        return new CharacterDetailDto
        {
            Id = c.Id,
            Name = c.Name,
            RaceId = c.RaceId,
            RaceName = c.Race?.Name,
            SubraceId = c.SubraceId,
            SubraceName = c.Subrace?.Name,
            AlignmentId = c.AlignmentId,
            AlignmentName = c.Alignment?.Name,
            BackgroundId = c.BackgroundId,
            BackgroundName = c.Background?.Name,
            Age = c.Age,
            Height = c.Height,
            Weight = c.Weight,
            Eyes = c.Eyes,
            Skin = c.Skin,
            Hair = c.Hair,
            Backstory = c.Backstory,
            PersonalityTraits = c.PersonalityTraits,
            Ideals = c.Ideals,
            Bonds = c.Bonds,
            Flaws = c.Flaws,
            TotalLevel = c.TotalLevel,
            ProficiencyBonus = profBonus,
            MaxHP = c.MaxHP,
            CurrentHP = c.CurrentHP,
            TempHP = c.TempHP,
            AC = c.AC,
            Speed = c.Speed,
            Inspiration = c.Inspiration,
            DeathSaveSuccesses = c.DeathSaveSuccesses,
            DeathSaveFailures = c.DeathSaveFailures,
            ExperiencePoints = c.ExperiencePoints,
            IsPublic = c.IsPublic,
            PortraitUrl = c.PortraitUrl,
            CarryingCapacityLbs = carryingCapacity,
            CurrentWeightLbs = currentWeight,
            Classes = c.Classes.Select(cc => new CharacterClassDto
            {
                ClassId = cc.ClassId,
                ClassName = cc.Class.Name,
                SubclassId = cc.SubclassId,
                SubclassName = cc.Subclass?.Name,
                Level = cc.Level,
                HitDie = cc.Class.HitDie,
                CasterType = cc.Class.CasterType,
                SpellcastingAbility = cc.Class.SpellcastingAbilityScore?.Abbreviation
            }).ToList(),
            AbilityScores = abilityScores,
            Skills = skills,
            SavingThrows = savingThrows,
            SpellSlots = c.SpellSlots.OrderBy(s => s.SlotLevel).Select(s => new SpellSlotDto
            {
                SlotLevel = s.SlotLevel,
                TotalSlots = s.TotalSlots,
                UsedSlots = s.UsedSlots
            }).ToList(),
            Spells = c.Spells.Select(s => new CharacterSpellDto
            {
                SpellId = s.SpellId,
                Name = s.Spell.Name,
                Level = s.Spell.Level,
                School = s.Spell.School.Name,
                IsPrepared = s.IsPrepared,
                IsConcentration = s.Spell.IsConcentration,
                IsRitual = s.Spell.IsRitual,
                CastingTime = s.Spell.CastingTime,
                Range = s.Spell.Range,
                Duration = s.Spell.Duration
            }).ToList(),
            Conditions = c.Conditions.Select(cond => new CharacterConditionDto
            {
                Id = cond.Id,
                ConditionId = cond.ConditionId,
                Name = cond.Condition.Name,
                Description = cond.Condition.Description,
                AppliedAt = cond.AppliedAt,
                Notes = cond.Notes
            }).ToList(),
            Inventory = c.Inventory.Select(i => new InventoryItemDto
            {
                Id = i.Id,
                ItemId = i.ItemId,
                Name = i.Item.Name,
                ItemType = i.Item.ItemType,
                Quantity = i.Quantity,
                WeightLbs = i.Item.WeightLbs,
                CostCp = i.Item.CostCp,
                IsEquipped = i.IsEquipped,
                IsAttuned = i.IsAttuned,
                EquipmentSlot = i.EquipmentSlot,
                Notes = i.Notes
            }).ToList(),
            Currency = c.Currency == null ? null : new CurrencyDto
            {
                CP = c.Currency.CP,
                SP = c.Currency.SP,
                EP = c.Currency.EP,
                GP = c.Currency.GP,
                PP = c.Currency.PP
            },
            Languages = c.Languages.Select(l => l.Language.Name).ToList(),
            Features = c.Features.Select(f => new CharacterFeatureDto
            {
                Id = f.Id,
                Source = f.Source,
                Name = f.Name,
                Description = f.Description,
                Level = f.Level,
                UsesMax = f.UsesMax,
                UsesRemaining = f.UsesRemaining
            }).ToList(),
            HitDice = c.HitDice.Select(hd => new HitDiceDto
            {
                ClassName = hd.Class.Name,
                HitDie = hd.Class.HitDie,
                Total = hd.Total,
                Remaining = hd.Remaining
            }).ToList()
        };
    }
}
