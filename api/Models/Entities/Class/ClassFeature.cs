public class ClassFeature : BaseEntity
{
    public int ClassId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Level { get; set; }

    public DndClass Class { get; set; } = null!;
}
