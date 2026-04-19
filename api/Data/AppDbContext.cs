using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Auth
    public DbSet<User> Users => Set<User>();

    // Sharing
    public DbSet<CustomContentShare> CustomContentShares => Set<CustomContentShare>();

    // Reference
    public DbSet<AbilityScore> AbilityScores => Set<AbilityScore>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<DamageType> DamageTypes => Set<DamageType>();
    public DbSet<Condition> Conditions => Set<Condition>();
    public DbSet<MagicSchool> MagicSchools => Set<MagicSchool>();
    public DbSet<WeaponProperty> WeaponProperties => Set<WeaponProperty>();
    public DbSet<CreatureSize> CreatureSizes => Set<CreatureSize>();
    public DbSet<CreatureType> CreatureTypes => Set<CreatureType>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Alignment> Alignments => Set<Alignment>();

    // Background
    public DbSet<Background> Backgrounds => Set<Background>();
    public DbSet<BackgroundFeature> BackgroundFeatures => Set<BackgroundFeature>();
    public DbSet<BackgroundSkillProficiency> BackgroundSkillProficiencies => Set<BackgroundSkillProficiency>();

    // Race
    public DbSet<Race> Races => Set<Race>();
    public DbSet<RaceAbilityBonus> RaceAbilityBonuses => Set<RaceAbilityBonus>();
    public DbSet<RaceTrait> RaceTraits => Set<RaceTrait>();
    public DbSet<RaceLanguage> RaceLanguages => Set<RaceLanguage>();
    public DbSet<Subrace> Subraces => Set<Subrace>();
    public DbSet<SubraceAbilityBonus> SubraceAbilityBonuses => Set<SubraceAbilityBonus>();
    public DbSet<SubraceTrait> SubraceTraits => Set<SubraceTrait>();

    // Class
    public DbSet<DndClass> Classes => Set<DndClass>();
    public DbSet<ClassSavingThrow> ClassSavingThrows => Set<ClassSavingThrow>();
    public DbSet<ClassSkillChoice> ClassSkillChoices => Set<ClassSkillChoice>();
    public DbSet<ClassFeature> ClassFeatures => Set<ClassFeature>();
    public DbSet<Subclass> Subclasses => Set<Subclass>();
    public DbSet<SubclassFeature> SubclassFeatures => Set<SubclassFeature>();
    public DbSet<SpellSlotProgression> SpellSlotProgressions => Set<SpellSlotProgression>();
    public DbSet<SpellLimitProgression> SpellLimitProgressions => Set<SpellLimitProgression>();

    // Spell
    public DbSet<Spell> Spells => Set<Spell>();
    public DbSet<SpellClass> SpellClasses => Set<SpellClass>();
    public DbSet<SpellDamage> SpellDamages => Set<SpellDamage>();

    // Item
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Weapon> Weapons => Set<Weapon>();
    public DbSet<WeaponPropertyAssignment> WeaponPropertyAssignments => Set<WeaponPropertyAssignment>();
    public DbSet<ArmorItem> Armors => Set<ArmorItem>();
    public DbSet<MagicItem> MagicItems => Set<MagicItem>();

    // Monster
    public DbSet<Monster> Monsters => Set<Monster>();
    public DbSet<MonsterAbilityScore> MonsterAbilityScores => Set<MonsterAbilityScore>();
    public DbSet<MonsterSkill> MonsterSkills => Set<MonsterSkill>();
    public DbSet<MonsterSavingThrow> MonsterSavingThrows => Set<MonsterSavingThrow>();
    public DbSet<MonsterDamageModifier> MonsterDamageModifiers => Set<MonsterDamageModifier>();
    public DbSet<MonsterConditionImmunity> MonsterConditionImmunities => Set<MonsterConditionImmunity>();
    public DbSet<MonsterSense> MonsterSenses => Set<MonsterSense>();
    public DbSet<MonsterLanguage> MonsterLanguages => Set<MonsterLanguage>();
    public DbSet<MonsterTrait> MonsterTraits => Set<MonsterTrait>();
    public DbSet<MonsterAction> MonsterActions => Set<MonsterAction>();
    public DbSet<MonsterSpellcasting> MonsterSpellcastings => Set<MonsterSpellcasting>();
    public DbSet<MonsterSpell> MonsterSpells => Set<MonsterSpell>();

    // Character
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterAbilityScore> CharacterAbilityScores => Set<CharacterAbilityScore>();
    public DbSet<CharacterClass> CharacterClasses => Set<CharacterClass>();
    public DbSet<CharacterSkillProficiency> CharacterSkillProficiencies => Set<CharacterSkillProficiency>();
    public DbSet<CharacterSavingThrow> CharacterSavingThrows => Set<CharacterSavingThrow>();
    public DbSet<CharacterSpellSlot> CharacterSpellSlots => Set<CharacterSpellSlot>();
    public DbSet<CharacterSpell> CharacterSpells => Set<CharacterSpell>();
    public DbSet<CharacterCondition> CharacterConditions => Set<CharacterCondition>();
    public DbSet<CharacterInventoryItem> CharacterInventory => Set<CharacterInventoryItem>();
    public DbSet<CharacterCurrency> CharacterCurrencies => Set<CharacterCurrency>();
    public DbSet<CharacterLanguage> CharacterLanguages => Set<CharacterLanguage>();
    public DbSet<CharacterFeature> CharacterFeatures => Set<CharacterFeature>();
    public DbSet<CharacterHitDice> CharacterHitDice => Set<CharacterHitDice>();

    // Campaign
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignMember> CampaignMembers => Set<CampaignMember>();
    public DbSet<CampaignSession> CampaignSessions => Set<CampaignSession>();
    public DbSet<Encounter> Encounters => Set<Encounter>();
    public DbSet<EncounterParticipant> EncounterParticipants => Set<EncounterParticipant>();
    public DbSet<EncounterCondition> EncounterConditions => Set<EncounterCondition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
        });

        modelBuilder.Entity<AbilityScore>(e =>
        {
            e.Property(a => a.Name).IsRequired().HasMaxLength(50);
            e.Property(a => a.Abbreviation).IsRequired().HasMaxLength(3);
        });

        modelBuilder.Entity<Background>(e =>
        {
            e.HasOne(b => b.CreatedBy).WithMany().HasForeignKey(b => b.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
            e.Property(b => b.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Race>(e =>
        {
            e.HasOne(r => r.CreatedBy).WithMany().HasForeignKey(r => r.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<DndClass>(e =>
        {
            e.ToTable("Classes");
            e.HasOne(c => c.CreatedBy).WithMany().HasForeignKey(c => c.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.SpellcastingAbilityScore).WithMany().HasForeignKey(c => c.SpellcastingAbilityScoreId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Spell>(e =>
        {
            e.HasOne(s => s.CreatedBy).WithMany().HasForeignKey(s => s.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Item>(e =>
        {
            e.HasOne(i => i.CreatedBy).WithMany().HasForeignKey(i => i.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
            e.Property(i => i.WeightLbs).HasColumnType("decimal(8,2)");
        });

        modelBuilder.Entity<Weapon>(e =>
        {
            e.HasOne(w => w.Item).WithOne(i => i.Weapon).HasForeignKey<Weapon>(w => w.ItemId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ArmorItem>(e =>
        {
            e.ToTable("Armors");
            e.HasOne(a => a.Item).WithOne(i => i.Armor).HasForeignKey<ArmorItem>(a => a.ItemId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MagicItem>(e =>
        {
            e.HasOne(m => m.Item).WithOne(i => i.MagicItem).HasForeignKey<MagicItem>(m => m.ItemId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Monster>(e =>
        {
            e.HasOne(m => m.CreatedBy).WithMany().HasForeignKey(m => m.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(m => m.Alignment).WithMany().HasForeignKey(m => m.AlignmentId).OnDelete(DeleteBehavior.SetNull);
            e.Property(m => m.ChallengeRating).HasColumnType("decimal(5,3)");
        });

        modelBuilder.Entity<MonsterAction>(e =>
        {
            e.HasOne(a => a.HitDamageType).WithMany().HasForeignKey(a => a.HitDamageTypeId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(a => a.SaveAbilityScore).WithMany().HasForeignKey(a => a.SaveAbilityScoreId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<MonsterSpellcasting>(e =>
        {
            e.HasOne(ms => ms.Monster).WithOne(m => m.Spellcasting).HasForeignKey<MonsterSpellcasting>(ms => ms.MonsterId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Character>(e =>
        {
            e.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(c => c.Race).WithMany().HasForeignKey(c => c.RaceId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.Subrace).WithMany().HasForeignKey(c => c.SubraceId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.Alignment).WithMany().HasForeignKey(c => c.AlignmentId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.Background).WithMany().HasForeignKey(c => c.BackgroundId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<CharacterCurrency>(e =>
        {
            e.HasOne(cc => cc.Character).WithOne(c => c.Currency).HasForeignKey<CharacterCurrency>(cc => cc.CharacterId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Campaign>(e =>
        {
            e.HasOne(c => c.DM).WithMany().HasForeignKey(c => c.DMUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Encounter>(e =>
        {
            e.HasOne(enc => enc.Campaign).WithMany(c => c.Encounters).HasForeignKey(enc => enc.CampaignId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CampaignSession>(e =>
        {
            e.HasOne(s => s.Campaign).WithMany(c => c.Sessions).HasForeignKey(s => s.CampaignId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CampaignMember>(e =>
        {
            e.HasOne(m => m.Campaign).WithMany(c => c.Members).HasForeignKey(m => m.CampaignId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CampaignMember>(e =>
        {
            e.HasIndex(cm => new { cm.CampaignId, cm.UserId }).IsUnique();
            e.HasOne(cm => cm.Character).WithMany().HasForeignKey(cm => cm.CharacterId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Encounter>(e =>
        {
            e.HasOne(enc => enc.Session).WithMany().HasForeignKey(enc => enc.SessionId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<EncounterParticipant>(e =>
        {
            e.HasOne(ep => ep.Character).WithMany().HasForeignKey(ep => ep.CharacterId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(ep => ep.Monster).WithMany().HasForeignKey(ep => ep.MonsterId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<CharacterSpellSlot>(e =>
        {
            e.HasIndex(cs => new { cs.CharacterId, cs.SlotLevel }).IsUnique();
        });

        modelBuilder.Entity<SpellSlotProgression>(e =>
        {
            e.HasIndex(s => new { s.ClassId, s.Level }).IsUnique();
        });

        modelBuilder.Entity<CustomContentShare>(e =>
        {
            e.HasOne(s => s.CreatedBy).WithMany().HasForeignKey(s => s.CreatedByUserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(s => s.SharedWithUser).WithMany().HasForeignKey(s => s.SharedWithUserId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(s => s.SharedWithCampaign).WithMany().HasForeignKey(s => s.SharedWithCampaignId).OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(s => new { s.ContentType, s.ContentId, s.SharedWithUserId, s.SharedWithCampaignId }).IsUnique();
        });
    }
}
