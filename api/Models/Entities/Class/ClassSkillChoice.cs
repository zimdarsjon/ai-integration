public class ClassSkillChoice : BaseEntity
{
    public int ClassId { get; set; }
    public int SkillId { get; set; }
    public int NumberOfChoices { get; set; }

    public DndClass Class { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
