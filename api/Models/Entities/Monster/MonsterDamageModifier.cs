public class MonsterDamageModifier : BaseEntity
{
    public int MonsterId { get; set; }
    public int DamageTypeId { get; set; }
    public DamageModifierType ModifierType { get; set; }

    public Monster Monster { get; set; } = null!;
    public DamageType DamageType { get; set; } = null!;
}
