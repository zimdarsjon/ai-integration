public class MonsterSpellcasting : BaseEntity
{
    public int MonsterId { get; set; }
    public int SpellcastingAbilityScoreId { get; set; }
    public int SpellSaveDC { get; set; }
    public int SpellAttackBonus { get; set; }
    public int CasterLevel { get; set; }

    public Monster Monster { get; set; } = null!;
    public AbilityScore SpellcastingAbilityScore { get; set; } = null!;
    public ICollection<MonsterSpell> Spells { get; set; } = [];
}
