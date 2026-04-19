public class DndClass : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int HitDie { get; set; }
    public int? SpellcastingAbilityScoreId { get; set; }
    public CasterType CasterType { get; set; } = CasterType.None;
    public bool IsCustom { get; set; }
    public bool IsPublic { get; set; }
    public int? CreatedByUserId { get; set; }
    public string SourceBook { get; set; } = "SRD";

    public AbilityScore? SpellcastingAbilityScore { get; set; }
    public User? CreatedBy { get; set; }
    public ICollection<ClassSavingThrow> SavingThrows { get; set; } = [];
    public ICollection<ClassSkillChoice> SkillChoices { get; set; } = [];
    public ICollection<ClassFeature> Features { get; set; } = [];
    public ICollection<Subclass> Subclasses { get; set; } = [];
    public ICollection<SpellSlotProgression> SpellSlotProgressions { get; set; } = [];
    public ICollection<SpellLimitProgression> SpellLimitProgressions { get; set; } = [];
    public ICollection<SpellClass> SpellClasses { get; set; } = [];
}
