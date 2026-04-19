public class RaceAbilityBonus : BaseEntity
{
    public int RaceId { get; set; }
    public int AbilityScoreId { get; set; }
    public int Bonus { get; set; }

    public Race Race { get; set; } = null!;
    public AbilityScore AbilityScore { get; set; } = null!;
}
