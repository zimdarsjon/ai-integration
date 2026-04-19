public class CharacterSavingThrow : BaseEntity
{
    public int CharacterId { get; set; }
    public int AbilityScoreId { get; set; }
    public bool IsProficient { get; set; }

    public Character Character { get; set; } = null!;
    public AbilityScore AbilityScore { get; set; } = null!;
}
