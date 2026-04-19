public class SubclassFeature : BaseEntity
{
    public int SubclassId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Level { get; set; }

    public Subclass Subclass { get; set; } = null!;
}
