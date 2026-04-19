public class Race : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Speed { get; set; }
    public int SizeId { get; set; }
    public bool IsCustom { get; set; }
    public bool IsPublic { get; set; }
    public int? CreatedByUserId { get; set; }
    public string SourceBook { get; set; } = "SRD";

    public CreatureSize Size { get; set; } = null!;
    public User? CreatedBy { get; set; }
    public ICollection<RaceAbilityBonus> AbilityBonuses { get; set; } = [];
    public ICollection<RaceTrait> Traits { get; set; } = [];
    public ICollection<RaceLanguage> Languages { get; set; } = [];
    public ICollection<Subrace> Subraces { get; set; } = [];
}
