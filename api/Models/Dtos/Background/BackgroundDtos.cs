public class BackgroundSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCustom { get; set; }
    public bool IsPublic { get; set; }
    public string SourceBook { get; set; } = string.Empty;
}

public class BackgroundDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int LanguagesGranted { get; set; }
    public string ToolProficiencyDescription { get; set; } = string.Empty;
    public string StartingEquipmentDescription { get; set; } = string.Empty;
    public bool IsCustom { get; set; }
    public bool IsPublic { get; set; }
    public int? CreatedByUserId { get; set; }
    public string SourceBook { get; set; } = string.Empty;
    public List<BackgroundFeatureDto> Features { get; set; } = [];
    public List<string> SkillProficiencies { get; set; } = [];
}

public class BackgroundFeatureDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateBackgroundRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int LanguagesGranted { get; set; }
    public string ToolProficiencyDescription { get; set; } = string.Empty;
    public string StartingEquipmentDescription { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public List<CreateBackgroundFeatureRequest> Features { get; set; } = [];
    public List<string> SkillProficiencies { get; set; } = [];
}

public class CreateBackgroundFeatureRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateBackgroundRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? LanguagesGranted { get; set; }
    public string? ToolProficiencyDescription { get; set; }
    public string? StartingEquipmentDescription { get; set; }
    public bool? IsPublic { get; set; }
}
