public class RaceLanguage : BaseEntity
{
    public int RaceId { get; set; }
    public int LanguageId { get; set; }

    public Race Race { get; set; } = null!;
    public Language Language { get; set; } = null!;
}
