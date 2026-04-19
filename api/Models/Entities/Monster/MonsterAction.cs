public class MonsterAction : BaseEntity
{
    public int MonsterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActionType ActionType { get; set; }
    public int? AttackBonus { get; set; }
    public string? ReachOrRange { get; set; }
    public string? HitDamageDice { get; set; }
    public int? HitDamageTypeId { get; set; }
    public int? SaveDC { get; set; }
    public int? SaveAbilityScoreId { get; set; }
    public int LegendaryCost { get; set; } = 1;

    public Monster Monster { get; set; } = null!;
    public DamageType? HitDamageType { get; set; }
    public AbilityScore? SaveAbilityScore { get; set; }
}
