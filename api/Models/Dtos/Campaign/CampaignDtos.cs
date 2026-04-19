public class CampaignSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DMName { get; set; } = string.Empty;
    public int MemberCount { get; set; }
    public bool IsActive { get; set; }
    public CampaignRole UserRole { get; set; }
}

public class CampaignDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DMUserId { get; set; }
    public string DMName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Setting { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public List<CampaignMemberDto> Members { get; set; } = [];
    public List<CampaignSessionDto> Sessions { get; set; } = [];
}

public class CampaignMemberDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int? CharacterId { get; set; }
    public string? CharacterName { get; set; }
    public CampaignRole Role { get; set; }
}

public class CampaignSessionDto
{
    public int Id { get; set; }
    public int SessionNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Summary { get; set; } = string.Empty;
}

public class CreateCampaignRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Setting { get; set; } = string.Empty;
}

public class UpdateCampaignRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Setting { get; set; }
    public string? Notes { get; set; }
    public bool? IsActive { get; set; }
}

public class AddMemberRequest
{
    public int UserId { get; set; }
    public CampaignRole Role { get; set; } = CampaignRole.Player;
}
