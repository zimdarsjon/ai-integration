public class CharacterCurrency : BaseEntity
{
    public int CharacterId { get; set; }
    public int CP { get; set; }
    public int SP { get; set; }
    public int EP { get; set; }
    public int GP { get; set; }
    public int PP { get; set; }

    public Character Character { get; set; } = null!;
}
