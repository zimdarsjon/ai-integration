public class SpellSlotProgression : BaseEntity
{
    public int ClassId { get; set; }
    public int Level { get; set; }
    public int Slot1 { get; set; }
    public int Slot2 { get; set; }
    public int Slot3 { get; set; }
    public int Slot4 { get; set; }
    public int Slot5 { get; set; }
    public int Slot6 { get; set; }
    public int Slot7 { get; set; }
    public int Slot8 { get; set; }
    public int Slot9 { get; set; }

    public DndClass Class { get; set; } = null!;
}
