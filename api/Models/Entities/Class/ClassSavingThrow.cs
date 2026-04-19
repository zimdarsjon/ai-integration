public class ClassSavingThrow : BaseEntity
{
    public int ClassId { get; set; }
    public int AbilityScoreId { get; set; }

    public DndClass Class { get; set; } = null!;
    public AbilityScore AbilityScore { get; set; } = null!;
}
