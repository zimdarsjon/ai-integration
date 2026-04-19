public class EncounterParticipant : BaseEntity
{
    public int EncounterId { get; set; }
    public ParticipantType ParticipantType { get; set; }
    public int? CharacterId { get; set; }
    public int? MonsterId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int Initiative { get; set; }
    public int CurrentHP { get; set; }
    public int MaxHP { get; set; }
    public int AC { get; set; }
    public int TurnOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public Encounter Encounter { get; set; } = null!;
    public Character? Character { get; set; }
    public Monster? Monster { get; set; }
    public ICollection<EncounterCondition> Conditions { get; set; } = [];
}
