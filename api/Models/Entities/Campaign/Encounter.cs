public class Encounter : BaseEntity
{
    public int CampaignId { get; set; }
    public int? SessionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Round { get; set; } = 1;
    public bool IsActive { get; set; }

    public Campaign Campaign { get; set; } = null!;
    public CampaignSession? Session { get; set; }
    public ICollection<EncounterParticipant> Participants { get; set; } = [];
}
