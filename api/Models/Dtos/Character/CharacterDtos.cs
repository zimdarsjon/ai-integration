public class CharacterSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? RaceName { get; set; }
    public string Classes { get; set; } = string.Empty;
    public int TotalLevel { get; set; }
    public int CurrentHP { get; set; }
    public int MaxHP { get; set; }
    public bool IsPublic { get; set; }
    public string? PortraitUrl { get; set; }
}

public class CharacterDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? RaceId { get; set; }
    public string? RaceName { get; set; }
    public int? SubraceId { get; set; }
    public string? SubraceName { get; set; }
    public int? AlignmentId { get; set; }
    public string? AlignmentName { get; set; }
    public int? BackgroundId { get; set; }
    public string? BackgroundName { get; set; }
    public int Age { get; set; }
    public string Height { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
    public string Eyes { get; set; } = string.Empty;
    public string Skin { get; set; } = string.Empty;
    public string Hair { get; set; } = string.Empty;
    public string Backstory { get; set; } = string.Empty;
    public string PersonalityTraits { get; set; } = string.Empty;
    public string Ideals { get; set; } = string.Empty;
    public string Bonds { get; set; } = string.Empty;
    public string Flaws { get; set; } = string.Empty;
    public int TotalLevel { get; set; }
    public int ProficiencyBonus { get; set; }
    public int MaxHP { get; set; }
    public int CurrentHP { get; set; }
    public int TempHP { get; set; }
    public int AC { get; set; }
    public int Speed { get; set; }
    public bool Inspiration { get; set; }
    public int DeathSaveSuccesses { get; set; }
    public int DeathSaveFailures { get; set; }
    public int ExperiencePoints { get; set; }
    public bool IsPublic { get; set; }
    public string? PortraitUrl { get; set; }
    public decimal CarryingCapacityLbs { get; set; }
    public decimal CurrentWeightLbs { get; set; }
    public List<CharacterClassDto> Classes { get; set; } = [];
    public List<AbilityScoreSheetDto> AbilityScores { get; set; } = [];
    public List<SkillSheetDto> Skills { get; set; } = [];
    public List<SavingThrowSheetDto> SavingThrows { get; set; } = [];
    public List<SpellSlotDto> SpellSlots { get; set; } = [];
    public List<CharacterSpellDto> Spells { get; set; } = [];
    public List<CharacterConditionDto> Conditions { get; set; } = [];
    public List<InventoryItemDto> Inventory { get; set; } = [];
    public CurrencyDto? Currency { get; set; }
    public List<string> Languages { get; set; } = [];
    public List<CharacterFeatureDto> Features { get; set; } = [];
    public List<HitDiceDto> HitDice { get; set; } = [];
}

public class CharacterClassDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int? SubclassId { get; set; }
    public string? SubclassName { get; set; }
    public int Level { get; set; }
    public int HitDie { get; set; }
    public CasterType CasterType { get; set; }
    public string? SpellcastingAbility { get; set; }
}

public class PrepareSpellRequest
{
    public bool IsPrepared { get; set; }
}

public class LevelUpRequest
{
    public int ClassId { get; set; }
    public int HpIncrease { get; set; }
    public int? SubclassId { get; set; }
    public List<int> SpellIds { get; set; } = [];
}

public class AbilityScoreSheetDto
{
    public int AbilityScoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public int BaseScore { get; set; }
    public int RacialBonus { get; set; }
    public int TotalScore { get; set; }
    public int Modifier { get; set; }
}

public class SkillSheetDto
{
    public int SkillId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AbilityAbbreviation { get; set; } = string.Empty;
    public ProficiencyType ProficiencyType { get; set; }
    public int Bonus { get; set; }
}

public class SavingThrowSheetDto
{
    public int AbilityScoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public bool IsProficient { get; set; }
    public int Bonus { get; set; }
}

public class SpellSlotDto
{
    public int SlotLevel { get; set; }
    public int TotalSlots { get; set; }
    public int UsedSlots { get; set; }
    public int AvailableSlots => TotalSlots - UsedSlots;
}

public class CharacterSpellDto
{
    public int SpellId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public string School { get; set; } = string.Empty;
    public bool IsPrepared { get; set; }
    public bool IsConcentration { get; set; }
    public bool IsRitual { get; set; }
    public string CastingTime { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
}

public class CharacterConditionDto
{
    public int Id { get; set; }
    public int ConditionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class InventoryItemDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public int Quantity { get; set; }
    public decimal WeightLbs { get; set; }
    public int CostCp { get; set; }
    public bool IsEquipped { get; set; }
    public bool IsAttuned { get; set; }
    public EquipmentSlot EquipmentSlot { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CurrencyDto
{
    public int CP { get; set; }
    public int SP { get; set; }
    public int EP { get; set; }
    public int GP { get; set; }
    public int PP { get; set; }
}

public class CharacterFeatureDto
{
    public int Id { get; set; }
    public FeatureSource Source { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Level { get; set; }
    public int? UsesMax { get; set; }
    public int? UsesRemaining { get; set; }
}

public class HitDiceDto
{
    public string ClassName { get; set; } = string.Empty;
    public int HitDie { get; set; }
    public int Total { get; set; }
    public int Remaining { get; set; }
}

public class CreateCharacterRequest
{
    public string Name { get; set; } = string.Empty;
    public int? RaceId { get; set; }
    public int? SubraceId { get; set; }
    public int? AlignmentId { get; set; }
    public int? BackgroundId { get; set; }
    public List<CharacterClassSelection> Classes { get; set; } = [];
    public Dictionary<int, int> AbilityScores { get; set; } = [];
    public List<int> SkillIds { get; set; } = [];
    public List<int> LanguageIds { get; set; } = [];
}

public class CharacterClassSelection
{
    public int ClassId { get; set; }
    public int? SubclassId { get; set; }
    public int Level { get; set; }
}

public class UpdateCharacterRequest
{
    public string? Name { get; set; }
    public int? AlignmentId { get; set; }
    public int? BackgroundId { get; set; }
    public int? Age { get; set; }
    public string? Height { get; set; }
    public string? Weight { get; set; }
    public string? Eyes { get; set; }
    public string? Skin { get; set; }
    public string? Hair { get; set; }
    public string? Backstory { get; set; }
    public string? PersonalityTraits { get; set; }
    public string? Ideals { get; set; }
    public string? Bonds { get; set; }
    public string? Flaws { get; set; }
    public int? MaxHP { get; set; }
    public int? AC { get; set; }
    public int? Speed { get; set; }
    public bool? Inspiration { get; set; }
    public bool? IsPublic { get; set; }
    public string? PortraitUrl { get; set; }
}

public class UpdateSpellSlotsRequest
{
    public int SlotLevel { get; set; }
    public int UsedSlots { get; set; }
}

public class UpdateCurrencyRequest
{
    public int CP { get; set; }
    public int SP { get; set; }
    public int EP { get; set; }
    public int GP { get; set; }
    public int PP { get; set; }
}

public class AddInventoryItemRequest
{
    public int ItemId { get; set; }
    public int Quantity { get; set; } = 1;
    public string Notes { get; set; } = string.Empty;
}

public class ApplyConditionRequest
{
    public int ConditionId { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class ApplyDamageRequest
{
    public int Amount { get; set; }
}

public class HealRequest
{
    public int Amount { get; set; }
}
