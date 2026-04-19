public class Character : BaseEntity
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? RaceId { get; set; }
    public int? SubraceId { get; set; }
    public int? AlignmentId { get; set; }
    public int? BackgroundId { get; set; }
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
    public int TotalLevel { get; set; } = 1;
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

    public User User { get; set; } = null!;
    public Race? Race { get; set; }
    public Subrace? Subrace { get; set; }
    public Alignment? Alignment { get; set; }
    public Background? Background { get; set; }
    public CharacterCurrency? Currency { get; set; }
    public ICollection<CharacterClass> Classes { get; set; } = [];
    public ICollection<CharacterAbilityScore> AbilityScores { get; set; } = [];
    public ICollection<CharacterSkillProficiency> SkillProficiencies { get; set; } = [];
    public ICollection<CharacterSavingThrow> SavingThrows { get; set; } = [];
    public ICollection<CharacterSpellSlot> SpellSlots { get; set; } = [];
    public ICollection<CharacterSpell> Spells { get; set; } = [];
    public ICollection<CharacterCondition> Conditions { get; set; } = [];
    public ICollection<CharacterInventoryItem> Inventory { get; set; } = [];
    public ICollection<CharacterLanguage> Languages { get; set; } = [];
    public ICollection<CharacterFeature> Features { get; set; } = [];
    public ICollection<CharacterHitDice> HitDice { get; set; } = [];
}
