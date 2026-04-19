public class Monster : BaseEntity
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
    public bool IsPublic { get; set; }
    public int? CreatedByUserId { get; set; }
    public string SourceBook { get; set; } = "SRD";

    public CreatureType CreatureType { get; set; } = null!;
    public CreatureSize Size { get; set; } = null!;
    public Alignment? Alignment { get; set; }
    public User? CreatedBy { get; set; }
    public ICollection<MonsterAbilityScore> AbilityScores { get; set; } = [];
    public ICollection<MonsterSkill> Skills { get; set; } = [];
    public ICollection<MonsterSavingThrow> SavingThrows { get; set; } = [];
    public ICollection<MonsterDamageModifier> DamageModifiers { get; set; } = [];
    public ICollection<MonsterConditionImmunity> ConditionImmunities { get; set; } = [];
    public ICollection<MonsterSense> Senses { get; set; } = [];
    public ICollection<MonsterLanguage> Languages { get; set; } = [];
    public ICollection<MonsterTrait> Traits { get; set; } = [];
    public ICollection<MonsterAction> Actions { get; set; } = [];
    public MonsterSpellcasting? Spellcasting { get; set; }
}
