public class CharacterSpell : BaseEntity
{
    public int CharacterId { get; set; }
    public int SpellId { get; set; }
    public bool IsPrepared { get; set; }

    public Character Character { get; set; } = null!;
    public Spell Spell { get; set; } = null!;
}
