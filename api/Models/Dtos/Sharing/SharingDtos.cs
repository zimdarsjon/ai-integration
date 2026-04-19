public class ContentShareDto
{
    public int Id { get; set; }
    public CustomContentType ContentType { get; set; }
    public int ContentId { get; set; }
    public int? SharedWithUserId { get; set; }
    public string? SharedWithUserName { get; set; }
    public int? SharedWithCampaignId { get; set; }
    public string? SharedWithCampaignName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ShareWithUserRequest
{
    public CustomContentType ContentType { get; set; }
    public int ContentId { get; set; }
    public int TargetUserId { get; set; }
}

public class ShareWithCampaignRequest
{
    public CustomContentType ContentType { get; set; }
    public int ContentId { get; set; }
    public int CampaignId { get; set; }
}

public class SetPublicRequest
{
    public bool IsPublic { get; set; }
}
