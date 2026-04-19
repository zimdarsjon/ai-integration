public class CharacterSkillProficiency : BaseEntity
{
    public int CharacterId { get; set; }
    public int SkillId { get; set; }
    public ProficiencyType ProficiencyType { get; set; }

    public Character Character { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
