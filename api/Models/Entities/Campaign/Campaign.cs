public class Campaign : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DMUserId { get; set; }
    public bool IsActive { get; set; } = true;
    public string Setting { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public User DM { get; set; } = null!;
    public ICollection<CampaignMember> Members { get; set; } = [];
    public ICollection<CampaignSession> Sessions { get; set; } = [];
    public ICollection<Encounter> Encounters { get; set; } = [];
}
