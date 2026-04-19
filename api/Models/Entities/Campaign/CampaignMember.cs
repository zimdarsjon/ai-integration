public class CampaignMember : BaseEntity
{
    public int CampaignId { get; set; }
    public int UserId { get; set; }
    public int? CharacterId { get; set; }
    public CampaignRole Role { get; set; }

    public Campaign Campaign { get; set; } = null!;
    public User User { get; set; } = null!;
    public Character? Character { get; set; }
}
