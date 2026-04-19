public class BackgroundFeature : BaseEntity
{
    public int BackgroundId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Background Background { get; set; } = null!;
}
