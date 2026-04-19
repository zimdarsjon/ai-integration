public class Item : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal WeightLbs { get; set; }
    public int CostCp { get; set; }
    public ItemType ItemType { get; set; }
    public bool IsCustom { get; set; }
    public bool IsPublic { get; set; }
    public int? CreatedByUserId { get; set; }
    public string SourceBook { get; set; } = "SRD";

    public User? CreatedBy { get; set; }
    public Weapon? Weapon { get; set; }
    public ArmorItem? Armor { get; set; }
    public MagicItem? MagicItem { get; set; }
}
