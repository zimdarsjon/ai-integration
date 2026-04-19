public class CharacterCondition : BaseEntity
{
    public int CharacterId { get; set; }
    public int ConditionId { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;

    public Character Character { get; set; } = null!;
    public Condition Condition { get; set; } = null!;
}
