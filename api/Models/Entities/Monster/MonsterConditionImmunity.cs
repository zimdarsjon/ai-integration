public class MonsterConditionImmunity : BaseEntity
{
    public int MonsterId { get; set; }
    public int ConditionId { get; set; }

    public Monster Monster { get; set; } = null!;
    public Condition Condition { get; set; } = null!;
}
