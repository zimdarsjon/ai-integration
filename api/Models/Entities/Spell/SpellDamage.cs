public class SpellDamage : BaseEntity
{
    public int SpellId { get; set; }
    public int DamageTypeId { get; set; }
    public string DamageDice { get; set; } = string.Empty;

    public Spell Spell { get; set; } = null!;
    public DamageType DamageType { get; set; } = null!;
}
