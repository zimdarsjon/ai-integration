public class CharacterInventoryItem : BaseEntity
{
    public int CharacterId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsEquipped { get; set; }
    public bool IsAttuned { get; set; }
    public EquipmentSlot EquipmentSlot { get; set; }
    public string Notes { get; set; } = string.Empty;

    public Character Character { get; set; } = null!;
    public Item Item { get; set; } = null!;
}
