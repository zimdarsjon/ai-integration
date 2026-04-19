public class CharacterAbilityScore : BaseEntity
{
    public int CharacterId { get; set; }
    public int AbilityScoreId { get; set; }
    public int BaseScore { get; set; }

    public Character Character { get; set; } = null!;
    public AbilityScore AbilityScore { get; set; } = null!;
}
