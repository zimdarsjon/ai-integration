public class SpellClass : BaseEntity
{
    public int SpellId { get; set; }
    public int ClassId { get; set; }

    public Spell Spell { get; set; } = null!;
    public DndClass Class { get; set; } = null!;
}
