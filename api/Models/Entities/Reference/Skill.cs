public class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int AbilityScoreId { get; set; }

    public AbilityScore AbilityScore { get; set; } = null!;
}
