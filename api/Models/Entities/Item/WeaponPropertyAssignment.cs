public class WeaponPropertyAssignment : BaseEntity
{
    public int WeaponId { get; set; }
    public int WeaponPropertyId { get; set; }

    public Weapon Weapon { get; set; } = null!;
    public WeaponProperty WeaponProperty { get; set; } = null!;
}
