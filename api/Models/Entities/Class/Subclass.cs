public class Subclass : BaseEntity
{
    public int ClassId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ChoiceLevel { get; set; }

    public DndClass Class { get; set; } = null!;
    public ICollection<SubclassFeature> Features { get; set; } = [];
}
