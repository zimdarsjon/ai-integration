public class CharacterFeature : BaseEntity
{
    public int CharacterId { get; set; }
    public FeatureSource Source { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Level { get; set; }
    public int? UsesMax { get; set; }
    public int? UsesRemaining { get; set; }

    public Character Character { get; set; } = null!;
}
