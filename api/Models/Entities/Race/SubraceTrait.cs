public class SubraceTrait : BaseEntity
{
    public int SubraceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Subrace Subrace { get; set; } = null!;
}
