public class CampaignSession : BaseEntity
{
    public int CampaignId { get; set; }
    public int SessionNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public Campaign Campaign { get; set; } = null!;
}
