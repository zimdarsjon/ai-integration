public class ArmorItem : BaseEntity
{
    public int ItemId { get; set; }
    public ArmorType ArmorType { get; set; }
    public int BaseAC { get; set; }
    public int? MaxDexBonus { get; set; }
    public int MinStrength { get; set; }
    public bool StealthDisadvantage { get; set; }
    public bool IsMagical { get; set; }
    public int MagicBonus { get; set; }

    public Item Item { get; set; } = null!;
}
