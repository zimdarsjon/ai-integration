public class AbilityScore : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCustom { get; set; }
    public bool IsPublic { get; set; }
    public int? CreatedByUserId { get; set; }

    public User? CreatedBy { get; set; }
    public ICollection<Skill> Skills { get; set; } = [];
}
