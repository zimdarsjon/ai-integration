public class CustomContentShare : BaseEntity
{
    public CustomContentType ContentType { get; set; }
    public int ContentId { get; set; }
    public int CreatedByUserId { get; set; }
    public int? SharedWithUserId { get; set; }
    public int? SharedWithCampaignId { get; set; }

    public User CreatedBy { get; set; } = null!;
    public User? SharedWithUser { get; set; }
    public Campaign? SharedWithCampaign { get; set; }
}
