public class Weapon : BaseEntity
{
    public int ItemId { get; set; }
    public string DamageDice { get; set; } = string.Empty;
    public int DamageTypeId { get; set; }
    public int? NormalRangeFt { get; set; }
    public int? LongRangeFt { get; set; }
    public bool IsRanged { get; set; }
    public bool IsMartial { get; set; }
    public bool IsMagical { get; set; }
    public int MagicBonus { get; set; }

    public Item Item { get; set; } = null!;
    public DamageType DamageType { get; set; } = null!;
    public ICollection<WeaponPropertyAssignment> PropertyAssignments { get; set; } = [];
}
