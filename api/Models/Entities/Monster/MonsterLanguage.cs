public class MonsterLanguage : BaseEntity
{
    public int MonsterId { get; set; }
    public int LanguageId { get; set; }
    public bool CanSpeak { get; set; }

    public Monster Monster { get; set; } = null!;
    public Language Language { get; set; } = null!;
}
