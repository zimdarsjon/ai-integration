public class MonsterSavingThrow : BaseEntity
{
    public int MonsterId { get; set; }
    public int AbilityScoreId { get; set; }

    public Monster Monster { get; set; } = null!;
    public AbilityScore AbilityScore { get; set; } = null!;
}
