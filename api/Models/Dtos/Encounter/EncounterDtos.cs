public class EncounterSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Round { get; set; }
    public bool IsActive { get; set; }
    public int ParticipantCount { get; set; }
}

public class EncounterDetailDto
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Round { get; set; }
    public bool IsActive { get; set; }
    public List<ParticipantDto> Participants { get; set; } = [];
}

public class ParticipantDto
{
    public int Id { get; set; }
    public ParticipantType ParticipantType { get; set; }
    public int? CharacterId { get; set; }
    public int? MonsterId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int Initiative { get; set; }
    public int CurrentHP { get; set; }
    public int MaxHP { get; set; }
    public int AC { get; set; }
    public int TurnOrder { get; set; }
    public bool IsActive { get; set; }
    public List<string> Conditions { get; set; } = [];
}

public class CreateEncounterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? SessionId { get; set; }
}

public class AddParticipantRequest
{
    public ParticipantType ParticipantType { get; set; }
    public int? CharacterId { get; set; }
    public int? MonsterId { get; set; }
    public string? DisplayName { get; set; }
    public int Initiative { get; set; }
    public int? MaxHP { get; set; }
    public int? AC { get; set; }
}

public class ApplyEncounterDamageRequest
{
    public int Amount { get; set; }
    public bool SyncToCharacter { get; set; } = true;
}
