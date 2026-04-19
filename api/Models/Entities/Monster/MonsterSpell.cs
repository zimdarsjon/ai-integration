public class MonsterSpell : BaseEntity
{
    public int MonsterSpellcastingId { get; set; }
    public int SpellId { get; set; }
    public int? UsesPerDay { get; set; }

    public MonsterSpellcasting MonsterSpellcasting { get; set; } = null!;
    public Spell Spell { get; set; } = null!;
}
