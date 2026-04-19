public class RaceTrait : BaseEntity
{
    public int RaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Race Race { get; set; } = null!;
}
