public class MonsterSkill : BaseEntity
{
    public int MonsterId { get; set; }
    public int SkillId { get; set; }
    public bool IsExpertise { get; set; }

    public Monster Monster { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
