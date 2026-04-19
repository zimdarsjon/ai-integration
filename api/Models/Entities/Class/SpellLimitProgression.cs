public class SpellLimitProgression : BaseEntity
{
    public int ClassId { get; set; }
    public int Level { get; set; }
    public int CantripsKnown { get; set; }
    public int? SpellsKnown { get; set; }      // null = uses prepared-spell formula (abilityMod + level)
    public int? StartingSpells { get; set; }   // creation-time selection limit; null = falls back to SpellsKnown / prepared formula

    public DndClass Class { get; set; } = null!;
}
