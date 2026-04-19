public class MonsterTrait : BaseEntity
{
    public int MonsterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Monster Monster { get; set; } = null!;
}
