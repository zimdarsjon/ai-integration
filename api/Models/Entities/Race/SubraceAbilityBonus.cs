public class SubraceAbilityBonus : BaseEntity
{
    public int SubraceId { get; set; }
    public int AbilityScoreId { get; set; }
    public int Bonus { get; set; }

    public Subrace Subrace { get; set; } = null!;
    public AbilityScore AbilityScore { get; set; } = null!;
}
