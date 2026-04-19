public class CharacterHitDice : BaseEntity
{
    public int CharacterId { get; set; }
    public int ClassId { get; set; }
    public int Total { get; set; }
    public int Remaining { get; set; }

    public Character Character { get; set; } = null!;
    public DndClass Class { get; set; } = null!;
}
