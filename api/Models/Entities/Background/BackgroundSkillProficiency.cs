public class BackgroundSkillProficiency : BaseEntity
{
    public int BackgroundId { get; set; }
    public string SkillName { get; set; } = string.Empty;

    public Background Background { get; set; } = null!;
}
