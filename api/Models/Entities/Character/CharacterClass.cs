public class CharacterClass : BaseEntity
{
    public int CharacterId { get; set; }
    public int ClassId { get; set; }
    public int? SubclassId { get; set; }
    public int Level { get; set; }

    public Character Character { get; set; } = null!;
    public DndClass Class { get; set; } = null!;
    public Subclass? Subclass { get; set; }
}
