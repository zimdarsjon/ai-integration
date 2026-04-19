public class ReferenceItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AbilityScoreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCustom { get; set; }
}

public class SkillDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int AbilityScoreId { get; set; }
    public string AbilityAbbreviation { get; set; } = string.Empty;
}

public class RaceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Speed { get; set; }
    public string Size { get; set; } = string.Empty;
    public bool IsCustom { get; set; }
    public List<AbilityBonusDto> AbilityBonuses { get; set; } = [];
    public List<TraitDto> Traits { get; set; } = [];
    public List<string> Languages { get; set; } = [];
    public List<SubraceDto> Subraces { get; set; } = [];
}

public class SubraceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<AbilityBonusDto> AbilityBonuses { get; set; } = [];
    public List<TraitDto> Traits { get; set; } = [];
}

public class AbilityBonusDto
{
    public string AbilityScoreName { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public int Bonus { get; set; }
}

public class TraitDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ClassDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int HitDie { get; set; }
    public string? SpellcastingAbility { get; set; }
    public CasterType CasterType { get; set; }
    public bool IsCustom { get; set; }
    public List<string> SavingThrows { get; set; } = [];
    public List<ClassFeatureDto> Features { get; set; } = [];
    public List<SubclassDto> Subclasses { get; set; } = [];
    public List<SpellSlotRowDto> SpellSlotProgression { get; set; } = [];
    public List<SpellLimitRowDto> SpellLimitProgression { get; set; } = [];
    public ClassSkillChoiceDto? SkillChoices { get; set; }
}

public class ClassSkillChoiceDto
{
    public int NumberOfChoices { get; set; }
    public List<SkillDto> AvailableSkills { get; set; } = [];
}

public class SpellLimitRowDto
{
    public int Level { get; set; }
    public int CantripsKnown { get; set; }
    public int? SpellsKnown { get; set; }    // null = uses prepared formula (ability mod + level)
    public int? StartingSpells { get; set; } // character-creation selection limit; null = falls back to SpellsKnown / prepared formula
}

public class ClassFeatureDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Level { get; set; }
}

public class SubclassDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ChoiceLevel { get; set; }
    public List<ClassFeatureDto> Features { get; set; } = [];
}

public class SpellSlotRowDto
{
    public int Level { get; set; }
    public int[] Slots { get; set; } = new int[9];
}

public class ItemSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public decimal WeightLbs { get; set; }
    public int CostCp { get; set; }
    public bool IsCustom { get; set; }
}

public class ItemDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public decimal WeightLbs { get; set; }
    public int CostCp { get; set; }
    public bool IsCustom { get; set; }
    public WeaponDetailDto? Weapon { get; set; }
    public ArmorDetailDto? Armor { get; set; }
    public MagicItemDetailDto? MagicItem { get; set; }
}

public class WeaponDetailDto
{
    public string DamageDice { get; set; } = string.Empty;
    public string DamageType { get; set; } = string.Empty;
    public int? NormalRangeFt { get; set; }
    public int? LongRangeFt { get; set; }
    public bool IsRanged { get; set; }
    public bool IsMartial { get; set; }
    public int MagicBonus { get; set; }
    public List<string> Properties { get; set; } = [];
}

public class ArmorDetailDto
{
    public ArmorType ArmorType { get; set; }
    public int BaseAC { get; set; }
    public int? MaxDexBonus { get; set; }
    public int MinStrength { get; set; }
    public bool StealthDisadvantage { get; set; }
    public int MagicBonus { get; set; }
}

public class MagicItemDetailDto
{
    public ItemRarity Rarity { get; set; }
    public bool RequiresAttunement { get; set; }
    public string? AttunementRequirements { get; set; }
    public string Properties { get; set; } = string.Empty;
}

public class SpellSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public string School { get; set; } = string.Empty;
    public string CastingTime { get; set; } = string.Empty;
    public bool IsConcentration { get; set; }
    public bool IsRitual { get; set; }
    public bool IsCustom { get; set; }
}

public class SpellDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public int SchoolId { get; set; }
    public string School { get; set; } = string.Empty;
    public string CastingTime { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    public SpellComponents Components { get; set; }
    public string? MaterialComponent { get; set; }
    public string Duration { get; set; } = string.Empty;
    public bool IsConcentration { get; set; }
    public bool IsRitual { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? AtHigherLevels { get; set; }
    public bool IsCustom { get; set; }
    public List<string> Classes { get; set; } = [];
    public List<SpellDamageDto> Damages { get; set; } = [];
}

public class SpellDamageDto
{
    public string DamageType { get; set; } = string.Empty;
    public string DamageDice { get; set; } = string.Empty;
}

public class MonsterSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal ChallengeRating { get; set; }
    public int XP { get; set; }
    public string CreatureType { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public int AC { get; set; }
    public int MaxHP { get; set; }
    public bool IsCustom { get; set; }
}

public class MonsterDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatureType { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string? Alignment { get; set; }
    public int AC { get; set; }
    public string ACSource { get; set; } = string.Empty;
    public int MaxHP { get; set; }
    public string HitDice { get; set; } = string.Empty;
    public int WalkSpeed { get; set; }
    public int? SwimSpeed { get; set; }
    public int? FlySpeed { get; set; }
    public int? ClimbSpeed { get; set; }
    public int? BurrowSpeed { get; set; }
    public decimal ChallengeRating { get; set; }
    public int XP { get; set; }
    public int ProficiencyBonus { get; set; }
    public bool IsLegendary { get; set; }
    public int LegendaryActionCount { get; set; }
    public bool IsCustom { get; set; }
    public List<MonsterAbilityScoreDto> AbilityScores { get; set; } = [];
    public List<string> SavingThrows { get; set; } = [];
    public List<string> Skills { get; set; } = [];
    public List<string> DamageImmunities { get; set; } = [];
    public List<string> DamageResistances { get; set; } = [];
    public List<string> DamageVulnerabilities { get; set; } = [];
    public List<string> ConditionImmunities { get; set; } = [];
    public List<string> Languages { get; set; } = [];
    public List<MonsterSenseDto> Senses { get; set; } = [];
    public List<TraitDto> Traits { get; set; } = [];
    public List<MonsterActionDto> Actions { get; set; } = [];
}

public class MonsterAbilityScoreDto
{
    public string Abbreviation { get; set; } = string.Empty;
    public int Score { get; set; }
    public int Modifier => (Score - 10) / 2;
}

public class MonsterSenseDto
{
    public string SenseType { get; set; } = string.Empty;
    public int RangeFt { get; set; }
}

public class MonsterActionDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActionType ActionType { get; set; }
    public int? AttackBonus { get; set; }
    public string? ReachOrRange { get; set; }
    public string? HitDamageDice { get; set; }
    public string? HitDamageType { get; set; }
    public int? SaveDC { get; set; }
    public string? SaveAbility { get; set; }
    public int LegendaryCost { get; set; }
}

public class CreateSpellRequest
{
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public int SchoolId { get; set; }
    public string CastingTime { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    public SpellComponents Components { get; set; }
    public string? MaterialComponent { get; set; }
    public string Duration { get; set; } = string.Empty;
    public bool IsConcentration { get; set; }
    public bool IsRitual { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? AtHigherLevels { get; set; }
    public List<int> ClassIds { get; set; } = [];
}

public class CreateMonsterRequest
{
    public string Name { get; set; } = string.Empty;
    public int CreatureTypeId { get; set; }
    public int SizeId { get; set; }
    public int? AlignmentId { get; set; }
    public int AC { get; set; }
    public string ACSource { get; set; } = string.Empty;
    public int MaxHP { get; set; }
    public string HitDice { get; set; } = string.Empty;
    public int WalkSpeed { get; set; }
    public decimal ChallengeRating { get; set; }
    public int XP { get; set; }
    public int ProficiencyBonus { get; set; }
    public Dictionary<int, int> AbilityScores { get; set; } = [];
}

public class CreateAbilityScoreRequest
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateRaceRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Speed { get; set; }
    public int SizeId { get; set; }
    public List<AbilityBonusInput> AbilityBonuses { get; set; } = [];
    public List<TraitInput> Traits { get; set; } = [];
    public List<int> LanguageIds { get; set; } = [];
}

public class AbilityBonusInput
{
    public int AbilityScoreId { get; set; }
    public int Bonus { get; set; }
}

public class TraitInput
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateClassRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int HitDie { get; set; }
    public int? SpellcastingAbilityScoreId { get; set; }
    public CasterType CasterType { get; set; }
    public List<int> SavingThrowAbilityIds { get; set; } = [];
}

public class CreateItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal WeightLbs { get; set; }
    public int CostCp { get; set; }
    public ItemType ItemType { get; set; }
    public WeaponInput? Weapon { get; set; }
    public ArmorInput? Armor { get; set; }
}

public class WeaponInput
{
    public string DamageDice { get; set; } = string.Empty;
    public int DamageTypeId { get; set; }
    public bool IsRanged { get; set; }
    public bool IsMartial { get; set; }
    public int? NormalRangeFt { get; set; }
    public int? LongRangeFt { get; set; }
    public List<int> PropertyIds { get; set; } = [];
}

public class ArmorInput
{
    public ArmorType ArmorType { get; set; }
    public int BaseAC { get; set; }
    public int? MaxDexBonus { get; set; }
    public int MinStrength { get; set; }
    public bool StealthDisadvantage { get; set; }
}
