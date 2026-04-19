public class Background : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int LanguagesGranted { get; set; }
    public string ToolProficiencyDescription { get; set; } = string.Empty;
    public string StartingEquipmentDescription { get; set; } = string.Empty;
    public bool IsCustom { get; set; }
    public bool IsPublic { get; set; }
    public int? CreatedByUserId { get; set; }
    public string SourceBook { get; set; } = "SRD";

    public User? CreatedBy { get; set; }
    public ICollection<BackgroundFeature> Features { get; set; } = [];
    public ICollection<BackgroundSkillProficiency> SkillProficiencies { get; set; } = [];
}
