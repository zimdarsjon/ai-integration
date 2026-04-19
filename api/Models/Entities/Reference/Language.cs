public class Language : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Script { get; set; } = string.Empty;
    public bool IsExotic { get; set; }
}
