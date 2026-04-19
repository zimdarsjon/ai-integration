public class CharacterLanguage : BaseEntity
{
    public int CharacterId { get; set; }
    public int LanguageId { get; set; }

    public Character Character { get; set; } = null!;
    public Language Language { get; set; } = null!;
}
