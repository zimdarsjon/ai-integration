public class EncounterCondition : BaseEntity
{
    public int EncounterParticipantId { get; set; }
    public int ConditionId { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public EncounterParticipant Participant { get; set; } = null!;
    public Condition Condition { get; set; } = null!;
}
