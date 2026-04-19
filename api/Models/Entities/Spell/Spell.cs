public class Spell : BaseEntity
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
    public bool IsCustom { get; set; }
    public bool IsPublic { get; set; }
    public int? CreatedByUserId { get; set; }
    public string SourceBook { get; set; } = "SRD";

    public MagicSchool School { get; set; } = null!;
    public User? CreatedBy { get; set; }
    public ICollection<SpellClass> SpellClasses { get; set; } = [];
    public ICollection<SpellDamage> Damages { get; set; } = [];
}
