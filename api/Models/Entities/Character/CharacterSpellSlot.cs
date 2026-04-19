public class CharacterSpellSlot : BaseEntity
{
    public int CharacterId { get; set; }
    public int SlotLevel { get; set; }
    public int TotalSlots { get; set; }
    public int UsedSlots { get; set; }

    public Character Character { get; set; } = null!;
}
