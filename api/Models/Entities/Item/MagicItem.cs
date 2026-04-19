public class MagicItem : BaseEntity
{
    public int ItemId { get; set; }
    public ItemRarity Rarity { get; set; }
    public bool RequiresAttunement { get; set; }
    public string? AttunementRequirements { get; set; }
    public string Properties { get; set; } = string.Empty;

    public Item Item { get; set; } = null!;
}
