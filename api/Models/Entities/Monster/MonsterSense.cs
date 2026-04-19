public class MonsterSense : BaseEntity
{
    public int MonsterId { get; set; }
    public SenseType SenseType { get; set; }
    public int RangeFt { get; set; }

    public Monster Monster { get; set; } = null!;
}
