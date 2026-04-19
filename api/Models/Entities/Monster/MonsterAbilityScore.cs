public class MonsterAbilityScore : BaseEntity
{
    public int MonsterId { get; set; }
    public int AbilityScoreId { get; set; }
    public int Score { get; set; }

    public Monster Monster { get; set; } = null!;
    public AbilityScore AbilityScore { get; set; } = null!;
}
