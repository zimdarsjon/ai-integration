using Microsoft.EntityFrameworkCore;

file record MonsterActionSeed(
    string Name, string Description, ActionType ActionType,
    int? AttackBonus, string? ReachOrRange, string? HitDamageDice,
    string? DamageType, int? SaveDC, string? SaveAbility);

file record MonsterSeed(
    string Name, string Type, string Size, string Alignment,
    int AC, int HP, string HitDice, int Walk, decimal CR, int XP, int Prof,
    Dictionary<string, int> Scores,
    (string Name, string Desc)[] Traits,
    MonsterActionSeed[] Actions,
    string[] Immunities, string[] Resistances, string[] ConditionImmunities,
    bool IsLegendary);

public class DatabaseSeeder
{
    private readonly AppDbContext _db;

    public DatabaseSeeder(AppDbContext db) => _db = db;

    public async Task SeedAsync()
    {
        if (!await _db.AbilityScores.AnyAsync())
            await RunSeedAsync();
    }

    public async Task ReseedAsync()
    {
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.MigrateAsync();
        await RunSeedAsync();
    }

    private async Task RunSeedAsync()
    {
        await SeedAbilityScoresAsync();
        await SeedSkillsAsync();
        await SeedDamageTypesAsync();
        await SeedConditionsAsync();
        await SeedMagicSchoolsAsync();
        await SeedWeaponPropertiesAsync();
        await SeedCreatureSizesAsync();
        await SeedCreatureTypesAsync();
        await SeedLanguagesAsync();
        await SeedAlignmentsAsync();
        await SeedRacesAsync();
        await SeedClassesAsync();
        await SeedClassFeaturesAndSubclassesAsync();
        await SeedSpellsAsync();
        await SeedItemsAsync();
        await SeedMonstersAsync();
        await SeedBackgroundsAsync();
    }

    private async Task SeedAbilityScoresAsync()
    {
        _db.AbilityScores.AddRange(
            new AbilityScore { Name = "Strength", Abbreviation = "STR", Description = "Measures bodily power, athletic training, and the extent to which you can exert raw physical force." },
            new AbilityScore { Name = "Dexterity", Abbreviation = "DEX", Description = "Measures agility, reflexes, and balance." },
            new AbilityScore { Name = "Constitution", Abbreviation = "CON", Description = "Measures health, stamina, and vital force." },
            new AbilityScore { Name = "Intelligence", Abbreviation = "INT", Description = "Measures mental acuity, accuracy of recall, and the ability to reason." },
            new AbilityScore { Name = "Wisdom", Abbreviation = "WIS", Description = "Reflects how attuned you are to the world around you and represents perceptiveness and intuition." },
            new AbilityScore { Name = "Charisma", Abbreviation = "CHA", Description = "Measures your ability to interact effectively with others." }
        );
        await _db.SaveChangesAsync();
    }

    private async Task SeedSkillsAsync()
    {
        var scores = await _db.AbilityScores.ToDictionaryAsync(a => a.Abbreviation);
        _db.Skills.AddRange(
            new Skill { Name = "Athletics", AbilityScoreId = scores["STR"].Id },
            new Skill { Name = "Acrobatics", AbilityScoreId = scores["DEX"].Id },
            new Skill { Name = "Sleight of Hand", AbilityScoreId = scores["DEX"].Id },
            new Skill { Name = "Stealth", AbilityScoreId = scores["DEX"].Id },
            new Skill { Name = "Arcana", AbilityScoreId = scores["INT"].Id },
            new Skill { Name = "History", AbilityScoreId = scores["INT"].Id },
            new Skill { Name = "Investigation", AbilityScoreId = scores["INT"].Id },
            new Skill { Name = "Nature", AbilityScoreId = scores["INT"].Id },
            new Skill { Name = "Religion", AbilityScoreId = scores["INT"].Id },
            new Skill { Name = "Animal Handling", AbilityScoreId = scores["WIS"].Id },
            new Skill { Name = "Insight", AbilityScoreId = scores["WIS"].Id },
            new Skill { Name = "Medicine", AbilityScoreId = scores["WIS"].Id },
            new Skill { Name = "Perception", AbilityScoreId = scores["WIS"].Id },
            new Skill { Name = "Survival", AbilityScoreId = scores["WIS"].Id },
            new Skill { Name = "Deception", AbilityScoreId = scores["CHA"].Id },
            new Skill { Name = "Intimidation", AbilityScoreId = scores["CHA"].Id },
            new Skill { Name = "Performance", AbilityScoreId = scores["CHA"].Id },
            new Skill { Name = "Persuasion", AbilityScoreId = scores["CHA"].Id }
        );
        await _db.SaveChangesAsync();
    }

    private async Task SeedDamageTypesAsync()
    {
        _db.DamageTypes.AddRange(
            new DamageType { Name = "Acid", Description = "Corrosive liquid or gas." },
            new DamageType { Name = "Bludgeoning", Description = "Blunt force, falls, constriction." },
            new DamageType { Name = "Cold", Description = "Freezing ice or cold winds." },
            new DamageType { Name = "Fire", Description = "Flames and intense heat." },
            new DamageType { Name = "Force", Description = "Pure magical energy." },
            new DamageType { Name = "Lightning", Description = "Electric shock." },
            new DamageType { Name = "Necrotic", Description = "Sapping life force." },
            new DamageType { Name = "Piercing", Description = "Puncturing and impaling." },
            new DamageType { Name = "Poison", Description = "Venomous injury." },
            new DamageType { Name = "Psychic", Description = "Mental assault." },
            new DamageType { Name = "Radiant", Description = "Holy energy and searing light." },
            new DamageType { Name = "Slashing", Description = "Cutting and lacerating." },
            new DamageType { Name = "Thunder", Description = "Concussive sound." }
        );
        await _db.SaveChangesAsync();
    }

    private async Task SeedConditionsAsync()
    {
        _db.Conditions.AddRange(
            new Condition { Name = "Blinded", Description = "A blinded creature can't see and automatically fails any ability check that requires sight. Attack rolls against the creature have advantage, and the creature's attack rolls have disadvantage." },
            new Condition { Name = "Charmed", Description = "A charmed creature can't attack the charmer or target the charmer with harmful abilities or magical effects. The charmer has advantage on any ability check to interact socially with the creature." },
            new Condition { Name = "Deafened", Description = "A deafened creature can't hear and automatically fails any ability check that requires hearing." },
            new Condition { Name = "Frightened", Description = "A frightened creature has disadvantage on ability checks and attack rolls while the source of its fear is within line of sight. The creature can't willingly move closer to the source of its fear." },
            new Condition { Name = "Grappled", Description = "A grappled creature's speed becomes 0, and it can't benefit from any bonus to its speed. The condition ends if the grappler is incapacitated or if an effect removes the grappled creature from the reach of the grappler." },
            new Condition { Name = "Incapacitated", Description = "An incapacitated creature can't take actions or reactions." },
            new Condition { Name = "Invisible", Description = "An invisible creature is impossible to see without the aid of magic or a special sense. The creature's location can be detected by any noise it makes or tracks it leaves." },
            new Condition { Name = "Paralyzed", Description = "A paralyzed creature is incapacitated and can't move or speak. The creature automatically fails STR and DEX saving throws. Attack rolls against the creature have advantage. Any attack that hits the creature is a critical hit if the attacker is within 5 feet." },
            new Condition { Name = "Petrified", Description = "A petrified creature is transformed into a solid inanimate substance. It is incapacitated, can't move or speak, and is unaware of its surroundings." },
            new Condition { Name = "Poisoned", Description = "A poisoned creature has disadvantage on attack rolls and ability checks." },
            new Condition { Name = "Prone", Description = "A prone creature's only movement option is to crawl, unless it stands up. The creature has disadvantage on attack rolls. Attack rolls against the creature have advantage if the attacker is within 5 feet, otherwise disadvantage." },
            new Condition { Name = "Restrained", Description = "A restrained creature's speed becomes 0. Attack rolls against the creature have advantage, and the creature's attack rolls have disadvantage. The creature has disadvantage on DEX saving throws." },
            new Condition { Name = "Stunned", Description = "A stunned creature is incapacitated, can't move, and can speak only falteringly. The creature automatically fails STR and DEX saving throws. Attack rolls against the creature have advantage." },
            new Condition { Name = "Unconscious", Description = "An unconscious creature is incapacitated, can't move or speak, and is unaware of its surroundings. The creature drops whatever it's holding and falls prone." },
            new Condition { Name = "Exhaustion", Description = "Exhaustion is measured in six levels. Each level applies cumulative effects." }
        );
        await _db.SaveChangesAsync();
    }

    private async Task SeedMagicSchoolsAsync()
    {
        _db.MagicSchools.AddRange(
            new MagicSchool { Name = "Abjuration", Description = "Protective spells that create barriers, negate harmful effects, or banish creatures." },
            new MagicSchool { Name = "Conjuration", Description = "Spells that produce objects and creatures out of thin air." },
            new MagicSchool { Name = "Divination", Description = "Spells that enable you to learn secrets long forgotten, interpret the future, find hidden things, and foil deceptive spells." },
            new MagicSchool { Name = "Enchantment", Description = "Spells that affect the minds of others, influencing or controlling their behavior." },
            new MagicSchool { Name = "Evocation", Description = "Spells that manipulate magical energy to produce desired effects, including harmful blasts and protective shields." },
            new MagicSchool { Name = "Illusion", Description = "Spells that deceive the senses or minds of others." },
            new MagicSchool { Name = "Necromancy", Description = "Spells that manipulate the energies of life and death." },
            new MagicSchool { Name = "Transmutation", Description = "Spells that change the properties of a creature, object, or environment." }
        );
        await _db.SaveChangesAsync();
    }

    private async Task SeedWeaponPropertiesAsync()
    {
        _db.WeaponProperties.AddRange(
            new WeaponProperty { Name = "Ammunition", Description = "You can use a weapon that has the ammunition property to make a ranged attack only if you have ammunition to fire from the weapon." },
            new WeaponProperty { Name = "Finesse", Description = "When making an attack with a finesse weapon, you use your choice of your Strength or Dexterity modifier for the attack and damage rolls." },
            new WeaponProperty { Name = "Heavy", Description = "Small creatures have disadvantage on attack rolls with heavy weapons." },
            new WeaponProperty { Name = "Light", Description = "A light weapon is small and easy to handle, making it ideal for use when fighting with two weapons." },
            new WeaponProperty { Name = "Loading", Description = "You can fire only one piece of ammunition from it when you use an action, bonus action, or reaction to fire it." },
            new WeaponProperty { Name = "Range", Description = "A weapon that can be used to make a ranged attack has a range shown in parentheses after the ammunition or thrown property." },
            new WeaponProperty { Name = "Reach", Description = "This weapon adds 5 feet to your reach when you attack with it, as well as when determining your reach for opportunity attacks with it." },
            new WeaponProperty { Name = "Thrown", Description = "If a weapon has the thrown property, you can throw the weapon to make a ranged attack." },
            new WeaponProperty { Name = "Two-Handed", Description = "This weapon requires two hands when you attack with it." },
            new WeaponProperty { Name = "Versatile", Description = "This weapon can be used with one or two hands. A damage value in parentheses appears with the property — the damage when the weapon is used with two hands." },
            new WeaponProperty { Name = "Special", Description = "A weapon with the special property has unusual rules governing its use." },
            new WeaponProperty { Name = "Silvered", Description = "Some monsters are susceptible to silvered weapons." }
        );
        await _db.SaveChangesAsync();
    }

    private async Task SeedCreatureSizesAsync()
    {
        _db.CreatureSizes.AddRange(
            new CreatureSize { Name = "Tiny", HitDie = "d4" },
            new CreatureSize { Name = "Small", HitDie = "d6" },
            new CreatureSize { Name = "Medium", HitDie = "d8" },
            new CreatureSize { Name = "Large", HitDie = "d10" },
            new CreatureSize { Name = "Huge", HitDie = "d12" },
            new CreatureSize { Name = "Gargantuan", HitDie = "d20" }
        );
        await _db.SaveChangesAsync();
    }

    private async Task SeedCreatureTypesAsync()
    {
        _db.CreatureTypes.AddRange(
            new CreatureType { Name = "Aberration" },
            new CreatureType { Name = "Beast" },
            new CreatureType { Name = "Celestial" },
            new CreatureType { Name = "Construct" },
            new CreatureType { Name = "Dragon" },
            new CreatureType { Name = "Elemental" },
            new CreatureType { Name = "Fey" },
            new CreatureType { Name = "Fiend" },
            new CreatureType { Name = "Giant" },
            new CreatureType { Name = "Humanoid" },
            new CreatureType { Name = "Monstrosity" },
            new CreatureType { Name = "Ooze" },
            new CreatureType { Name = "Plant" },
            new CreatureType { Name = "Undead" }
        );
        await _db.SaveChangesAsync();
    }

    private async Task SeedLanguagesAsync()
    {
        _db.Languages.AddRange(
            new Language { Name = "Common", Script = "Common" },
            new Language { Name = "Dwarvish", Script = "Dwarvish" },
            new Language { Name = "Elvish", Script = "Elvish" },
            new Language { Name = "Giant", Script = "Dwarvish" },
            new Language { Name = "Gnomish", Script = "Dwarvish" },
            new Language { Name = "Goblin", Script = "Dwarvish" },
            new Language { Name = "Halfling", Script = "Common" },
            new Language { Name = "Orc", Script = "Dwarvish" },
            new Language { Name = "Abyssal", Script = "Infernal", IsExotic = true },
            new Language { Name = "Celestial", Script = "Celestial", IsExotic = true },
            new Language { Name = "Draconic", Script = "Draconic", IsExotic = true },
            new Language { Name = "Deep Speech", Script = "None", IsExotic = true },
            new Language { Name = "Infernal", Script = "Infernal", IsExotic = true },
            new Language { Name = "Primordial", Script = "Dwarvish", IsExotic = true },
            new Language { Name = "Sylvan", Script = "Elvish", IsExotic = true },
            new Language { Name = "Undercommon", Script = "Elvish", IsExotic = true }
        );
        await _db.SaveChangesAsync();
    }

    private async Task SeedAlignmentsAsync()
    {
        _db.Alignments.AddRange(
            new Alignment { Name = "Lawful Good", Abbreviation = "LG", Description = "Acts with compassion and duty." },
            new Alignment { Name = "Neutral Good", Abbreviation = "NG", Description = "Does the best good without bias toward order or chaos." },
            new Alignment { Name = "Chaotic Good", Abbreviation = "CG", Description = "Acts with a good heart but little respect for rules." },
            new Alignment { Name = "Lawful Neutral", Abbreviation = "LN", Description = "Acts in accordance with law, tradition, or personal code." },
            new Alignment { Name = "True Neutral", Abbreviation = "N", Description = "Avoids siding with good, evil, order, or chaos." },
            new Alignment { Name = "Chaotic Neutral", Abbreviation = "CN", Description = "Follows whims and values personal freedom above all." },
            new Alignment { Name = "Lawful Evil", Abbreviation = "LE", Description = "Takes what is wanted by using law, tradition, or order." },
            new Alignment { Name = "Neutral Evil", Abbreviation = "NE", Description = "Does whatever can be gotten away with, without compassion." },
            new Alignment { Name = "Chaotic Evil", Abbreviation = "CE", Description = "Acts with arbitrary violence, spurred by greed, hatred, or bloodlust." }
        );
        await _db.SaveChangesAsync();
    }

    private async Task SeedRacesAsync()
    {
        var sizes = await _db.CreatureSizes.ToDictionaryAsync(s => s.Name);
        var languages = await _db.Languages.ToDictionaryAsync(l => l.Name);
        var abilityScores = await _db.AbilityScores.ToDictionaryAsync(a => a.Abbreviation);
        var medium = sizes["Medium"];
        var small = sizes["Small"];

        // Dwarf
        var dwarf = new Race { Name = "Dwarf", Description = "Bold and hardy, dwarves are known as skilled warriors, miners, and workers of stone and metal.", Speed = 25, SizeId = medium.Id };
        _db.Races.Add(dwarf);
        await _db.SaveChangesAsync();
        _db.RaceAbilityBonuses.Add(new RaceAbilityBonus { RaceId = dwarf.Id, AbilityScoreId = abilityScores["CON"].Id, Bonus = 2 });
        _db.RaceTraits.AddRange(
            new RaceTrait { RaceId = dwarf.Id, Name = "Darkvision", Description = "Accustomed to life underground, you have superior vision in dark and dim conditions. You can see in dim light within 60 feet as if bright light, and in darkness as if dim light." },
            new RaceTrait { RaceId = dwarf.Id, Name = "Dwarven Resilience", Description = "You have advantage on saving throws against poison, and you have resistance against poison damage." },
            new RaceTrait { RaceId = dwarf.Id, Name = "Dwarven Combat Training", Description = "You have proficiency with the battleaxe, handaxe, light hammer, and warhammer." },
            new RaceTrait { RaceId = dwarf.Id, Name = "Stonecunning", Description = "Whenever you make an Intelligence (History) check related to the origin of stonework, you are considered proficient in the History skill and add double your proficiency bonus." }
        );
        _db.RaceLanguages.AddRange(
            new RaceLanguage { RaceId = dwarf.Id, LanguageId = languages["Common"].Id },
            new RaceLanguage { RaceId = dwarf.Id, LanguageId = languages["Dwarvish"].Id }
        );
        var hillDwarf = new Subrace { RaceId = dwarf.Id, Name = "Hill Dwarf", Description = "As a hill dwarf, you have keen senses, deep intuition, and remarkable resilience." };
        var mountainDwarf = new Subrace { RaceId = dwarf.Id, Name = "Mountain Dwarf", Description = "As a mountain dwarf, you're strong and hardy, accustomed to a difficult life in rugged terrain." };
        _db.Subraces.AddRange(hillDwarf, mountainDwarf);
        await _db.SaveChangesAsync();
        _db.SubraceAbilityBonuses.Add(new SubraceAbilityBonus { SubraceId = hillDwarf.Id, AbilityScoreId = abilityScores["WIS"].Id, Bonus = 1 });
        _db.SubraceTraits.Add(new SubraceTrait { SubraceId = hillDwarf.Id, Name = "Dwarven Toughness", Description = "Your hit point maximum increases by 1, and it increases by 1 every time you gain a level." });
        _db.SubraceAbilityBonuses.Add(new SubraceAbilityBonus { SubraceId = mountainDwarf.Id, AbilityScoreId = abilityScores["STR"].Id, Bonus = 2 });
        _db.SubraceTraits.Add(new SubraceTrait { SubraceId = mountainDwarf.Id, Name = "Dwarven Armor Training", Description = "You have proficiency with light and medium armor." });

        // Elf
        var elf = new Race { Name = "Elf", Description = "Elves are a magical people of otherworldly grace, living in the world but not entirely part of it.", Speed = 30, SizeId = medium.Id };
        _db.Races.Add(elf);
        await _db.SaveChangesAsync();
        _db.RaceAbilityBonuses.Add(new RaceAbilityBonus { RaceId = elf.Id, AbilityScoreId = abilityScores["DEX"].Id, Bonus = 2 });
        _db.RaceTraits.AddRange(
            new RaceTrait { RaceId = elf.Id, Name = "Darkvision", Description = "Accustomed to twilit forests and the night sky, you have superior vision in dark conditions. 60 ft." },
            new RaceTrait { RaceId = elf.Id, Name = "Keen Senses", Description = "You have proficiency in the Perception skill." },
            new RaceTrait { RaceId = elf.Id, Name = "Fey Ancestry", Description = "You have advantage on saving throws against being charmed, and magic can't put you to sleep." },
            new RaceTrait { RaceId = elf.Id, Name = "Trance", Description = "Elves don't need to sleep. Instead, they meditate deeply, remaining semiconscious, for 4 hours a day." }
        );
        _db.RaceLanguages.AddRange(
            new RaceLanguage { RaceId = elf.Id, LanguageId = languages["Common"].Id },
            new RaceLanguage { RaceId = elf.Id, LanguageId = languages["Elvish"].Id }
        );
        var highElf = new Subrace { RaceId = elf.Id, Name = "High Elf", Description = "As a high elf, you have a keen mind and a mastery of at least the basics of magic." };
        var woodElf = new Subrace { RaceId = elf.Id, Name = "Wood Elf", Description = "As a wood elf, you have keen senses and intuition, and your fleet feet carry you quickly." };
        _db.Subraces.AddRange(highElf, woodElf);
        await _db.SaveChangesAsync();
        _db.SubraceAbilityBonuses.Add(new SubraceAbilityBonus { SubraceId = highElf.Id, AbilityScoreId = abilityScores["INT"].Id, Bonus = 1 });
        _db.SubraceTraits.AddRange(
            new SubraceTrait { SubraceId = highElf.Id, Name = "Cantrip", Description = "You know one cantrip of your choice from the wizard spell list." },
            new SubraceTrait { SubraceId = highElf.Id, Name = "Extra Language", Description = "You can speak, read, and write one extra language of your choice." }
        );
        _db.SubraceAbilityBonuses.Add(new SubraceAbilityBonus { SubraceId = woodElf.Id, AbilityScoreId = abilityScores["WIS"].Id, Bonus = 1 });
        _db.SubraceTraits.AddRange(
            new SubraceTrait { SubraceId = woodElf.Id, Name = "Fleet of Foot", Description = "Your base walking speed increases to 35 feet." },
            new SubraceTrait { SubraceId = woodElf.Id, Name = "Mask of the Wild", Description = "You can attempt to hide even when you are only lightly obscured by foliage, heavy rain, falling snow, mist, and other natural phenomena." }
        );

        // Halfling
        var halfling = new Race { Name = "Halfling", Description = "The comforts of home are the goals of most halflings' lives.", Speed = 25, SizeId = small.Id };
        _db.Races.Add(halfling);
        await _db.SaveChangesAsync();
        _db.RaceAbilityBonuses.Add(new RaceAbilityBonus { RaceId = halfling.Id, AbilityScoreId = abilityScores["DEX"].Id, Bonus = 2 });
        _db.RaceTraits.AddRange(
            new RaceTrait { RaceId = halfling.Id, Name = "Lucky", Description = "When you roll a 1 on the d20 for an attack roll, ability check, or saving throw, you can reroll the die and must use the new roll." },
            new RaceTrait { RaceId = halfling.Id, Name = "Brave", Description = "You have advantage on saving throws against being frightened." },
            new RaceTrait { RaceId = halfling.Id, Name = "Halfling Nimbleness", Description = "You can move through the space of any creature that is of a size larger than yours." }
        );
        _db.RaceLanguages.AddRange(
            new RaceLanguage { RaceId = halfling.Id, LanguageId = languages["Common"].Id },
            new RaceLanguage { RaceId = halfling.Id, LanguageId = languages["Halfling"].Id }
        );
        var lightfoot = new Subrace { RaceId = halfling.Id, Name = "Lightfoot", Description = "As a lightfoot halfling, you can easily hide from notice." };
        var stout = new Subrace { RaceId = halfling.Id, Name = "Stout", Description = "As a stout halfling, you're hardier than average." };
        _db.Subraces.AddRange(lightfoot, stout);
        await _db.SaveChangesAsync();
        _db.SubraceAbilityBonuses.Add(new SubraceAbilityBonus { SubraceId = lightfoot.Id, AbilityScoreId = abilityScores["CHA"].Id, Bonus = 1 });
        _db.SubraceTraits.Add(new SubraceTrait { SubraceId = lightfoot.Id, Name = "Naturally Stealthy", Description = "You can attempt to hide even when you are obscured only by a creature that is at least one size larger than you." });
        _db.SubraceAbilityBonuses.Add(new SubraceAbilityBonus { SubraceId = stout.Id, AbilityScoreId = abilityScores["CON"].Id, Bonus = 1 });
        _db.SubraceTraits.Add(new SubraceTrait { SubraceId = stout.Id, Name = "Stout Resilience", Description = "You have advantage on saving throws against poison, and you have resistance against poison damage." });

        // Human
        var human = new Race { Name = "Human", Description = "Humans are the most adaptable and ambitious people among the common races.", Speed = 30, SizeId = medium.Id };
        _db.Races.Add(human);
        await _db.SaveChangesAsync();
        foreach (var score in abilityScores.Values)
            _db.RaceAbilityBonuses.Add(new RaceAbilityBonus { RaceId = human.Id, AbilityScoreId = score.Id, Bonus = 1 });
        _db.RaceTraits.Add(new RaceTrait { RaceId = human.Id, Name = "Extra Language", Description = "You can speak, read, and write one extra language of your choice." });
        _db.RaceLanguages.AddRange(
            new RaceLanguage { RaceId = human.Id, LanguageId = languages["Common"].Id }
        );

        // Dragonborn
        var dragonborn = new Race { Name = "Dragonborn", Description = "Dragonborn look very much like dragons standing erect in humanoid form.", Speed = 30, SizeId = medium.Id };
        _db.Races.Add(dragonborn);
        await _db.SaveChangesAsync();
        _db.RaceAbilityBonuses.AddRange(
            new RaceAbilityBonus { RaceId = dragonborn.Id, AbilityScoreId = abilityScores["STR"].Id, Bonus = 2 },
            new RaceAbilityBonus { RaceId = dragonborn.Id, AbilityScoreId = abilityScores["CHA"].Id, Bonus = 1 }
        );
        _db.RaceTraits.AddRange(
            new RaceTrait { RaceId = dragonborn.Id, Name = "Draconic Ancestry", Description = "You have draconic ancestry. Choose one type of dragon from the Draconic Ancestry table." },
            new RaceTrait { RaceId = dragonborn.Id, Name = "Breath Weapon", Description = "You can use your action to exhale destructive energy. Your draconic ancestry determines the size, shape, and damage type of the exhalation." },
            new RaceTrait { RaceId = dragonborn.Id, Name = "Damage Resistance", Description = "You have resistance to the damage type associated with your draconic ancestry." }
        );
        _db.RaceLanguages.AddRange(
            new RaceLanguage { RaceId = dragonborn.Id, LanguageId = languages["Common"].Id },
            new RaceLanguage { RaceId = dragonborn.Id, LanguageId = languages["Draconic"].Id }
        );

        // Gnome
        var gnome = new Race { Name = "Gnome", Description = "A gnome's energy and enthusiasm for living shines through every inch of his or her tiny body.", Speed = 25, SizeId = small.Id };
        _db.Races.Add(gnome);
        await _db.SaveChangesAsync();
        _db.RaceAbilityBonuses.Add(new RaceAbilityBonus { RaceId = gnome.Id, AbilityScoreId = abilityScores["INT"].Id, Bonus = 2 });
        _db.RaceTraits.AddRange(
            new RaceTrait { RaceId = gnome.Id, Name = "Darkvision", Description = "Accustomed to life underground, 60 ft darkvision." },
            new RaceTrait { RaceId = gnome.Id, Name = "Gnome Cunning", Description = "You have advantage on all Intelligence, Wisdom, and Charisma saving throws against magic." }
        );
        _db.RaceLanguages.AddRange(
            new RaceLanguage { RaceId = gnome.Id, LanguageId = languages["Common"].Id },
            new RaceLanguage { RaceId = gnome.Id, LanguageId = languages["Gnomish"].Id }
        );
        var forestGnome = new Subrace { RaceId = gnome.Id, Name = "Forest Gnome", Description = "As a forest gnome, you have a natural knack for illusion and inherent quickness and stealth." };
        var rockGnome = new Subrace { RaceId = gnome.Id, Name = "Rock Gnome", Description = "As a rock gnome, you have a natural inventiveness and hardiness beyond that of other gnomes." };
        _db.Subraces.AddRange(forestGnome, rockGnome);
        await _db.SaveChangesAsync();
        _db.SubraceAbilityBonuses.Add(new SubraceAbilityBonus { SubraceId = forestGnome.Id, AbilityScoreId = abilityScores["DEX"].Id, Bonus = 1 });
        _db.SubraceTraits.AddRange(
            new SubraceTrait { SubraceId = forestGnome.Id, Name = "Natural Illusionist", Description = "You know the minor illusion cantrip." },
            new SubraceTrait { SubraceId = forestGnome.Id, Name = "Speak with Small Beasts", Description = "Through sounds and gestures, you can communicate simple ideas with Small or smaller beasts." }
        );
        _db.SubraceAbilityBonuses.Add(new SubraceAbilityBonus { SubraceId = rockGnome.Id, AbilityScoreId = abilityScores["CON"].Id, Bonus = 1 });
        _db.SubraceTraits.AddRange(
            new SubraceTrait { SubraceId = rockGnome.Id, Name = "Artificer's Lore", Description = "Whenever you make an Intelligence (History) check related to magic items, alchemical objects, or technological devices, add twice your proficiency bonus." },
            new SubraceTrait { SubraceId = rockGnome.Id, Name = "Tinker", Description = "You have proficiency with artisan's tools (tinker's tools)." }
        );

        // Half-Elf
        var halfElf = new Race { Name = "Half-Elf", Description = "Half-elves combine what some say are the best qualities of their elf and human parents.", Speed = 30, SizeId = medium.Id };
        _db.Races.Add(halfElf);
        await _db.SaveChangesAsync();
        _db.RaceAbilityBonuses.Add(new RaceAbilityBonus { RaceId = halfElf.Id, AbilityScoreId = abilityScores["CHA"].Id, Bonus = 2 });
        _db.RaceTraits.AddRange(
            new RaceTrait { RaceId = halfElf.Id, Name = "Darkvision", Description = "60 ft darkvision thanks to your elf blood." },
            new RaceTrait { RaceId = halfElf.Id, Name = "Fey Ancestry", Description = "You have advantage on saving throws against being charmed, and magic can't put you to sleep." },
            new RaceTrait { RaceId = halfElf.Id, Name = "Skill Versatility", Description = "You gain proficiency in two skills of your choice." },
            new RaceTrait { RaceId = halfElf.Id, Name = "Ability Score Increase (Flexible)", Description = "Two other ability scores of your choice each increase by 1." }
        );
        _db.RaceLanguages.AddRange(
            new RaceLanguage { RaceId = halfElf.Id, LanguageId = languages["Common"].Id },
            new RaceLanguage { RaceId = halfElf.Id, LanguageId = languages["Elvish"].Id }
        );

        // Half-Orc
        var halfOrc = new Race { Name = "Half-Orc", Description = "Half-orcs' grayish pigmentation, sloping foreheads, jutting jaws, prominent teeth, and towering builds make their orcish heritage plain for all to see.", Speed = 30, SizeId = medium.Id };
        _db.Races.Add(halfOrc);
        await _db.SaveChangesAsync();
        _db.RaceAbilityBonuses.AddRange(
            new RaceAbilityBonus { RaceId = halfOrc.Id, AbilityScoreId = abilityScores["STR"].Id, Bonus = 2 },
            new RaceAbilityBonus { RaceId = halfOrc.Id, AbilityScoreId = abilityScores["CON"].Id, Bonus = 1 }
        );
        _db.RaceTraits.AddRange(
            new RaceTrait { RaceId = halfOrc.Id, Name = "Darkvision", Description = "60 ft darkvision." },
            new RaceTrait { RaceId = halfOrc.Id, Name = "Menacing", Description = "You gain proficiency in the Intimidation skill." },
            new RaceTrait { RaceId = halfOrc.Id, Name = "Relentless Endurance", Description = "When you are reduced to 0 hit points but not killed outright, you can drop to 1 hit point instead. Once you use this trait, you can't do so again until you finish a long rest." },
            new RaceTrait { RaceId = halfOrc.Id, Name = "Savage Attacks", Description = "When you score a critical hit with a melee weapon attack, you can roll one of the weapon's damage dice one additional time and add it to the extra damage of the critical hit." }
        );
        _db.RaceLanguages.AddRange(
            new RaceLanguage { RaceId = halfOrc.Id, LanguageId = languages["Common"].Id },
            new RaceLanguage { RaceId = halfOrc.Id, LanguageId = languages["Orc"].Id }
        );

        // Tiefling
        var tiefling = new Race { Name = "Tiefling", Description = "Tieflings are derived from human bloodlines, and in the broadest possible sense, they still look human. However, their infernal heritage has left a clear imprint on their appearance.", Speed = 30, SizeId = medium.Id };
        _db.Races.Add(tiefling);
        await _db.SaveChangesAsync();
        _db.RaceAbilityBonuses.AddRange(
            new RaceAbilityBonus { RaceId = tiefling.Id, AbilityScoreId = abilityScores["INT"].Id, Bonus = 1 },
            new RaceAbilityBonus { RaceId = tiefling.Id, AbilityScoreId = abilityScores["CHA"].Id, Bonus = 2 }
        );
        _db.RaceTraits.AddRange(
            new RaceTrait { RaceId = tiefling.Id, Name = "Darkvision", Description = "60 ft darkvision." },
            new RaceTrait { RaceId = tiefling.Id, Name = "Hellish Resistance", Description = "You have resistance to fire damage." },
            new RaceTrait { RaceId = tiefling.Id, Name = "Infernal Legacy", Description = "You know the thaumaturgy cantrip. At 3rd level, you can cast hellish rebuke as a 2nd-level spell. At 5th level, you can cast darkness." }
        );
        _db.RaceLanguages.AddRange(
            new RaceLanguage { RaceId = tiefling.Id, LanguageId = languages["Common"].Id },
            new RaceLanguage { RaceId = tiefling.Id, LanguageId = languages["Infernal"].Id }
        );

        await _db.SaveChangesAsync();
    }

    private async Task SeedClassesAsync()
    {
        var scores = await _db.AbilityScores.ToDictionaryAsync(a => a.Abbreviation);

        static int[] FullCasterSlots(int level) => level switch
        {
            1 => [2, 0, 0, 0, 0, 0, 0, 0, 0],
            2 => [3, 0, 0, 0, 0, 0, 0, 0, 0],
            3 => [4, 2, 0, 0, 0, 0, 0, 0, 0],
            4 => [4, 3, 0, 0, 0, 0, 0, 0, 0],
            5 => [4, 3, 2, 0, 0, 0, 0, 0, 0],
            6 => [4, 3, 3, 0, 0, 0, 0, 0, 0],
            7 => [4, 3, 3, 1, 0, 0, 0, 0, 0],
            8 => [4, 3, 3, 2, 0, 0, 0, 0, 0],
            9 => [4, 3, 3, 3, 1, 0, 0, 0, 0],
            10 => [4, 3, 3, 3, 2, 0, 0, 0, 0],
            11 => [4, 3, 3, 3, 2, 1, 0, 0, 0],
            12 => [4, 3, 3, 3, 2, 1, 0, 0, 0],
            13 => [4, 3, 3, 3, 2, 1, 1, 0, 0],
            14 => [4, 3, 3, 3, 2, 1, 1, 0, 0],
            15 => [4, 3, 3, 3, 2, 1, 1, 1, 0],
            16 => [4, 3, 3, 3, 2, 1, 1, 1, 0],
            17 => [4, 3, 3, 3, 2, 1, 1, 1, 1],
            18 => [4, 3, 3, 3, 3, 1, 1, 1, 1],
            19 => [4, 3, 3, 3, 3, 2, 1, 1, 1],
            _ =>  [4, 3, 3, 3, 3, 2, 2, 1, 1]
        };

        static int[] HalfCasterSlots(int level) => level switch
        {
            1 => [0, 0, 0, 0, 0, 0, 0, 0, 0],
            2 => [2, 0, 0, 0, 0, 0, 0, 0, 0],
            3 => [3, 0, 0, 0, 0, 0, 0, 0, 0],
            4 => [3, 0, 0, 0, 0, 0, 0, 0, 0],
            5 => [4, 2, 0, 0, 0, 0, 0, 0, 0],
            6 => [4, 2, 0, 0, 0, 0, 0, 0, 0],
            7 => [4, 3, 0, 0, 0, 0, 0, 0, 0],
            8 => [4, 3, 0, 0, 0, 0, 0, 0, 0],
            9 => [4, 3, 2, 0, 0, 0, 0, 0, 0],
            10 => [4, 3, 2, 0, 0, 0, 0, 0, 0],
            11 => [4, 3, 3, 0, 0, 0, 0, 0, 0],
            12 => [4, 3, 3, 0, 0, 0, 0, 0, 0],
            13 => [4, 3, 3, 1, 0, 0, 0, 0, 0],
            14 => [4, 3, 3, 1, 0, 0, 0, 0, 0],
            15 => [4, 3, 3, 2, 0, 0, 0, 0, 0],
            16 => [4, 3, 3, 2, 0, 0, 0, 0, 0],
            17 => [4, 3, 3, 3, 1, 0, 0, 0, 0],
            18 => [4, 3, 3, 3, 1, 0, 0, 0, 0],
            19 => [4, 3, 3, 3, 2, 0, 0, 0, 0],
            _ =>  [4, 3, 3, 3, 2, 0, 0, 0, 0]
        };

        static int[] WarlockSlots(int level) => level switch
        {
            1 => [1, 0, 0, 0, 0, 0, 0, 0, 0],
            2 => [2, 0, 0, 0, 0, 0, 0, 0, 0],
            3 => [0, 2, 0, 0, 0, 0, 0, 0, 0],
            4 => [0, 2, 0, 0, 0, 0, 0, 0, 0],
            5 => [0, 0, 2, 0, 0, 0, 0, 0, 0],
            6 => [0, 0, 2, 0, 0, 0, 0, 0, 0],
            7 => [0, 0, 0, 2, 0, 0, 0, 0, 0],
            8 => [0, 0, 0, 2, 0, 0, 0, 0, 0],
            9 => [0, 0, 0, 0, 2, 0, 0, 0, 0],
            _ =>  [0, 0, 0, 0, 3, 0, 0, 0, 0]
        };

        // Returns (cantripsKnown, spellsKnown, startingSpells) per class level.
        // spellsKnown = null  → class uses prepared-spell formula (ability mod + level) for daily use.
        // startingSpells      → character-creation selection limit; null = same as spellsKnown / prepared formula.
        //                       Wizard is the key exception: spellbook size at creation = 6 + (level-1)*2,
        //                       which differs from the prepared formula (INT mod + level).
        static (int cantrips, int? spellsKnown, int? startingSpells) SpellLimits(string className, int level) => className switch
        {
            "Bard"    => (level switch { <= 3 => 2, <= 9 => 3, _ => 4 },
                          level switch { 1=>2,2=>3,3=>4,4=>5,5=>6,6=>7,7=>8,8=>9,9=>10,10=>10,11=>11,12=>11,13=>12,14=>12,15=>13,16=>13,17=>14,18=>14,19=>15,_=>15 },
                          null),
            "Cleric"  => (level switch { <= 3 => 3, <= 9 => 4, _ => 5 }, null, null),
            "Druid"   => (level switch { <= 3 => 2, <= 9 => 3, _ => 4 }, null, null),
            "Paladin" => (0, null, null),
            "Ranger"  => (0,
                          level switch { 1=>2,2=>3,3=>3,4=>4,5=>4,6=>5,7=>5,8=>6,9=>6,10=>7,11=>7,12=>8,13=>8,14=>9,15=>9,16=>10,17=>10,18=>11,19=>11,_=>11 },
                          null),
            "Sorcerer"=> (level switch { <= 3 => 4, <= 9 => 5, _ => 6 },
                          level switch { 1=>2,2=>3,3=>4,4=>5,5=>6,6=>7,7=>8,8=>9,9=>10,10=>11,11=>12,12=>12,13=>13,14=>13,15=>14,16=>14,17=>15,18=>15,_=>15 },
                          null),
            "Warlock" => (level switch { <= 3 => 2, <= 9 => 3, _ => 4 },
                          level switch { 1=>2,2=>3,3=>4,4=>5,5=>6,6=>7,7=>8,8=>9,9=>10,10=>10,11=>11,12=>11,13=>12,14=>12,15=>13,16=>13,17=>14,18=>14,19=>15,_=>15 },
                          null),
            // Wizard: spellsKnown = null (prepared formula for daily use), startingSpells = spellbook at creation
            "Wizard"  => (level switch { <= 3 => 3, <= 9 => 4, _ => 5 }, null, 6 + (level - 1) * 2),
            _         => (0, null, null)
        };

        var classData = new[]
        {
            new { Name = "Barbarian", Desc = "A fierce warrior of primitive background who can enter a battle rage.", HitDie = 12, STAbils = new[]{"STR","CON"}, CastType = CasterType.None, SpellAbil = (string?)null },
            new { Name = "Bard", Desc = "An inspiring magician whose power echoes the music of creation.", HitDie = 8, STAbils = new[]{"DEX","CHA"}, CastType = CasterType.Full, SpellAbil = (string?)"CHA" },
            new { Name = "Cleric", Desc = "A priestly champion who wields divine magic in service of a higher power.", HitDie = 8, STAbils = new[]{"WIS","CHA"}, CastType = CasterType.Full, SpellAbil = (string?)"WIS" },
            new { Name = "Druid", Desc = "A priest of the Old Faith, wielding the powers of nature and adopting animal forms.", HitDie = 8, STAbils = new[]{"INT","WIS"}, CastType = CasterType.Full, SpellAbil = (string?)"WIS" },
            new { Name = "Fighter", Desc = "A master of martial combat, skilled with a variety of weapons and armor.", HitDie = 10, STAbils = new[]{"STR","CON"}, CastType = CasterType.None, SpellAbil = (string?)null },
            new { Name = "Monk", Desc = "A master of martial arts, harnessing the power of the body in pursuit of physical and spiritual perfection.", HitDie = 8, STAbils = new[]{"STR","DEX"}, CastType = CasterType.None, SpellAbil = (string?)null },
            new { Name = "Paladin", Desc = "A holy warrior bound to a sacred oath.", HitDie = 10, STAbils = new[]{"WIS","CHA"}, CastType = CasterType.Half, SpellAbil = (string?)"CHA" },
            new { Name = "Ranger", Desc = "A warrior who uses martial prowess and nature magic to combat threats on the edges of civilization.", HitDie = 10, STAbils = new[]{"STR","DEX"}, CastType = CasterType.Half, SpellAbil = (string?)"WIS" },
            new { Name = "Rogue", Desc = "A scoundrel who uses stealth and trickery to overcome obstacles and enemies.", HitDie = 8, STAbils = new[]{"DEX","INT"}, CastType = CasterType.None, SpellAbil = (string?)null },
            new { Name = "Sorcerer", Desc = "A spellcaster who draws on inherent magic from a gift or bloodline.", HitDie = 6, STAbils = new[]{"CON","CHA"}, CastType = CasterType.Full, SpellAbil = (string?)"CHA" },
            new { Name = "Warlock", Desc = "A wielder of magic that is derived from a bargain with an extraplanar entity.", HitDie = 8, STAbils = new[]{"WIS","CHA"}, CastType = CasterType.Warlock, SpellAbil = (string?)"CHA" },
            new { Name = "Wizard", Desc = "A scholarly magic-user capable of manipulating the structures of reality.", HitDie = 6, STAbils = new[]{"INT","WIS"}, CastType = CasterType.Full, SpellAbil = (string?)"INT" }
        };

        // D&D 5e skill choices per class: (numberOfChoices, skill names[])
        static (int count, string[] skills) ClassSkillChoices(string className) => className switch
        {
            "Barbarian" => (2, ["Animal Handling", "Athletics", "Intimidation", "Nature", "Perception", "Survival"]),
            "Bard"      => (3, ["Athletics", "Acrobatics", "Sleight of Hand", "Stealth", "Arcana", "History", "Investigation", "Nature", "Religion", "Animal Handling", "Insight", "Medicine", "Perception", "Survival", "Deception", "Intimidation", "Performance", "Persuasion"]),
            "Cleric"    => (2, ["History", "Insight", "Medicine", "Persuasion", "Religion"]),
            "Druid"     => (2, ["Arcana", "Animal Handling", "Insight", "Medicine", "Nature", "Perception", "Religion", "Survival"]),
            "Fighter"   => (2, ["Acrobatics", "Animal Handling", "Athletics", "History", "Insight", "Intimidation", "Perception", "Survival"]),
            "Monk"      => (2, ["Acrobatics", "Athletics", "History", "Insight", "Religion", "Stealth"]),
            "Paladin"   => (2, ["Athletics", "Insight", "Intimidation", "Medicine", "Persuasion", "Religion"]),
            "Ranger"    => (3, ["Animal Handling", "Athletics", "Insight", "Investigation", "Nature", "Perception", "Stealth", "Survival"]),
            "Rogue"     => (4, ["Acrobatics", "Athletics", "Deception", "Insight", "Intimidation", "Investigation", "Perception", "Performance", "Persuasion", "Sleight of Hand", "Stealth"]),
            "Sorcerer"  => (2, ["Arcana", "Deception", "Insight", "Intimidation", "Persuasion", "Religion"]),
            "Warlock"   => (2, ["Arcana", "Deception", "History", "Intimidation", "Investigation", "Nature", "Religion"]),
            "Wizard"    => (2, ["Arcana", "History", "Insight", "Investigation", "Medicine", "Religion"]),
            _           => (0, [])
        };

        var allSkills = await _db.Skills.ToDictionaryAsync(s => s.Name);

        foreach (var cd in classData)
        {
            var dndClass = new DndClass
            {
                Name = cd.Name, Description = cd.Desc, HitDie = cd.HitDie,
                CasterType = cd.CastType,
                SpellcastingAbilityScoreId = cd.SpellAbil != null ? scores[cd.SpellAbil].Id : null
            };
            _db.Classes.Add(dndClass);
            await _db.SaveChangesAsync();

            foreach (var st in cd.STAbils)
                _db.ClassSavingThrows.Add(new ClassSavingThrow { ClassId = dndClass.Id, AbilityScoreId = scores[st].Id });

            var (choiceCount, choiceSkills) = ClassSkillChoices(cd.Name);
            if (choiceCount > 0)
            {
                foreach (var skillName in choiceSkills)
                {
                    if (allSkills.TryGetValue(skillName, out var skill))
                        _db.ClassSkillChoices.Add(new ClassSkillChoice
                        {
                            ClassId = dndClass.Id,
                            SkillId = skill.Id,
                            NumberOfChoices = choiceCount
                        });
                }
            }

            if (cd.CastType != CasterType.None)
            {
                for (int lvl = 1; lvl <= 20; lvl++)
                {
                    int[] slots = cd.CastType switch
                    {
                        CasterType.Full => FullCasterSlots(lvl),
                        CasterType.Half => HalfCasterSlots(lvl),
                        CasterType.Warlock => WarlockSlots(lvl),
                        _ => new int[9]
                    };
                    _db.SpellSlotProgressions.Add(new SpellSlotProgression
                    {
                        ClassId = dndClass.Id, Level = lvl,
                        Slot1 = slots[0], Slot2 = slots[1], Slot3 = slots[2],
                        Slot4 = slots[3], Slot5 = slots[4], Slot6 = slots[5],
                        Slot7 = slots[6], Slot8 = slots[7], Slot9 = slots[8]
                    });

                    var (cantrips, spellsKnown, startingSpells) = SpellLimits(cd.Name, lvl);
                    _db.SpellLimitProgressions.Add(new SpellLimitProgression
                    {
                        ClassId = dndClass.Id, Level = lvl,
                        CantripsKnown = cantrips,
                        SpellsKnown = spellsKnown,
                        StartingSpells = startingSpells
                    });
                }
            }

            await _db.SaveChangesAsync();
        }
    }

    private async Task SeedClassFeaturesAndSubclassesAsync()
    {
        var classes = await _db.Classes.ToDictionaryAsync(c => c.Name);

        // ── Class features ───────────────────────────────────────────────────────
        var features = new List<(string cls, int lvl, string name, string desc)>
        {
            // Barbarian
            ("Barbarian",  1, "Rage", "In battle, you fight with primal ferocity. On your turn, you can enter a rage as a bonus action. While raging, you have advantage on STR checks and saving throws, deal bonus damage on melee STR attacks, and have resistance to bludgeoning, piercing, and slashing damage."),
            ("Barbarian",  1, "Unarmored Defense", "While you are not wearing any armor, your AC equals 10 + your DEX modifier + your CON modifier."),
            ("Barbarian",  2, "Reckless Attack", "When you make your first attack on your turn, you can decide to attack recklessly. Doing so gives you advantage on melee weapon attack rolls during this turn, but attack rolls against you have advantage until your next turn."),
            ("Barbarian",  2, "Danger Sense", "You have an uncanny sense of when things nearby aren't as they should be, giving you an edge when you dodge away from danger. You have advantage on DEX saving throws against effects that you can see."),
            ("Barbarian",  4, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Barbarian",  5, "Extra Attack", "You can attack twice, instead of once, whenever you take the Attack action on your turn."),
            ("Barbarian",  5, "Fast Movement", "Your speed increases by 10 feet while you aren't wearing heavy armor."),
            ("Barbarian",  7, "Feral Instinct", "Your instincts are so honed that you have advantage on initiative rolls."),
            ("Barbarian",  9, "Brutal Critical", "You can roll one additional weapon damage die when determining extra damage for a critical hit with a melee attack."),
            ("Barbarian", 11, "Relentless Rage", "Your rage can keep you fighting despite grievous wounds. If you drop to 0 hit points while you're raging and don't die outright, you can make a DC 10 CON saving throw to drop to 1 hit point instead."),
            ("Barbarian", 20, "Primal Champion", "You embody the power of the wilds. Your STR and CON scores each increase by 4. Your maximum for those scores is now 24."),

            // Bard
            ("Bard",  1, "Bardic Inspiration", "You can inspire others through stirring words or music. A creature that has a Bardic Inspiration die can roll it and add the number rolled to one ability check, attack roll, or saving throw."),
            ("Bard",  1, "Spellcasting", "You have learned to untangle and reshape the fabric of reality in harmony with your wishes and music."),
            ("Bard",  2, "Jack of All Trades", "Starting at 2nd level, you can add half your proficiency bonus, rounded down, to any ability check you make that doesn't already include your proficiency bonus."),
            ("Bard",  2, "Song of Rest", "You can use soothing music or oration to help revitalize your wounded allies during a short rest."),
            ("Bard",  4, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Bard",  5, "Font of Inspiration", "Beginning when you reach 5th level, you regain all your expended uses of Bardic Inspiration when you finish a short or long rest."),
            ("Bard",  6, "Countercharm", "At 6th level, you gain the ability to use musical notes or words of power to disrupt mind-influencing effects."),
            ("Bard", 10, "Magical Secrets", "By 10th level, you have plundered magical knowledge from a wide spectrum of disciplines. You learn two spells of your choice from any class."),
            ("Bard", 20, "Superior Inspiration", "At 20th level, when you roll initiative and have no uses of Bardic Inspiration left, you regain one use."),

            // Cleric
            ("Cleric",  1, "Spellcasting", "As a conduit for divine power, you can cast cleric spells. You prepare the list of cleric spells that are available for you to cast each day."),
            ("Cleric",  1, "Divine Domain", "Choose one domain related to your deity. Your choice grants you domain spells and other features as you gain levels."),
            ("Cleric",  2, "Channel Divinity", "At 2nd level, you gain the ability to channel divine energy directly from your deity. You can use Channel Divinity to fuel magical effects. When you use your Channel Divinity, you choose which effect to create."),
            ("Cleric",  2, "Channel Divinity: Turn Undead", "As an action, you present your holy symbol and speak a prayer censuring the undead. Each undead within 30 feet that can see or hear you must make a WIS saving throw."),
            ("Cleric",  4, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Cleric",  5, "Destroy Undead", "Starting at 5th level, when an undead fails its saving throw against your Turn Undead feature, the creature is instantly destroyed if its challenge rating is at or below a certain threshold."),
            ("Cleric", 10, "Divine Intervention", "Beginning at 10th level, you can call on your deity to intervene on your behalf when your need is great."),
            ("Cleric", 20, "Divine Intervention Improvement", "At 20th level, your call for intervention succeeds automatically, no roll required."),

            // Druid
            ("Druid",  1, "Druidic", "You know Druidic, the secret language of druids. You can speak the language and use it to leave hidden messages."),
            ("Druid",  1, "Spellcasting", "Drawing on the divine essence of nature itself, you can cast spells to shape that essence to your will."),
            ("Druid",  2, "Wild Shape", "Starting at 2nd level, you can use your action to magically assume the shape of a beast that you have seen before. You can use this feature twice, regaining expended uses when you finish a short or long rest. Your druid level determines the beasts you can transform into."),
            ("Druid",  2, "Druid Circle", "At 2nd level, you choose to identify with a circle of druids. Your choice grants you features at 2nd level and again at 6th, 10th, and 14th level."),
            ("Druid",  4, "Wild Shape Improvement", "You can now transform into a beast with a swimming speed."),
            ("Druid",  4, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Druid",  8, "Wild Shape Improvement", "You can now transform into a beast with a flying speed."),
            ("Druid",  8, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Druid", 18, "Timeless Body", "Starting at 18th level, the primal magic that you wield causes you to age more slowly. For every 10 years that pass, your body ages only 1 year."),
            ("Druid", 18, "Beast Spells", "Beginning at 18th level, you can cast many of your druid spells in any shape you assume using Wild Shape."),
            ("Druid", 20, "Archdruid", "At 20th level, you can use your Wild Shape an unlimited number of times."),

            // Fighter
            ("Fighter",  1, "Fighting Style", "You adopt a particular style of fighting as your specialty. Choose one of the following options: Archery, Defense, Dueling, Great Weapon Fighting, Protection, or Two-Weapon Fighting."),
            ("Fighter",  1, "Second Wind", "You have a limited well of stamina that you can draw on to protect yourself from harm. On your turn, you can use a bonus action to regain hit points equal to 1d10 + your fighter level."),
            ("Fighter",  2, "Action Surge", "Starting at 2nd level, you can push yourself beyond your normal limits for a moment. On your turn, you can take one additional action. Once you use this feature, you must finish a short or long rest before you can use it again."),
            ("Fighter",  4, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Fighter",  5, "Extra Attack", "Beginning at 5th level, you can attack twice, instead of once, whenever you take the Attack action on your turn."),
            ("Fighter",  9, "Indomitable", "Beginning at 9th level, you can reroll a saving throw that you fail. If you do so, you must use the new roll."),
            ("Fighter", 11, "Extra Attack (2)", "At 11th level, you can attack three times whenever you take the Attack action on your turn."),
            ("Fighter", 20, "Extra Attack (3)", "At 20th level, you can attack four times whenever you take the Attack action on your turn."),

            // Monk
            ("Monk",  1, "Unarmored Defense", "While you are wearing no armor and not wielding a shield, your AC equals 10 + your DEX modifier + your WIS modifier."),
            ("Monk",  1, "Martial Arts", "Your practice of martial arts gives you mastery of combat styles that use unarmed strikes and monk weapons. You gain the following benefits: use DEX instead of STR, roll a d4 for unarmed strikes, and can make an unarmed strike as a bonus action."),
            ("Monk",  2, "Ki", "Starting at 2nd level, your training allows you to harness the mystic energy of ki. Your access to this energy is represented by a number of ki points equal to your monk level."),
            ("Monk",  2, "Unarmored Movement", "Starting at 2nd level, your speed increases by 10 feet while you are not wearing armor or wielding a shield."),
            ("Monk",  4, "Slow Fall", "Beginning at 4th level, you can use your reaction when you fall to reduce any falling damage you take by an amount equal to five times your monk level."),
            ("Monk",  4, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Monk",  5, "Extra Attack", "Beginning at 5th level, you can attack twice, instead of once, whenever you take the Attack action on your turn."),
            ("Monk",  5, "Stunning Strike", "Starting at 5th level, you can interfere with the flow of ki in an opponent's body. When you hit another creature with a melee weapon attack, you can spend 1 ki point to attempt a stunning strike."),
            ("Monk",  6, "Ki-Empowered Strikes", "Starting at 6th level, your unarmed strikes count as magical for the purpose of overcoming resistance and immunity to nonmagical attacks and damage."),
            ("Monk",  7, "Evasion", "At 7th level, your instinctive agility lets you dodge out of the way of certain area effects. When you are subjected to an effect that allows a DEX saving throw to take only half damage, you take no damage on a success and half damage on a failure."),
            ("Monk", 20, "Perfect Self", "At 20th level, when you roll for initiative and have no ki points remaining, you regain 4 ki points."),

            // Paladin
            ("Paladin",  1, "Divine Sense", "The presence of strong evil registers on your senses like a noxious odor, and powerful good rings like heavenly music in your ears. As an action, you can open your awareness to detect such forces until the end of your next turn."),
            ("Paladin",  1, "Lay on Hands", "Your blessed touch can heal wounds. You have a pool of healing power that replenishes when you take a long rest. With that pool, you can restore a total number of hit points equal to your paladin level × 5."),
            ("Paladin",  2, "Fighting Style", "At 2nd level, you adopt a style of fighting as your specialty."),
            ("Paladin",  2, "Spellcasting", "By 2nd level, you have learned to draw on divine magic through meditation and prayer to cast spells as a paladin."),
            ("Paladin",  2, "Divine Smite", "Starting at 2nd level, when you hit a creature with a melee weapon attack, you can expend one spell slot to deal radiant damage to the target."),
            ("Paladin",  3, "Divine Health", "By 3rd level, the divine magic flowing through you makes you immune to disease."),
            ("Paladin",  4, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Paladin",  5, "Extra Attack", "Beginning at 5th level, you can attack twice, instead of once, whenever you take the Attack action on your turn."),
            ("Paladin",  6, "Aura of Protection", "Starting at 6th level, whenever you or a friendly creature within 10 feet of you must make a saving throw, the creature gains a bonus to the saving throw equal to your CHA modifier."),
            ("Paladin", 20, "Sacred Weapon", "At 20th level, as an action, you can imbue one weapon that you are holding with positive energy."),

            // Ranger
            ("Ranger",  1, "Favored Enemy", "Beginning at 1st level, you have significant experience studying, tracking, hunting, and even talking to a certain type of enemy. Choose a type of favored enemy: aberrations, beasts, celestials, constructs, dragons, elementals, fey, fiends, giants, monstrosities, oozes, plants, or undead."),
            ("Ranger",  1, "Natural Explorer", "You are particularly familiar with one type of natural environment and are adept at traveling and surviving in such regions."),
            ("Ranger",  2, "Fighting Style", "At 2nd level, you adopt a particular style of fighting as your specialty."),
            ("Ranger",  2, "Spellcasting", "By the time you reach 2nd level, you have learned to use the magical essence of nature to cast spells."),
            ("Ranger",  4, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Ranger",  5, "Extra Attack", "Beginning at 5th level, you can attack twice whenever you take the Attack action on your turn."),
            ("Ranger",  8, "Land's Stride", "Starting at 8th level, moving through nonmagical difficult terrain costs you no extra movement."),
            ("Ranger", 14, "Vanish", "Starting at 14th level, you can use the Hide action as a bonus action on your turn."),
            ("Ranger", 20, "Foe Slayer", "At 20th level, you become an unparalleled hunter. Once on each of your turns, you can add your WIS modifier to the attack roll or the damage roll of an attack you make."),

            // Rogue
            ("Rogue",  1, "Expertise", "At 1st level, choose two of your skill proficiencies. Your proficiency bonus is doubled for any ability check you make that uses either of the chosen proficiencies."),
            ("Rogue",  1, "Sneak Attack", "Beginning at 1st level, you know how to strike subtly and exploit a foe's distraction. Once per turn, you can deal an extra 1d6 damage to one creature you hit with an attack if you have advantage on the attack roll."),
            ("Rogue",  1, "Thieves' Cant", "During your rogue training you learned thieves' cant, a secret mix of dialect, jargon, and code that allows you to hide messages in seemingly normal conversation."),
            ("Rogue",  2, "Cunning Action", "Starting at 2nd level, your quick thinking and agility allow you to move and act quickly. You can take a bonus action on each of your turns to take the Dash, Disengage, or Hide action."),
            ("Rogue",  4, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Rogue",  5, "Uncanny Dodge", "Starting at 5th level, when an attacker that you can see hits you with an attack, you can use your reaction to halve the attack's damage against you."),
            ("Rogue",  7, "Evasion", "Beginning at 7th level, you can nimbly dodge out of the way of certain area effects."),
            ("Rogue", 11, "Reliable Talent", "By 11th level, you have refined your chosen skills until they approach perfection. Whenever you make an ability check that lets you add your proficiency bonus, you can treat a d20 roll of 9 or lower as a 10."),
            ("Rogue", 20, "Stroke of Luck", "At 20th level, you have an uncanny knack for succeeding when you need to. If your attack misses a target within range, you can turn the miss into a hit. Alternatively, if you fail an ability check, you can treat the d20 roll as a 20."),

            // Sorcerer
            ("Sorcerer",  1, "Spellcasting", "An event in your past, or in the life of a parent or ancestor, left an indelible mark on you, infusing you with arcane magic."),
            ("Sorcerer",  2, "Font of Magic", "At 2nd level, you tap into a deep wellspring of magic within yourself. This wellspring is represented by sorcery points, which allow you to create a variety of magical effects."),
            ("Sorcerer",  4, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Sorcerer",  3, "Metamagic", "At 3rd level, you gain the ability to twist your spells to suit your needs. You gain two of the following Metamagic options: Careful Spell, Distant Spell, Empowered Spell, Extended Spell, Heightened Spell, Quickened Spell, Subtle Spell, Twinned Spell."),
            ("Sorcerer", 20, "Sorcerous Restoration", "At 20th level, you regain 4 expended sorcery points whenever you finish a short rest."),

            // Warlock
            ("Warlock",  1, "Otherworldly Patron", "At 1st level, you have struck a bargain with an otherworldly being of your choice. Your choice grants you features at 1st level and again at 6th, 10th, and 14th level."),
            ("Warlock",  1, "Pact Magic", "Your arcane research and the magic bestowed on you by your patron have given you facility with spells. You can cast known warlock spells using your spell slots."),
            ("Warlock",  2, "Eldritch Invocations", "In your study of occult lore, you have unearthed eldritch invocations, fragments of forbidden knowledge that imbue you with an abiding magical ability. You gain two eldritch invocations of your choice."),
            ("Warlock",  3, "Pact Boon", "At 3rd level, your otherworldly patron bestows a gift upon you for your loyal service. You gain one of the following features of your choice: Pact of the Chain, Pact of the Blade, Pact of the Tome."),
            ("Warlock",  4, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Warlock", 11, "Mystic Arcanum", "At 11th level, your patron bestows upon you a magical secret called an arcanum. Choose one 6th-level spell from the warlock spell list as this arcanum."),
            ("Warlock", 20, "Eldritch Master", "At 20th level, you can draw on your inner reserve of mystical power to beg your patron to restore your expended spell slots. You can spend 1 minute entreating your patron for aid to regain all your expended spell slots from your Pact Magic feature."),

            // Wizard
            ("Wizard",  1, "Arcane Recovery", "You have learned to regain some of your magical energy by studying your spellbook. Once per day when you finish a short rest, you can choose expended spell slots to recover. The spell slots can have a combined level that is equal to or less than half your wizard level (rounded up)."),
            ("Wizard",  1, "Spellcasting", "As a student of arcane magic, you have a spellbook containing spells that show the first glimmerings of your true power."),
            ("Wizard",  2, "Arcane Tradition", "When you reach 2nd level, you choose an arcane tradition, shaping your practice of magic through one of the eight schools."),
            ("Wizard",  4, "Ability Score Improvement", "Your ability scores improve. You can increase one ability score by 2, or two ability scores by 1 each."),
            ("Wizard", 18, "Spell Mastery", "At 18th level, you have achieved such mastery over certain spells that you can cast them at will. Choose a 1st-level wizard spell and a 2nd-level wizard spell. You can cast those spells at their lowest level without expending a spell slot."),
            ("Wizard", 20, "Signature Spell", "When you reach 20th level, you gain mastery over two powerful spells and can cast them with little effort. Choose two 3rd-level wizard spells in your spellbook. You always have these spells prepared and can cast each of them once at 3rd level without expending a spell slot."),
        };

        foreach (var (cls, lvl, name, desc) in features)
        {
            if (classes.TryGetValue(cls, out var dndClass))
                _db.ClassFeatures.Add(new ClassFeature { ClassId = dndClass.Id, Level = lvl, Name = name, Description = desc });
        }
        await _db.SaveChangesAsync();

        // ── Subclasses ───────────────────────────────────────────────────────────
        var subclassData = new[]
        {
            // Barbarian — Primal Path (level 3)
            new { Class = "Barbarian", Name = "Path of the Berserker", Desc = "For some barbarians, rage is a means to an end—that end being violence. The Path of the Berserker is a path of untrammeled fury.", ChoiceLevel = 3,
                Features = new[]{ ("Frenzy", 3, "When you choose this path at 3rd level, you can go into a frenzy when you rage. Until the rage ends, you can make a single melee weapon attack as a bonus action on each of your turns after this one."), ("Mindless Rage", 6, "Beginning at 6th level, you can't be charmed or frightened while raging."), ("Retaliation", 14, "Starting at 14th level, when you take damage from a creature within 5 feet of you, you can use your reaction to make a melee weapon attack against that creature.") } },
            new { Class = "Barbarian", Name = "Path of the Totem Warrior", Desc = "The Path of the Totem Warrior is a spiritual journey, as the barbarian accepts a spirit animal as guide, protector, and inspiration.", ChoiceLevel = 3,
                Features = new[]{ ("Spirit Seeker", 3, "Yours is a path that seeks attunement with the natural world, giving you a kinship with beasts. You gain the ability to cast the beast sense and speak with animals spells."), ("Totem Spirit", 3, "At 3rd level, when you adopt this path, you choose a totem spirit (Bear, Eagle, or Wolf) and gain its feature while you rage."), ("Aspect of the Beast", 6, "At 6th level, you gain a magical benefit based on the totem animal of your choice.") } },

            // Bard — Bard College (level 3)
            new { Class = "Bard", Name = "College of Lore", Desc = "Bards of the College of Lore know something about most things, collecting bits of knowledge from sources as diverse as scholarly tomes and peasant tales.", ChoiceLevel = 3,
                Features = new[]{ ("Bonus Proficiencies", 3, "When you join the College of Lore at 3rd level, you gain proficiency with three skills of your choice."), ("Cutting Words", 3, "Also at 3rd level, you learn how to use your wit to distract, confuse, and otherwise sap the confidence and competence of others. When a creature that you can see within 60 feet makes an attack roll, ability check, or damage roll, you can use your reaction to expend one Bardic Inspiration die to reduce the roll."), ("Peerless Skill", 14, "Starting at 14th level, when you make an ability check, you can expend one use of Bardic Inspiration and add the number rolled to your ability check.") } },
            new { Class = "Bard", Name = "College of Valor", Desc = "Bards of the College of Valor are daring skalds whose tales keep alive the memory of the great heroes of the past.", ChoiceLevel = 3,
                Features = new[]{ ("Bonus Proficiencies", 3, "When you join the College of Valor at 3rd level, you gain proficiency with medium armor, shields, and martial weapons."), ("Combat Inspiration", 3, "Also at 3rd level, you learn to inspire others in battle. A creature that has a Bardic Inspiration die from you can roll that die and add the number rolled to a weapon damage roll it just made."), ("Battle Magic", 14, "At 14th level, you have mastered the art of weaving spellcasting and weapon use into a single harmonious act.") } },

            // Cleric — Divine Domain (level 1)
            new { Class = "Cleric", Name = "Life Domain", Desc = "The Life domain focuses on the vibrant positive energy—one of the fundamental forces of the universe—that sustains all life.", ChoiceLevel = 1,
                Features = new[]{ ("Disciple of Life", 2, "Also starting at 2nd level, your healing spells are more effective. Whenever you use a spell of 1st level or higher to restore hit points to a creature, the creature regains additional hit points equal to 2 + the spell's level."), ("Blessed Healer", 6, "Beginning at 6th level, the healing spells you cast on others heal you as well."), ("Divine Strike", 8, "At 8th level, you gain the ability to infuse your weapon strikes with divine energy."), ("Supreme Healing", 17, "Starting at 17th level, when you would normally roll one or more dice to restore hit points with a spell, you instead use the highest number possible for each die.") } },
            new { Class = "Cleric", Name = "Light Domain", Desc = "Gods of light—including Helm, Lathander, Pholtus, Branchala, the Silver Flame, Belenus, Apollo, and Re-Horakhty—promote the ideals of rebirth and renewal.", ChoiceLevel = 1,
                Features = new[]{ ("Warding Flare", 1, "Also at 1st level, you can interpose divine light between yourself and an attacking enemy. When you are attacked by a creature within 30 feet of you that you can see, you can use your reaction to impose disadvantage on the attack roll."), ("Radiance of the Dawn", 2, "Starting at 2nd level, you can use your Channel Divinity to harness sunlight, banishing darkness and dealing radiant damage to your foes."), ("Improved Flare", 6, "Starting at 6th level, you can also use your Warding Flare feature when a creature that you can see within 30 feet of you attacks a creature other than you."), ("Corona of Light", 17, "Starting at 17th level, you can use your action to activate an aura of sunlight that lasts for 1 minute or until you dismiss it using another action.") } },

            // Druid — Druid Circle (level 2)
            new { Class = "Druid", Name = "Circle of the Land", Desc = "The Circle of the Land is made up of mystics and sages who safeguard ancient knowledge and rites through a vast oral tradition.", ChoiceLevel = 2,
                Features = new[]{ ("Bonus Cantrip", 2, "When you choose this circle at 2nd level, you learn one additional druid cantrip of your choice."), ("Natural Recovery", 2, "Starting at 2nd level, you can regain some of your magical energy by sitting in meditation and communing with nature. During a short rest, you choose expended spell slots to recover."), ("Land's Stride", 6, "Starting at 6th level, moving through nonmagical difficult terrain costs you no extra movement."), ("Nature's Sanctuary", 10, "When you reach 10th level, creatures of the natural world sense your connection to nature and become hesitant to attack you."), ("Nature's Ward", 14, "When you reach 14th level, you can't be charmed or frightened by elementals or fey, and you are immune to poison and disease.") } },
            new { Class = "Druid", Name = "Circle of the Moon", Desc = "Druids of the Circle of the Moon are fierce guardians of the wilds. Their order gathers under the full moon to share news and trade warnings.", ChoiceLevel = 2,
                Features = new[]{ ("Combat Wild Shape", 2, "When you choose this circle at 2nd level, you gain the ability to use Wild Shape on your turn as a bonus action, rather than as an action. Additionally, while you are transformed by Wild Shape, you can use a bonus action to expend one spell slot to regain 1d8 hit points per level of the spell slot expended."), ("Circle Forms", 2, "The rites of your circle grant you the ability to transform into more dangerous animal forms. Starting at 2nd level, you can use your Wild Shape to transform into a beast with a challenge rating as high as 1."), ("Primal Strike", 6, "Starting at 6th level, your attacks in beast form count as magical for the purpose of overcoming resistance and immunity to nonmagical attacks and damage."), ("Elemental Wild Shape", 10, "At 10th level, you can expend two uses of Wild Shape at the same time to transform into an air elemental, an earth elemental, a fire elemental, or a water elemental."), ("Thousand Forms", 14, "By 14th level, you have learned to use magic to alter your physical form in more subtle ways. You can cast the alter self spell at will.") } },

            // Fighter — Martial Archetype (level 3)
            new { Class = "Fighter", Name = "Champion", Desc = "The archetypal Champion focuses on the development of raw physical power honed to deadly perfection.", ChoiceLevel = 3,
                Features = new[]{ ("Improved Critical", 3, "Beginning when you choose this archetype at 3rd level, your weapon attacks score a critical hit on a roll of 19 or 20."), ("Remarkable Athlete", 7, "Starting at 7th level, you can add half your proficiency bonus (rounded up) to any STR, DEX, or CON check you make that doesn't already use your proficiency bonus."), ("Superior Critical", 15, "Starting at 15th level, your weapon attacks score a critical hit on a roll of 18–20."), ("Survivor", 18, "At 18th level, you attain the pinnacle of resilience in battle.") } },
            new { Class = "Fighter", Name = "Battle Master", Desc = "Those who emulate the archetypal Battle Master employ martial techniques passed down through generations.", ChoiceLevel = 3,
                Features = new[]{ ("Combat Superiority", 3, "When you choose this archetype at 3rd level, you learn maneuvers that are fueled by special dice called superiority dice. You learn three maneuvers of your choice and gain four superiority dice (d8s)."), ("Know Your Enemy", 7, "If you spend at least 1 minute observing or interacting with another creature outside combat, you can learn certain information about its capabilities compared to your own."), ("Improved Combat Superiority", 10, "At 10th level, your superiority dice turn into d10s."), ("Relentless", 15, "Starting at 15th level, when you roll initiative and have no superiority dice remaining, you regain 1 superiority die.") } },
            new { Class = "Fighter", Name = "Eldritch Knight", Desc = "The archetypal Eldritch Knight combines the martial mastery common to all fighters with a careful study of magic.", ChoiceLevel = 3,
                Features = new[]{ ("Spellcasting", 3, "When you reach 3rd level, you augment your martial prowess with the ability to cast spells. You learn two cantrips of your choice from the wizard spell list."), ("War Magic", 7, "Beginning at 7th level, when you use your action to cast a cantrip, you can make one weapon attack as a bonus action."), ("Arcane Charge", 10, "At 10th level, you gain the ability to teleport up to 30 feet to an unoccupied space you can see when you use your Action Surge."), ("Improved War Magic", 18, "Starting at 18th level, when you use your action to cast a spell, you can make one weapon attack as a bonus action.") } },

            // Monk — Monastic Tradition (level 3)
            new { Class = "Monk", Name = "Way of the Open Hand", Desc = "Monks of the Way of the Open Hand are the ultimate masters of martial arts combat, whether armed or unarmed.", ChoiceLevel = 3,
                Features = new[]{ ("Open Hand Technique", 3, "Starting when you choose this tradition at 3rd level, you can manipulate your enemy's ki when you harness your own. Whenever you hit a creature with one of the attacks granted by your Flurry of Blows, you can impose one of the following effects on that target."), ("Wholeness of Body", 6, "At 6th level, you gain the ability to heal yourself. As an action, you can regain hit points equal to three times your monk level."), ("Tranquility", 11, "Beginning at 11th level, you can enter a special meditation that surrounds you with an aura of peace. At the end of a long rest, you gain the effect of a sanctuary spell."), ("Quivering Palm", 17, "At 17th level, you gain the ability to set up lethal vibrations in someone's body. When you hit a creature with an unarmed strike, you can spend 3 ki points to start these imperceptible vibrations.") } },
            new { Class = "Monk", Name = "Way of Shadow", Desc = "Monks of the Way of Shadow follow a tradition that values stealth and subterfuge. These monks might be called ninjas or shadowdancers.", ChoiceLevel = 3,
                Features = new[]{ ("Shadow Arts", 3, "Starting when you choose this tradition at 3rd level, you can use your ki to duplicate the effects of certain spells. As an action, you can spend 2 ki points to cast darkness, darkvision, pass without trace, or silence, without providing material components."), ("Shadow Step", 6, "At 6th level, you gain the ability to step from one shadow into another. When you are in dim light or darkness, as a bonus action you can teleport up to 60 feet to an unoccupied space you can see that is also in dim light or darkness."), ("Cloak of Shadows", 11, "By 11th level, you have learned to become one with the shadows. When you are in an area of dim light or darkness, you can use your action to become invisible."), ("Opportunist", 17, "At 17th level, you can exploit a creature's momentary distraction when it is hit by an attack.") } },

            // Paladin — Sacred Oath (level 3)
            new { Class = "Paladin", Name = "Oath of Devotion", Desc = "The Oath of Devotion binds a paladin to the loftiest ideals of justice, virtue, and order.", ChoiceLevel = 3,
                Features = new[]{ ("Sacred Weapon", 3, "As an action, you can imbue one weapon that you are holding with positive energy. For 1 minute, you add your CHA modifier to attack rolls made with that weapon."), ("Turn the Unholy", 3, "As an action, you present your holy symbol and speak a prayer censuring fiends and undead, using your Channel Divinity."), ("Aura of Devotion", 7, "Starting at 7th level, you and friendly creatures within 10 feet of you can't be charmed while you are conscious."), ("Holy Nimbus", 20, "At 20th level, as an action, you can emanate an aura of sunlight. For 1 minute, bright light shines from you in a 30-foot radius.") } },
            new { Class = "Paladin", Name = "Oath of the Ancients", Desc = "The Oath of the Ancients is as old as the race of elves and the rituals of the druids. Paladins who swear this oath cherish the light.", ChoiceLevel = 3,
                Features = new[]{ ("Nature's Wrath", 3, "You can use your Channel Divinity to invoke primeval forces to ensnare a foe."), ("Turn the Faithless", 3, "You can use your Channel Divinity to utter ancient words that are painful for fey and fiends to hear."), ("Aura of Warding", 7, "Beginning at 7th level, ancient magic lies so heavily upon you that it forms an eldritch ward. You and friendly creatures within 10 feet of you have resistance to damage from spells."), ("Elder Champion", 20, "At 20th level, you can assume the form of an ancient force of nature, taking on an appearance you choose.") } },

            // Ranger — Ranger Archetype (level 3)
            new { Class = "Ranger", Name = "Hunter", Desc = "Emulating the Hunter archetype means accepting your place as a bulwark between civilization and the terrors of the wilderness.", ChoiceLevel = 3,
                Features = new[]{ ("Hunter's Prey", 3, "At 3rd level, you gain one of the following features of your choice: Colossus Slayer, Giant Killer, or Horde Breaker."), ("Defensive Tactics", 7, "At 7th level, you gain one of the following features of your choice: Escape the Horde, Multiattack Defense, or Steel Will."), ("Multiattack", 11, "At 11th level, you gain one of the following features of your choice: Volley or Whirlwind Attack."), ("Superior Hunter's Defense", 15, "At 15th level, you gain one of the following features of your choice: Evasion, Stand Against the Tide, or Uncanny Dodge.") } },
            new { Class = "Ranger", Name = "Beast Master", Desc = "The Beast Master archetype embodies a friendship between the civilized races and the beasts of the world.", ChoiceLevel = 3,
                Features = new[]{ ("Ranger's Companion", 3, "At 3rd level, you gain a beast companion that accompanies you on your adventures and is trained to fight alongside you. Choose a beast that is no larger than Medium and that has a challenge rating of 1/4 or lower."), ("Exceptional Training", 7, "Beginning at 7th level, on any of your turns when your beast companion doesn't attack, you can use a bonus action to command the beast to take the Dash, Disengage, Dodge, or Help action on its turn."), ("Bestial Fury", 11, "Starting at 11th level, your beast companion can make two attacks when you command it to use the Attack action."), ("Share Spells", 15, "Beginning at 15th level, when you cast a spell targeting yourself, you can also affect your beast companion with the spell if the beast is within 30 feet of you.") } },

            // Rogue — Roguish Archetype (level 3)
            new { Class = "Rogue", Name = "Thief", Desc = "You hone your skills in the larcenous arts. Burglars, bandits, cutpurses, and other criminals typically follow this archetype.", ChoiceLevel = 3,
                Features = new[]{ ("Fast Hands", 3, "Starting at 3rd level, you can use the bonus action granted by your Cunning Action to make a DEX (Sleight of Hand) check, use your thieves' tools to disarm a trap or open a lock, or take the Use an Object action."), ("Second-Story Work", 3, "When you choose this archetype at 3rd level, you gain the ability to climb faster than normal; climbing no longer costs you extra movement."), ("Supreme Sneak", 9, "Starting at 9th level, you have advantage on a DEX (Stealth) check if you move no more than half your speed on the same turn."), ("Use Magic Device", 13, "By 13th level, you have learned enough about the workings of magic that you can improvise the use of items even when they are not intended for you."), ("Thief's Reflexes", 17, "When you reach 17th level, you have become adept at laying ambushes and quickly escaping danger.") } },
            new { Class = "Rogue", Name = "Assassin", Desc = "You focus your training on the grim art of death. Those who adhere to this archetype are diverse—hired killers, spies, bounty hunters.", ChoiceLevel = 3,
                Features = new[]{ ("Bonus Proficiencies", 3, "When you choose this archetype at 3rd level, you gain proficiency with the disguise kit and the poisoner's kit."), ("Assassinate", 3, "Starting at 3rd level, you are at your deadliest when you get the drop on your enemies. You have advantage on attack rolls against any creature that hasn't taken a turn in the combat yet."), ("Infiltration Expertise", 9, "Starting at 9th level, you can unfailingly create false identities for yourself."), ("Impostor", 13, "At 13th level, you gain the ability to unerringly mimic another person's speech, writing, and behavior."), ("Death Strike", 17, "Starting at 17th level, you become a master of instant death. When you attack and hit a creature that is surprised, it must make a CON saving throw (DC 8 + your DEX modifier + your proficiency bonus).") } },
            new { Class = "Rogue", Name = "Arcane Trickster", Desc = "Some rogues enhance their fine-honed skills of stealth and agility with magic, learning tricks of enchantment and illusion.", ChoiceLevel = 3,
                Features = new[]{ ("Spellcasting", 3, "When you reach 3rd level, you augment your martial prowess with the ability to cast spells."), ("Mage Hand Legerdemain", 3, "Starting at 3rd level, when you cast Mage Hand, you can make the spectral hand invisible, and you can perform the following additional tasks with it."), ("Magical Ambush", 9, "Starting at 9th level, if you are hidden from a creature when you cast a spell on it, the creature has disadvantage on any saving throw it makes against the spell this turn."), ("Versatile Trickster", 13, "At 13th level, you gain the ability to distract targets with your Mage Hand."), ("Spell Thief", 17, "At 17th level, you gain the ability to magically steal the knowledge of how to cast a spell from another spellcaster.") } },

            // Sorcerer — Sorcerous Origin (level 1)
            new { Class = "Sorcerer", Name = "Draconic Bloodline", Desc = "Your innate magic comes from draconic magic that was mingled with your blood or that of your ancestors.", ChoiceLevel = 1,
                Features = new[]{ ("Dragon Ancestor", 1, "At 1st level, you choose one type of dragon as your ancestor. The damage type associated with each dragon is used by features you gain later."), ("Draconic Resilience", 1, "As magic flows through your body, it causes physical traits of your dragon ancestors to emerge. At 1st level, your hit point maximum increases by 1 and increases by 1 again whenever you gain a level in this class. Additionally, parts of your skin are covered by a thin sheen of dragon-like scales. When you aren't wearing armor, your AC equals 13 + your DEX modifier."), ("Elemental Affinity", 6, "Starting at 6th level, when you cast a spell that deals damage of the type associated with your draconic ancestry, you can add your CHA modifier to one damage roll of that spell."), ("Dragon Wings", 14, "At 14th level, you gain the ability to sprout a pair of dragon wings from your back, gaining a flying speed equal to your current speed."), ("Draconic Presence", 18, "Beginning at 18th level, you can channel the dread presence of your dragon ancestor, causing those around you to become awestruck or frightened.") } },
            new { Class = "Sorcerer", Name = "Wild Magic", Desc = "Your innate magic comes from the wild forces of chaos that underlie the order of creation.", ChoiceLevel = 1,
                Features = new[]{ ("Wild Magic Surge", 1, "Starting when you choose this origin at 1st level, your spellcasting can unleash surges of untamed magic. Immediately after you cast a sorcerer spell of 1st level or higher, the DM can have you roll a d20. If you roll a 1, roll on the Wild Magic Surge table to create a random magical effect."), ("Tides of Chaos", 1, "Starting at 1st level, you can manipulate the forces of chance and chaos to gain advantage on one attack roll, ability check, or saving throw. Once you do so, you must finish a long rest before you can use this feature again."), ("Bend Luck", 6, "Starting at 6th level, you have the ability to twist fate using your wild magic. When another creature you can see makes an attack roll, an ability check, or a saving throw, you can use your reaction and spend 2 sorcery points to roll 1d4 and apply the number rolled as a bonus or penalty to the creature's roll."), ("Controlled Chaos", 14, "At 14th level, you gain a modicum of control over the surges of your wild magic."), ("Spell Bombardment", 18, "Beginning at 18th level, the harmful energy of your spells intensifies.") } },

            // Warlock — Otherworldly Patron (level 1)
            new { Class = "Warlock", Name = "The Fiend", Desc = "You have made a pact with a fiend from the lower planes of existence, a being whose aims are evil, even if you strive against those aims.", ChoiceLevel = 1,
                Features = new[]{ ("Dark One's Blessing", 1, "Starting at 1st level, when you reduce a hostile creature to 0 hit points, you gain temporary hit points equal to your CHA modifier + your warlock level (minimum of 1)."), ("Dark One's Own Luck", 6, "Starting at 6th level, you can call on your patron to alter fate in your favor. When you make an ability check or a saving throw, you can use this feature to add a d10 to your roll."), ("Fiendish Resilience", 10, "Starting at 10th level, you can choose one damage type when you finish a short or long rest. You gain resistance to that damage type until you choose a different one with this feature."), ("Hurl Through Hell", 14, "Starting at 14th level, when you hit a creature with an attack, you can use this feature to instantly transport the target through the lower planes.") } },
            new { Class = "Warlock", Name = "The Archfey", Desc = "Your patron is a lord or lady of the fey, a creature of legend who holds secrets that were forgotten before the mortal races were born.", ChoiceLevel = 1,
                Features = new[]{ ("Fey Presence", 1, "Starting at 1st level, your patron bestows upon you the ability to project the beguiling and fearsome presence of the fey."), ("Misty Escape", 6, "Starting at 6th level, you can vanish in a puff of mist in response to harm. When you take damage, you can use your reaction to turn invisible and teleport up to 60 feet to an unoccupied space you can see."), ("Beguiling Defenses", 10, "Beginning at 10th level, your patron teaches you how to turn the mind-affecting magic of your enemies against them."), ("Dark Delirium", 14, "Starting at 14th level, you can plunge a creature into an illusory realm.") } },

            // Wizard — Arcane Tradition (level 2)
            new { Class = "Wizard", Name = "School of Evocation", Desc = "You focus your study on magic that creates powerful elemental effects such as bitter cold, searing flame, rolling thunder, crackling lightning, and burning acid.", ChoiceLevel = 2,
                Features = new[]{ ("Evocation Savant", 2, "Beginning when you select this school at 2nd level, the gold and time you must spend to copy an evocation spell into your spellbook is halved."), ("Sculpt Spells", 2, "Beginning at 2nd level, you can create pockets of relative safety within the effects of your evocation spells."), ("Potent Cantrip", 6, "Starting at 6th level, your damaging cantrips affect even creatures that avoid the brunt of the effect."), ("Empowered Evocation", 10, "Beginning at 10th level, you can add your INT modifier to the damage roll of any wizard evocation spell you cast."), ("Overchannel", 14, "Starting at 14th level, you can increase the power of your simpler spells.") } },
            new { Class = "Wizard", Name = "School of Abjuration", Desc = "The School of Abjuration emphasizes magic that blocks, banishes, or protects.", ChoiceLevel = 2,
                Features = new[]{ ("Abjuration Savant", 2, "Beginning when you select this school at 2nd level, the gold and time you must spend to copy an abjuration spell into your spellbook is halved."), ("Arcane Ward", 2, "Starting at 2nd level, you can weave magic around yourself for protection. When you cast an abjuration spell of 1st level or higher, you can simultaneously use a strand of the spell's magic to create a magical ward on yourself that lasts until you finish a long rest."), ("Projected Ward", 6, "Starting at 6th level, when a creature that you can see within 30 feet of you takes damage, you can use your reaction to cause your Arcane Ward to absorb that damage."), ("Improved Abjuration", 10, "Beginning at 10th level, when you cast an abjuration spell that requires you to make an ability check as a part of casting that spell, you add your proficiency bonus to that ability check."), ("Spell Resistance", 14, "Starting at 14th level, you have advantage on saving throws against spells.") } },
            new { Class = "Wizard", Name = "School of Divination", Desc = "The counsel of a diviner is sought by royalty and commoners alike, for all seek a clearer understanding of the past, present, and future.", ChoiceLevel = 2,
                Features = new[]{ ("Divination Savant", 2, "Beginning when you select this school at 2nd level, the gold and time you must spend to copy a divination spell into your spellbook is halved."), ("Portent", 2, "Starting at 2nd level when you choose this school, glimpses of the future begin to press in on your awareness. When you finish a long rest, roll two d20s and record the numbers rolled. You can replace any attack roll, saving throw, or ability check made by you or a creature that you can see with one of these foretelling rolls."), ("Expert Divination", 6, "Beginning at 6th level, casting divination spells comes so easily to you that it expends only a fraction of your spellcasting efforts."), ("The Third Eye", 10, "Starting at 10th level, you can use your action to increase your powers of perception."), ("Greater Portent", 14, "Starting at 14th level, the visions in your dreams intensify and paint a more accurate picture in your mind of what is to come.") } },
            new { Class = "Wizard", Name = "School of Illusion", Desc = "You focus your studies on magic that dazzles the senses, befuddles the mind, and tricks even the wisest folk.", ChoiceLevel = 2,
                Features = new[]{ ("Illusion Savant", 2, "Beginning when you select this school at 2nd level, the gold and time you must spend to copy an illusion spell into your spellbook is halved."), ("Improved Minor Illusion", 2, "When you choose this school at 2nd level, you learn the minor illusion cantrip. If you already know this cantrip, you learn a different wizard cantrip of your choice. The cantrip doesn't count against your number of cantrips known."), ("Malleable Illusions", 6, "Starting at 6th level, when you cast an illusion spell that has a duration of 1 minute or longer, you can use your action to change the nature of that illusion."), ("Illusory Self", 10, "Beginning at 10th level, you can create an illusory duplicate of yourself as an instant, almost instinctual reaction to danger."), ("Illusory Reality", 14, "By 14th level, you have learned the secret of weaving shadow magic into your illusions to give them a semi-reality.") } },
        };

        foreach (var sd in subclassData)
        {
            if (!classes.TryGetValue(sd.Class, out var dndClass)) continue;
            var subclass = new Subclass { ClassId = dndClass.Id, Name = sd.Name, Description = sd.Desc, ChoiceLevel = sd.ChoiceLevel };
            _db.Subclasses.Add(subclass);
            await _db.SaveChangesAsync();
            foreach (var (fname, flvl, fdesc) in sd.Features)
                _db.SubclassFeatures.Add(new SubclassFeature { SubclassId = subclass.Id, Name = fname, Level = flvl, Description = fdesc });
        }
        await _db.SaveChangesAsync();
    }

    private async Task SeedSpellsAsync()
    {
        var schools = await _db.MagicSchools.ToDictionaryAsync(s => s.Name);
        var classes = await _db.Classes.ToDictionaryAsync(c => c.Name);
        var damageTypes = await _db.DamageTypes.ToDictionaryAsync(d => d.Name);

        List<(Spell spell, string[] classNames, (string type, string dice)[] damages)> spells = [
            (new Spell { Name = "Fireball", Level = 3, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "150 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a tiny ball of bat guano and sulfur", Duration = "Instantaneous", Description = "A bright streak flashes from your pointing finger to a point you choose within range and then blossoms with a low roar into an explosion of flame. Each creature in a 20-foot-radius sphere centered on that point must make a Dexterity saving throw.", AtHigherLevels = "When you cast this spell using a spell slot of 4th level or higher, the damage increases by 1d6 for each slot level above 3rd." },
             ["Sorcerer", "Wizard"], [("Fire", "8d6")]),
            (new Spell { Name = "Magic Missile", Level = 1, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "120 feet", Components = SpellComponents.VerbalSomatic, Duration = "Instantaneous", Description = "You create three glowing darts of magical force. Each dart hits a creature of your choice that you can see within range. A dart deals 1d4+1 force damage to its target. The darts all strike simultaneously, and you can direct them to hit one creature or several.", AtHigherLevels = "When you cast this spell using a spell slot of 2nd level or higher, the spell creates one more dart for each slot level above 1st." },
             ["Sorcerer", "Wizard"], [("Force", "1d4+1")]),
            (new Spell { Name = "Cure Wounds", Level = 1, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "Touch", Components = SpellComponents.VerbalSomatic, Duration = "Instantaneous", Description = "A creature you touch regains a number of hit points equal to 1d8 + your spellcasting ability modifier. This spell has no effect on undead or constructs.", AtHigherLevels = "When you cast this spell using a spell slot of 2nd level or higher, the healing increases by 1d8 for each slot level above 1st." },
             ["Bard", "Cleric", "Druid", "Paladin", "Ranger"], []),
            (new Spell { Name = "Shield", Level = 1, SchoolId = schools["Abjuration"].Id, CastingTime = "1 reaction, when you are hit by an attack or targeted by the magic missile spell", Range = "Self", Components = SpellComponents.VerbalSomatic, Duration = "1 round", Description = "An invisible barrier of magical force appears and protects you. Until the start of your next turn, you have a +5 bonus to AC, including against the triggering attack, and you take no damage from magic missile." },
             ["Sorcerer", "Wizard"], []),
            (new Spell { Name = "Mage Armor", Level = 1, SchoolId = schools["Abjuration"].Id, CastingTime = "1 action", Range = "Touch", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a piece of cured leather", Duration = "8 hours", Description = "You touch a willing creature who isn't wearing armor, and a protective magical force surrounds it until the spell ends. The target's base AC becomes 13 + its Dexterity modifier. The spell ends if the target dons armor or if you dismiss the spell as an action." },
             ["Sorcerer", "Wizard"], []),
            (new Spell { Name = "Healing Word", Level = 1, SchoolId = schools["Evocation"].Id, CastingTime = "1 bonus action", Range = "60 feet", Components = SpellComponents.Verbal, Duration = "Instantaneous", Description = "A creature of your choice that you can see within range regains hit points equal to 1d4 + your spellcasting ability modifier.", AtHigherLevels = "When you cast this spell using a spell slot of 2nd level or higher, the healing increases by 1d4 for each slot level above 1st." },
             ["Bard", "Cleric", "Druid"], []),
            (new Spell { Name = "Lightning Bolt", Level = 3, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "Self (100-foot line)", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a bit of fur and a rod of amber, crystal, or glass", Duration = "Instantaneous", Description = "A stroke of lightning forming a line 100 feet long and 5 feet wide blasts out from you in a direction you choose. Each creature in the line must make a Dexterity saving throw.", AtHigherLevels = "When you cast this spell using a spell slot of 4th level or higher, the damage increases by 1d6 for each slot level above 3rd." },
             ["Sorcerer", "Wizard"], [("Lightning", "8d6")]),
            (new Spell { Name = "Hold Person", Level = 2, SchoolId = schools["Enchantment"].Id, CastingTime = "1 action", Range = "60 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a small, straight piece of iron", Duration = "1 minute", IsConcentration = true, Description = "Choose a humanoid that you can see within range. The target must succeed on a Wisdom saving throw or be paralyzed for the duration.", AtHigherLevels = "When you cast this spell using a spell slot of 3rd level or higher, you can target one additional humanoid for each slot level above 2nd." },
             ["Bard", "Cleric", "Druid", "Sorcerer", "Warlock", "Wizard"], []),
            (new Spell { Name = "Counterspell", Level = 3, SchoolId = schools["Abjuration"].Id, CastingTime = "1 reaction, which you take when you see a creature within 60 feet casting a spell", Range = "60 feet", Components = SpellComponents.Somatic, Duration = "Instantaneous", Description = "You attempt to interrupt a creature in the process of casting a spell. If the creature is casting a spell of 3rd level or lower, its spell fails and has no effect. If it is casting a spell of 4th level or higher, make an ability check using your spellcasting ability.", AtHigherLevels = "When you cast this spell using a spell slot of 4th level or higher, the interrupted spell has no effect if its level is less than or equal to the level of the spell slot you used." },
             ["Sorcerer", "Warlock", "Wizard"], []),
            (new Spell { Name = "Misty Step", Level = 2, SchoolId = schools["Conjuration"].Id, CastingTime = "1 bonus action", Range = "Self", Components = SpellComponents.Verbal, Duration = "Instantaneous", Description = "Briefly surrounded by silvery mist, you teleport up to 30 feet to an unoccupied space that you can see." },
             ["Sorcerer", "Warlock", "Wizard"], []),
            (new Spell { Name = "Invisibility", Level = 2, SchoolId = schools["Illusion"].Id, CastingTime = "1 action", Range = "Touch", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "an eyelash encased in gum arabic", Duration = "1 hour", IsConcentration = true, Description = "A creature you touch becomes invisible until the spell ends. Anything the target is wearing or carrying is invisible as long as it is on the target's person. The spell ends for a target that attacks or casts a spell.", AtHigherLevels = "When you cast this spell using a spell slot of 3rd level or higher, you can target one additional creature for each slot level above 2nd." },
             ["Bard", "Sorcerer", "Warlock", "Wizard"], []),
            (new Spell { Name = "Charm Person", Level = 1, SchoolId = schools["Enchantment"].Id, CastingTime = "1 action", Range = "30 feet", Components = SpellComponents.VerbalSomatic, Duration = "1 hour", Description = "You attempt to charm a humanoid you can see within range. It must make a Wisdom saving throw, and does so with advantage if you or your companions are fighting it. If it fails the saving throw, it is charmed by you until the spell ends or until you or your companions do anything harmful to it.", AtHigherLevels = "When you cast this spell using a spell slot of 2nd level or higher, you can target one additional creature for each slot level above 1st." },
             ["Bard", "Druid", "Sorcerer", "Warlock", "Wizard"], []),
            (new Spell { Name = "Sleep", Level = 1, SchoolId = schools["Enchantment"].Id, CastingTime = "1 action", Range = "90 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a pinch of fine sand, rose petals, or a cricket", Duration = "1 minute", Description = "This spell sends creatures into a magical slumber. Roll 5d8; the total is how many hit points of creatures this spell can affect. Creatures within 20 feet of a point you choose within range are affected in ascending order of their current hit points.", AtHigherLevels = "When you cast this spell using a spell slot of 2nd level or higher, roll an additional 2d8 for each slot level above 1st." },
             ["Bard", "Sorcerer", "Wizard"], []),
            (new Spell { Name = "Detect Magic", Level = 1, SchoolId = schools["Divination"].Id, CastingTime = "1 action", Range = "Self", Components = SpellComponents.VerbalSomatic, Duration = "10 minutes", IsConcentration = true, IsRitual = true, Description = "For the duration, you sense the presence of magic within 30 feet of you. If you sense magic in this way, you can use your action to see a faint aura around any visible creature or object in the area that bears magic, and you learn its school of magic, if any." },
             ["Bard", "Cleric", "Druid", "Paladin", "Ranger", "Sorcerer", "Wizard"], []),
            (new Spell { Name = "Polymorph", Level = 4, SchoolId = schools["Transmutation"].Id, CastingTime = "1 action", Range = "60 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a caterpillar cocoon", Duration = "1 hour", IsConcentration = true, Description = "This spell transforms a creature that you can see within range into a new form. An unwilling creature must make a Wisdom saving throw to avoid the effect. The spell has no effect on a shapechanger or a creature with 0 hit points." },
             ["Bard", "Cleric", "Druid", "Sorcerer", "Wizard"], []),
            (new Spell { Name = "Haste", Level = 3, SchoolId = schools["Transmutation"].Id, CastingTime = "1 action", Range = "30 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a shaving of licorice root", Duration = "1 minute", IsConcentration = true, Description = "Choose a willing creature that you can see within range. Until the spell ends, the target's speed is doubled, it gains a +2 bonus to AC, it has advantage on Dexterity saving throws, and it gains an additional action on each of its turns." },
             ["Sorcerer", "Wizard"], []),
            (new Spell { Name = "Bless", Level = 1, SchoolId = schools["Enchantment"].Id, CastingTime = "1 action", Range = "30 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a sprinkling of holy water", Duration = "1 minute", IsConcentration = true, Description = "You bless up to three creatures of your choice within range. Whenever a target makes an attack roll or a saving throw before the spell ends, the target can roll a d4 and add the number rolled to the attack roll or saving throw.", AtHigherLevels = "When you cast this spell using a spell slot of 2nd level or higher, you can target one additional creature for each slot level above 1st." },
             ["Cleric", "Paladin"], []),
            (new Spell { Name = "Spiritual Weapon", Level = 2, SchoolId = schools["Evocation"].Id, CastingTime = "1 bonus action", Range = "60 feet", Components = SpellComponents.VerbalSomatic, Duration = "1 minute", Description = "You create a floating, spectral weapon within range that lasts for the duration or until you cast this spell again. When you cast the spell, you can make a melee spell attack against a creature within 5 feet of the weapon. On a hit, the target takes force damage equal to 1d8 + your spellcasting ability modifier.", AtHigherLevels = "When you cast this spell using a spell slot of 3rd level or higher, the damage increases by 1d8 for every two slot levels above 2nd." },
             ["Cleric"], [("Force", "1d8")]),
            (new Spell { Name = "Sacred Flame", Level = 0, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "60 feet", Components = SpellComponents.VerbalSomatic, Duration = "Instantaneous", Description = "Flame-like radiance descends on a creature that you can see within range. The target must succeed on a Dexterity saving throw or take 1d8 radiant damage. The target gains no benefit from cover for this saving throw." },
             ["Cleric"], [("Radiant", "1d8")]),
            (new Spell { Name = "Eldritch Blast", Level = 0, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "120 feet", Components = SpellComponents.VerbalSomatic, Duration = "Instantaneous", Description = "A beam of crackling energy streaks toward a creature within range. Make a ranged spell attack against the target. On a hit, the target takes 1d10 force damage." },
             ["Warlock"], [("Force", "1d10")]),
            (new Spell { Name = "Fire Bolt", Level = 0, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "120 feet", Components = SpellComponents.VerbalSomatic, Duration = "Instantaneous", Description = "You hurl a mote of fire at a creature or object within range. Make a ranged spell attack against the target. On a hit, the target takes 1d10 fire damage. A flammable object hit by this spell ignites if it isn't being worn or carried." },
             ["Sorcerer", "Wizard"], [("Fire", "1d10")]),
            (new Spell { Name = "Vicious Mockery", Level = 0, SchoolId = schools["Enchantment"].Id, CastingTime = "1 action", Range = "60 feet", Components = SpellComponents.Verbal, Duration = "Instantaneous", Description = "You unleash a string of insults laced with subtle enchantments at a creature you can see within range. If the target can hear you (though it need not understand you), it must succeed on a Wisdom saving throw or take 1d4 psychic damage and have disadvantage on the next attack roll it makes before the end of its next turn." },
             ["Bard"], [("Psychic", "1d4")]),
            (new Spell { Name = "Toll the Dead", Level = 0, SchoolId = schools["Necromancy"].Id, CastingTime = "1 action", Range = "60 feet", Components = SpellComponents.VerbalSomatic, Duration = "Instantaneous", Description = "You point at one creature you can see within range, and the sound of a dolorous bell fills the air around it for a moment. The target must succeed on a Wisdom saving throw or take 1d8 necrotic damage. If the target is missing any of its hit points, it instead takes 1d12 necrotic damage." },
             ["Cleric", "Warlock", "Wizard"], [("Necrotic", "1d8")]),
            (new Spell { Name = "Thunderwave", Level = 1, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "Self (15-foot cube)", Components = SpellComponents.VerbalSomatic, Duration = "Instantaneous", Description = "A wave of thunderous force sweeps out from you. Each creature in a 15-foot cube originating from you must make a Constitution saving throw. On a failed save, a creature takes 2d8 thunder damage and is pushed 10 feet away from you. On a successful save, the creature takes half as much damage and isn't pushed.", AtHigherLevels = "When you cast this spell using a spell slot of 2nd level or higher, the damage increases by 1d8 for each slot level above 1st." },
             ["Bard", "Druid", "Sorcerer", "Wizard"], [("Thunder", "2d8")]),
            (new Spell { Name = "Burning Hands", Level = 1, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "Self (15-foot cone)", Components = SpellComponents.VerbalSomatic, Duration = "Instantaneous", Description = "As you hold your hands with thumbs touching and fingers spread, a thin sheet of flames shoots forth from your outstretched fingertips. Each creature in a 15-foot cone must make a Dexterity saving throw. A creature takes 3d6 fire damage on a failed save, or half as much damage on a successful one.", AtHigherLevels = "When you cast this spell using a spell slot of 2nd level or higher, the damage increases by 1d6 for each slot level above 1st." },
             ["Sorcerer", "Wizard"], [("Fire", "3d6")]),
            (new Spell { Name = "Blight", Level = 4, SchoolId = schools["Necromancy"].Id, CastingTime = "1 action", Range = "30 feet", Components = SpellComponents.VerbalSomatic, Duration = "Instantaneous", Description = "Necromantic energy washes over a creature of your choice that you can see within range, draining moisture and vitality from it. The target must make a Constitution saving throw. The target takes 8d8 necrotic damage on a failed save, or half as much damage on a successful one.", AtHigherLevels = "When you cast this spell using a spell slot of 5th level or higher, the damage increases by 1d8 for each slot level above 4th." },
             ["Cleric", "Druid", "Sorcerer", "Warlock", "Wizard"], [("Necrotic", "8d8")]),
            (new Spell { Name = "Dimension Door", Level = 4, SchoolId = schools["Conjuration"].Id, CastingTime = "1 action", Range = "500 feet", Components = SpellComponents.Verbal, Duration = "Instantaneous", Description = "You teleport yourself from your current location to any other spot within range. You arrive at exactly the spot desired. It can be a place you can see, one you can visualize, or one you can describe by stating distance and direction." },
             ["Bard", "Sorcerer", "Warlock", "Wizard"], []),
            (new Spell { Name = "Greater Invisibility", Level = 4, SchoolId = schools["Illusion"].Id, CastingTime = "1 action", Range = "Touch", Components = SpellComponents.VerbalSomatic, Duration = "1 minute", IsConcentration = true, Description = "You or a creature you touch becomes invisible until the spell ends. Anything the target is wearing or carrying is invisible as long as it is on the target's person." },
             ["Bard", "Sorcerer", "Wizard"], []),
            (new Spell { Name = "Cone of Cold", Level = 5, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "Self (60-foot cone)", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a small crystal or glass cone", Duration = "Instantaneous", Description = "A blast of cold air erupts from your hands. Each creature in a 60-foot cone must make a Constitution saving throw. A creature takes 8d8 cold damage on a failed save, or half as much damage on a successful one.", AtHigherLevels = "When you cast this spell using a spell slot of 6th level or higher, the damage increases by 1d8 for each slot level above 5th." },
             ["Sorcerer", "Wizard"], [("Cold", "8d8")]),
            (new Spell { Name = "Mass Cure Wounds", Level = 5, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "60 feet", Components = SpellComponents.VerbalSomatic, Duration = "Instantaneous", Description = "A wave of healing energy washes out from a point of your choice within range. Choose up to six creatures in a 30-foot-radius sphere centered on that point. Each target regains hit points equal to 3d8 + your spellcasting ability modifier.", AtHigherLevels = "When you cast this spell using a spell slot of 6th level or higher, the healing increases by 1d8 for each slot level above 5th." },
             ["Bard", "Cleric", "Druid"], []),
            (new Spell { Name = "Raise Dead", Level = 5, SchoolId = schools["Necromancy"].Id, CastingTime = "1 hour", Range = "Touch", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a diamond worth at least 500 gp, which the spell consumes", Duration = "Instantaneous", Description = "You return a dead creature you touch to life, provided that it has been dead no longer than 10 days. If the creature's soul is both willing and at liberty to rejoin the body, the creature returns to life with 1 hit point." },
             ["Bard", "Cleric", "Paladin"], []),
            (new Spell { Name = "Wall of Fire", Level = 4, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "120 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a small piece of phosphorus", Duration = "1 minute", IsConcentration = true, Description = "You create a wall of fire on a solid surface within range. The wall can be up to 60 feet long, 20 feet high, and 1 foot thick, or a ringed wall up to 20 feet in diameter, 20 feet high, and 1 foot thick. The wall is opaque and lasts for the duration. When the wall appears, each creature within its area must make a Dexterity saving throw.", AtHigherLevels = "When you cast this spell using a spell slot of 5th level or higher, the damage increases by 1d8 for each slot level above 4th." },
             ["Druid", "Sorcerer", "Wizard"], [("Fire", "5d8")]),
            (new Spell { Name = "Heroism", Level = 1, SchoolId = schools["Enchantment"].Id, CastingTime = "1 action", Range = "Touch", Components = SpellComponents.VerbalSomatic, Duration = "1 minute", IsConcentration = true, Description = "A willing creature you touch is imbued with bravery. Until the spell ends, the creature is immune to being frightened and gains temporary hit points equal to your spellcasting ability modifier at the start of each of its turns.", AtHigherLevels = "When you cast this spell using a spell slot of 2nd level or higher, you can target one additional creature for each slot level above 1st." },
             ["Bard", "Paladin"], []),
            (new Spell { Name = "Inflict Wounds", Level = 1, SchoolId = schools["Necromancy"].Id, CastingTime = "1 action", Range = "Touch", Components = SpellComponents.VerbalSomatic, Duration = "Instantaneous", Description = "Make a melee spell attack against a creature you can reach. On a hit, the target takes 3d10 necrotic damage.", AtHigherLevels = "When you cast this spell using a spell slot of 2nd level or higher, the damage increases by 1d10 for each slot level above 1st." },
             ["Cleric"], [("Necrotic", "3d10")]),
            (new Spell { Name = "Ice Storm", Level = 4, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "300 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a pinch of dust and a few drops of water", Duration = "Instantaneous", Description = "A hail of rock-hard ice pounds to the ground in a 20-foot-radius, 40-foot-high cylinder centered on a point within range. Each creature in the cylinder must make a Dexterity saving throw. A creature takes 2d8 bludgeoning damage and 4d6 cold damage on a failed save, or half as much damage on a successful one.", AtHigherLevels = "When you cast this spell using a spell slot of 5th level or higher, the bludgeoning damage increases by 1d8 for each slot level above 4th." },
             ["Druid", "Sorcerer", "Wizard"], [("Bludgeoning", "2d8"), ("Cold", "4d6")]),
            (new Spell { Name = "Entangle", Level = 1, SchoolId = schools["Conjuration"].Id, CastingTime = "1 action", Range = "90 feet", Components = SpellComponents.VerbalSomatic, Duration = "1 minute", IsConcentration = true, Description = "Grasping weeds and vines sprout from the ground in a 20-foot square starting from a point within range. For the duration, these plants turn the ground in the area into difficult terrain. A creature in the area when you cast the spell must succeed on a Strength saving throw or be restrained by the entangling plants until the spell ends." },
             ["Druid"], []),
            (new Spell { Name = "Longstrider", Level = 1, SchoolId = schools["Transmutation"].Id, CastingTime = "1 action", Range = "Touch", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a pinch of dirt", Duration = "1 hour", Description = "You touch a creature. The target's speed increases by 10 feet until the spell ends.", AtHigherLevels = "When you cast this spell using a spell slot of 2nd level or higher, you can target one additional creature for each level above 1st." },
             ["Bard", "Druid", "Ranger", "Wizard"], []),
            (new Spell { Name = "Hunter's Mark", Level = 1, SchoolId = schools["Divination"].Id, CastingTime = "1 bonus action", Range = "90 feet", Components = SpellComponents.Verbal, Duration = "1 hour", IsConcentration = true, Description = "You choose a creature you can see within range and mystically mark it as your quarry. Until the spell ends, you deal an extra 1d6 damage to the target whenever you hit it with a weapon attack, and you have advantage on any Wisdom (Perception) or Wisdom (Survival) check you make to find it.", AtHigherLevels = "When you cast this spell using a spell slot of 3rd or 4th level, you can maintain your concentration on the spell for up to 8 hours. When you use a spell slot of 5th level or higher, you can maintain your concentration on the spell for up to 24 hours." },
             ["Ranger"], []),
            (new Spell { Name = "Sanctuary", Level = 1, SchoolId = schools["Abjuration"].Id, CastingTime = "1 bonus action", Range = "30 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a small silver mirror", Duration = "1 minute", Description = "You ward a creature within range against attack. Until the spell ends, any creature who targets the warded creature with an attack or a harmful spell must first make a Wisdom saving throw. On a failed save, the creature must choose a new target or lose the attack or spell." },
             ["Cleric"], []),
            (new Spell { Name = "Shatter", Level = 2, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "60 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a chip of mica", Duration = "Instantaneous", Description = "A sudden loud ringing noise, painfully intense, erupts from a point of your choice within range. Each creature in a 10-foot-radius sphere centered on that point must make a Constitution saving throw. A creature takes 3d8 thunder damage on a failed save, or half as much damage on a successful one.", AtHigherLevels = "When you cast this spell using a spell slot of 3rd level or higher, the damage increases by 1d8 for each slot level above 2nd." },
             ["Bard", "Sorcerer", "Warlock", "Wizard"], [("Thunder", "3d8")]),
            (new Spell { Name = "Web", Level = 2, SchoolId = schools["Conjuration"].Id, CastingTime = "1 action", Range = "60 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a bit of spiderweb", Duration = "1 hour", IsConcentration = true, Description = "You conjure a mass of thick, sticky webbing at a point of your choice within range. The webs fill a 20-foot cube from that point for the duration. The webs are difficult terrain and lightly obscure their area. Each creature that starts its turn in the webs or that enters them during its turn must make a Dexterity saving throw." },
             ["Sorcerer", "Wizard"], []),
            (new Spell { Name = "Darkness", Level = 2, SchoolId = schools["Evocation"].Id, CastingTime = "1 action", Range = "60 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "bat fur and a drop of pitch or piece of coal", Duration = "10 minutes", IsConcentration = true, Description = "Magical darkness spreads from a point you choose within range to fill a 15-foot-radius sphere for the duration. The darkness spreads around corners. A creature with darkvision can't see through this magical darkness, and nonmagical light can't illuminate it." },
             ["Sorcerer", "Warlock", "Wizard"], []),
            (new Spell { Name = "Dispel Magic", Level = 3, SchoolId = schools["Abjuration"].Id, CastingTime = "1 action", Range = "120 feet", Components = SpellComponents.VerbalSomatic, Duration = "Instantaneous", Description = "Choose one creature, object, or magical effect within range. Any spell of 3rd level or lower on the target ends. For each spell of 4th level or higher on the target, make an ability check using your spellcasting ability.", AtHigherLevels = "When you cast this spell using a spell slot of 4th level or higher, you automatically end the effects of a spell on the target if the spell's level is equal to or less than the level of the spell slot you used." },
             ["Bard", "Cleric", "Druid", "Paladin", "Sorcerer", "Warlock", "Wizard"], []),
            (new Spell { Name = "Animate Dead", Level = 3, SchoolId = schools["Necromancy"].Id, CastingTime = "1 minute", Range = "10 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a drop of blood, a piece of flesh, and a pinch of bone dust", Duration = "Instantaneous", Description = "This spell creates an undead servant. Choose a pile of bones or a corpse of a Medium or Small humanoid within range. Your spell imbues the target with a foul mimicry of life, raising it as an undead creature. The target becomes a skeleton if you chose bones or a zombie if you chose a corpse.", AtHigherLevels = "When you cast this spell using a spell slot of 4th level or higher, you animate or reassert control over two additional undead creatures for each slot level above 3rd." },
             ["Cleric", "Wizard"], []),
            (new Spell { Name = "Fly", Level = 3, SchoolId = schools["Transmutation"].Id, CastingTime = "1 action", Range = "Touch", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a wing feather from any bird", Duration = "10 minutes", IsConcentration = true, Description = "You touch a willing creature. The target gains a flying speed of 60 feet for the duration. When the spell ends, the target falls if it is still aloft, unless it can stop the fall.", AtHigherLevels = "When you cast this spell using a spell slot of 4th level or higher, you can target one additional creature for each slot level above 3rd." },
             ["Sorcerer", "Warlock", "Wizard"], []),
            (new Spell { Name = "Mass Suggestion", Level = 6, SchoolId = schools["Enchantment"].Id, CastingTime = "1 action", Range = "60 feet", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a snake's tongue and either a bit of honeycomb or a drop of sweet oil", Duration = "24 hours", Description = "You suggest a course of activity (limited to a sentence or two) and magically influence up to twelve creatures of your choice that you can see within range and that can hear and understand you.", AtHigherLevels = "When you cast this spell using a 7th-level spell slot, the duration is 10 days. With an 8th-level spell slot, the duration is 30 days. With a 9th-level spell slot, the duration is a year and a day." },
             ["Bard", "Sorcerer", "Warlock", "Wizard"], []),
            (new Spell { Name = "True Resurrection", Level = 9, SchoolId = schools["Necromancy"].Id, CastingTime = "1 hour", Range = "Touch", Components = SpellComponents.VerbalSomaticMaterial, MaterialComponent = "a sprinkle of holy water and diamonds worth at least 25,000 gp, which the spell consumes", Duration = "Instantaneous", Description = "You touch a creature that has been dead for no longer than 200 years and that died for any reason except old age. If the creature's soul is free and willing, the creature is restored to life with all its hit points." },
             ["Cleric", "Druid"], []),
            (new Spell { Name = "Wish", Level = 9, SchoolId = schools["Conjuration"].Id, CastingTime = "1 action", Range = "Self", Components = SpellComponents.Verbal, Duration = "Instantaneous", Description = "Wish is the mightiest spell a mortal creature can cast. By simply speaking aloud, you can alter the very foundations of reality in accord with your desires." },
             ["Sorcerer", "Wizard"], []),
            (new Spell { Name = "Power Word Kill", Level = 9, SchoolId = schools["Enchantment"].Id, CastingTime = "1 action", Range = "60 feet", Components = SpellComponents.Verbal, Duration = "Instantaneous", Description = "You utter a word of power that can compel one creature you can see within range to die instantly. If the creature you choose has 100 hit points or fewer, it dies. Otherwise, the spell has no effect." },
             ["Sorcerer", "Warlock", "Wizard"], []),
        ];

        foreach (var (spell, classNames, damages) in spells)
        {
            _db.Spells.Add(spell);
            await _db.SaveChangesAsync();

            foreach (var cn in classNames)
                if (classes.TryGetValue(cn, out var cls))
                    _db.SpellClasses.Add(new SpellClass { SpellId = spell.Id, ClassId = cls.Id });

            foreach (var (typeName, dice) in damages)
                if (damageTypes.TryGetValue(typeName, out var dt))
                    _db.SpellDamages.Add(new SpellDamage { SpellId = spell.Id, DamageTypeId = dt.Id, DamageDice = dice });

            await _db.SaveChangesAsync();
        }
    }

    private async Task SeedItemsAsync()
    {
        var damageTypes = await _db.DamageTypes.ToDictionaryAsync(d => d.Name);
        var properties = await _db.WeaponProperties.ToDictionaryAsync(p => p.Name);

        var itemsToCreate = new (string name, string desc, decimal weight, int costCp, ItemType type, WeaponInput? weapon, ArmorInput? armor)[]
        {
            ("Longsword", "A versatile martial melee weapon.", 3, 1500, ItemType.Weapon,
             new WeaponInput { DamageDice = "1d8", DamageTypeId = 0, IsRanged = false, IsMartial = true, PropertyIds = [] }, null),
            ("Shortsword", "A simple light martial melee weapon.", 2, 1000, ItemType.Weapon,
             new WeaponInput { DamageDice = "1d6", DamageTypeId = 0, IsRanged = false, IsMartial = true, PropertyIds = [] }, null),
            ("Dagger", "A simple light melee weapon that can be thrown.", 1, 200, ItemType.Weapon,
             new WeaponInput { DamageDice = "1d4", DamageTypeId = 0, IsRanged = false, IsMartial = false, NormalRangeFt = 20, LongRangeFt = 60, PropertyIds = [] }, null),
            ("Greataxe", "A heavy two-handed martial melee weapon.", 7, 3000, ItemType.Weapon,
             new WeaponInput { DamageDice = "1d12", DamageTypeId = 0, IsRanged = false, IsMartial = true, PropertyIds = [] }, null),
            ("Handaxe", "A simple thrown weapon.", 2, 500, ItemType.Weapon,
             new WeaponInput { DamageDice = "1d6", DamageTypeId = 0, IsRanged = false, IsMartial = false, NormalRangeFt = 20, LongRangeFt = 60, PropertyIds = [] }, null),
            ("Shortbow", "A simple ranged weapon.", 2, 2500, ItemType.Weapon,
             new WeaponInput { DamageDice = "1d6", DamageTypeId = 0, IsRanged = true, IsMartial = false, NormalRangeFt = 80, LongRangeFt = 320, PropertyIds = [] }, null),
            ("Longbow", "A martial ranged weapon.", 2, 5000, ItemType.Weapon,
             new WeaponInput { DamageDice = "1d8", DamageTypeId = 0, IsRanged = true, IsMartial = true, NormalRangeFt = 150, LongRangeFt = 600, PropertyIds = [] }, null),
            ("Crossbow, Light", "A simple ranged weapon with a trigger mechanism.", 5, 2500, ItemType.Weapon,
             new WeaponInput { DamageDice = "1d8", DamageTypeId = 0, IsRanged = true, IsMartial = false, NormalRangeFt = 80, LongRangeFt = 320, PropertyIds = [] }, null),
            ("Quarterstaff", "A simple two-handed melee weapon.", 4, 20, ItemType.Weapon,
             new WeaponInput { DamageDice = "1d6", DamageTypeId = 0, IsRanged = false, IsMartial = false, PropertyIds = [] }, null),
            ("Rapier", "An elegant martial melee weapon.", 2, 2500, ItemType.Weapon,
             new WeaponInput { DamageDice = "1d8", DamageTypeId = 0, IsRanged = false, IsMartial = true, PropertyIds = [] }, null),
            ("Leather Armor", "Light armor made of leather.", 10, 1000, ItemType.Armor,
             null, new ArmorInput { ArmorType = ArmorType.Light, BaseAC = 11, MaxDexBonus = null, MinStrength = 0, StealthDisadvantage = false }),
            ("Chain Mail", "Heavy interlocked metal rings.", 55, 7500, ItemType.Armor,
             null, new ArmorInput { ArmorType = ArmorType.Heavy, BaseAC = 16, MaxDexBonus = 0, MinStrength = 13, StealthDisadvantage = true }),
            ("Plate Armor", "Full suit of articulated plate.", 65, 150000, ItemType.Armor,
             null, new ArmorInput { ArmorType = ArmorType.Heavy, BaseAC = 18, MaxDexBonus = 0, MinStrength = 15, StealthDisadvantage = true }),
            ("Shield", "A wooden or metal shield.", 6, 1000, ItemType.Armor,
             null, new ArmorInput { ArmorType = ArmorType.Shield, BaseAC = 2, MaxDexBonus = null, MinStrength = 0, StealthDisadvantage = false }),
            ("Studded Leather Armor", "Light armor reinforced with metal studs.", 13, 4500, ItemType.Armor,
             null, new ArmorInput { ArmorType = ArmorType.Light, BaseAC = 12, MaxDexBonus = null, MinStrength = 0, StealthDisadvantage = false }),
            ("Scale Mail", "Medium armor of overlapping metal scales.", 45, 5000, ItemType.Armor,
             null, new ArmorInput { ArmorType = ArmorType.Medium, BaseAC = 14, MaxDexBonus = 2, MinStrength = 0, StealthDisadvantage = true }),
            ("Half Plate", "Medium armor of shaped metal plates.", 40, 75000, ItemType.Armor,
             null, new ArmorInput { ArmorType = ArmorType.Medium, BaseAC = 15, MaxDexBonus = 2, MinStrength = 0, StealthDisadvantage = true }),
            ("Backpack", "A leather pack for carrying gear.", 5, 200, ItemType.Gear, null, null),
            ("Rope, Hempen (50 feet)", "Strong rope made of hemp.", 10, 100, ItemType.Gear, null, null),
            ("Torch", "A wooden stick with a flammable tip.", 1, 1, ItemType.Gear, null, null),
            ("Healing Potion", "A vial of red liquid that restores 2d4+2 hit points when consumed.", 0.5m, 5000, ItemType.Gear, null, null),
            ("Thieves' Tools", "Includes lock picks and other tools for opening locks.", 1, 2500, ItemType.Gear, null, null),
            ("Arcane Focus", "A special item — an orb, crystal, rod, staff, or wand — that channels magical energy.", 1, 1000, ItemType.Gear, null, null),
            ("Holy Symbol", "An emblem of a deity used by clerics and paladins as a spellcasting focus.", 1, 500, ItemType.Gear, null, null),
            ("Rations (1 day)", "Compact dry foods suitable for extended travel.", 2, 50, ItemType.Gear, null, null),
        };

        var slashId = damageTypes["Slashing"].Id;
        var pierceId = damageTypes["Piercing"].Id;
        var bludgeId = damageTypes["Bludgeoning"].Id;

        // Assign damage types by item name pattern
        static int GetDmgType(string name, int slash, int pierce, int bludge) => name switch
        {
            var n when n.Contains("axe") || n.Contains("sword") || n.Contains("Rapier") => slash,
            var n when n.Contains("bow") || n.Contains("arrow") || n.Contains("Crossbow") || n.Contains("Dagger") || n.Contains("Short") => pierce,
            _ => bludge
        };

        foreach (var (name, desc, weight, costCp, itemType, weapon, armor) in itemsToCreate)
        {
            var item = new Item { Name = name, Description = desc, WeightLbs = weight, CostCp = costCp, ItemType = itemType };
            _db.Items.Add(item);
            await _db.SaveChangesAsync();

            if (weapon != null)
            {
                var dmgTypeId = GetDmgType(name, slashId, pierceId, bludgeId);
                var w = new Weapon { ItemId = item.Id, DamageDice = weapon.DamageDice, DamageTypeId = dmgTypeId, IsRanged = weapon.IsRanged, IsMartial = weapon.IsMartial, NormalRangeFt = weapon.NormalRangeFt, LongRangeFt = weapon.LongRangeFt };
                _db.Weapons.Add(w);
                await _db.SaveChangesAsync();

                if (weapon.IsRanged && properties.TryGetValue("Ammunition", out var ammoP))
                    _db.WeaponPropertyAssignments.Add(new WeaponPropertyAssignment { WeaponId = w.Id, WeaponPropertyId = ammoP.Id });
                if (!weapon.IsRanged && name.Contains("Dagger") && properties.TryGetValue("Thrown", out var thrownP))
                    _db.WeaponPropertyAssignments.Add(new WeaponPropertyAssignment { WeaponId = w.Id, WeaponPropertyId = thrownP.Id });
                if (!weapon.IsRanged && name.Contains("Dagger") && properties.TryGetValue("Light", out var lightP))
                    _db.WeaponPropertyAssignments.Add(new WeaponPropertyAssignment { WeaponId = w.Id, WeaponPropertyId = lightP.Id });
                if (name.Contains("Rapier") && properties.TryGetValue("Finesse", out var finesseP))
                    _db.WeaponPropertyAssignments.Add(new WeaponPropertyAssignment { WeaponId = w.Id, WeaponPropertyId = finesseP.Id });
            }

            if (armor != null)
                _db.Armors.Add(new ArmorItem { ItemId = item.Id, ArmorType = armor.ArmorType, BaseAC = armor.BaseAC, MaxDexBonus = armor.MaxDexBonus, MinStrength = armor.MinStrength, StealthDisadvantage = armor.StealthDisadvantage });

            await _db.SaveChangesAsync();
        }
    }

    private async Task SeedMonstersAsync()
    {
        var creatureTypes = await _db.CreatureTypes.ToDictionaryAsync(c => c.Name);
        var sizes = await _db.CreatureSizes.ToDictionaryAsync(s => s.Name);
        var alignments = await _db.Alignments.ToDictionaryAsync(a => a.Abbreviation);
        var abilityScores = await _db.AbilityScores.ToDictionaryAsync(a => a.Abbreviation);
        var damageTypes = await _db.DamageTypes.ToDictionaryAsync(d => d.Name);
        var conditions = await _db.Conditions.ToDictionaryAsync(c => c.Name);

        MonsterSeed[] monstersData = [
            new("Goblin", "Humanoid", "Small", "NE", 15, 7, "2d6", 30, 0.25m, 50, 2,
                new(){{"STR",8},{"DEX",14},{"CON",10},{"INT",10},{"WIS",8},{"CHA",8}},
                [("Nimble Escape", "The goblin can take the Disengage or Hide action as a bonus action on each of its turns.")],
                [new("Scimitar", "Melee Weapon Attack: +4 to hit, reach 5 ft., one target. Hit: 5 (1d6 + 2) slashing damage.", ActionType.Action, 4, "5 ft.", "1d6+2", "Slashing", null, null)],
                [], [], [], false),
            new("Skeleton", "Undead", "Medium", "LE", 13, 13, "2d8+4", 30, 0.25m, 50, 2,
                new(){{"STR",10},{"DEX",14},{"CON",15},{"INT",6},{"WIS",8},{"CHA",5}}, [],
                [new("Shortsword","Melee Weapon Attack: +4 to hit, reach 5 ft., one target. Hit: 5 (1d6 + 2) piercing damage.",ActionType.Action,4,"5 ft.","1d6+2","Piercing",null,null),
                 new("Shortbow","Ranged Weapon Attack: +4 to hit, range 80/320 ft., one target. Hit: 5 (1d6 + 2) piercing damage.",ActionType.Action,4,"80/320 ft.","1d6+2","Piercing",null,null)],
                ["Poison"], [], ["Exhaustion","Poisoned"], false),
            new("Zombie", "Undead", "Medium", "NE", 8, 22, "3d8+9", 20, 0.25m, 50, 2,
                new(){{"STR",13},{"DEX",6},{"CON",16},{"INT",3},{"WIS",6},{"CHA",5}},
                [("Undead Fortitude","If damage reduces the zombie to 0 hit points, it must make a Constitution saving throw with a DC of 5 + the damage taken, unless the damage is radiant or from a critical hit. On a success, the zombie drops to 1 hit point instead.")],
                [new("Slam","Melee Weapon Attack: +3 to hit, reach 5 ft., one target. Hit: 4 (1d6 + 1) bludgeoning damage.",ActionType.Action,3,"5 ft.","1d6+1","Bludgeoning",null,null)],
                ["Poison"], [], ["Poisoned"], false),
            new("Giant Spider", "Beast", "Large", "N", 14, 26, "4d10+4", 30, 1m, 200, 2,
                new(){{"STR",14},{"DEX",16},{"CON",12},{"INT",2},{"WIS",11},{"CHA",4}},
                [("Spider Climb","The spider can climb difficult surfaces, including upside down on ceilings, without needing to make an ability check."),("Web Sense","While in contact with a web, the spider knows the exact location of any other creature in contact with the same web."),("Web Walker","The spider ignores movement restrictions caused by webbing.")],
                [new("Bite","Melee Weapon Attack: +5 to hit, reach 5 ft., one creature. Hit: 7 (1d8 + 3) piercing damage, and the target must make a DC 11 Constitution saving throw, taking 9 (2d8) poison damage on a failed save.",ActionType.Action,5,"5 ft.","1d8+3","Piercing",null,null),
                 new("Web","Ranged Weapon Attack: +5 to hit, range 30/60 ft., one creature. The target is restrained by webbing.",ActionType.Action,5,"30/60 ft.",null,null,null,null)],
                [], [], [], false),
            new("Troll", "Giant", "Large", "CE", 15, 84, "8d10+40", 30, 5m, 1800, 3,
                new(){{"STR",18},{"DEX",13},{"CON",20},{"INT",7},{"WIS",9},{"CHA",7}},
                [("Keen Smell","The troll has advantage on Wisdom (Perception) checks that rely on smell."),("Regeneration","The troll regains 10 hit points at the start of its turn. If the troll takes acid or fire damage, this trait doesn't function at the start of the troll's next turn.")],
                [new("Multiattack","The troll makes three attacks: one with its bite and two with its claws.",ActionType.Action,null,null,null,null,null,null),
                 new("Bite","Melee Weapon Attack: +7 to hit, reach 5 ft., one target. Hit: 7 (1d6 + 4) piercing damage.",ActionType.Action,7,"5 ft.","1d6+4","Piercing",null,null),
                 new("Claw","Melee Weapon Attack: +7 to hit, reach 5 ft., one target. Hit: 11 (2d6 + 4) slashing damage.",ActionType.Action,7,"5 ft.","2d6+4","Slashing",null,null)],
                [], [], [], false),
            new("Adult Red Dragon", "Dragon", "Huge", "CE", 19, 256, "19d12+133", 40, 17m, 18000, 6,
                new(){{"STR",27},{"DEX",10},{"CON",25},{"INT",16},{"WIS",13},{"CHA",21}},
                [("Legendary Resistance (3/Day)","If the dragon fails a saving throw, it can choose to succeed instead.")],
                [new("Multiattack","The dragon can use its Frightful Presence. It then makes three attacks: one with its bite and two with its claws.",ActionType.Action,null,null,null,null,null,null),
                 new("Bite","Melee Weapon Attack: +14 to hit, reach 10 ft., one target. Hit: 19 (2d10 + 8) piercing damage plus 7 (2d6) fire damage.",ActionType.Action,14,"10 ft.","2d10+8","Piercing",null,null),
                 new("Claw","Melee Weapon Attack: +14 to hit, reach 5 ft., one target. Hit: 15 (2d6 + 8) slashing damage.",ActionType.Action,14,"5 ft.","2d6+8","Slashing",null,null),
                 new("Fire Breath","The dragon exhales fire in a 60-foot cone. Each creature in that area must make a DC 21 Dexterity saving throw, taking 63 (18d6) fire damage on a failed save, or half as much damage on a successful one.",ActionType.Action,null,"60-foot cone","18d6","Fire",21,"DEX")],
                ["Fire"], [], [], true),
            new("Lich", "Undead", "Medium", "NE", 17, 135, "18d8+54", 30, 21m, 33000, 7,
                new(){{"STR",11},{"DEX",16},{"CON",16},{"INT",20},{"WIS",14},{"CHA",16}},
                [("Legendary Resistance (3/Day)","If the lich fails a saving throw, it can choose to succeed instead."),("Rejuvenation","If it has a phylactery, a destroyed lich gains a new body in 1d10 days, regaining all its hit points and becoming active again."),("Turn Resistance","The lich has advantage on saving throws against any effect that turns undead.")],
                [new("Paralyzing Touch","Melee Spell Attack: +12 to hit, reach 5 ft., one creature. Hit: 10 (3d6) cold damage. The target must succeed on a DC 18 Constitution saving throw or be paralyzed for 1 minute.",ActionType.Action,12,"5 ft.","3d6","Cold",18,"CON")],
                ["Cold","Lightning","Necrotic","Poison"], ["Bludgeoning","Piercing","Slashing"], ["Charmed","Exhaustion","Frightened","Paralyzed","Poisoned"], true),
            new("Owlbear", "Monstrosity", "Large", "N", 13, 59, "7d10+21", 40, 3m, 700, 2,
                new(){{"STR",20},{"DEX",12},{"CON",17},{"INT",3},{"WIS",12},{"CHA",7}},
                [("Keen Sight and Smell","The owlbear has advantage on Wisdom (Perception) checks that rely on sight or smell.")],
                [new("Multiattack","The owlbear makes two attacks: one with its beak and one with its claws.",ActionType.Action,null,null,null,null,null,null),
                 new("Beak","Melee Weapon Attack: +7 to hit, reach 5 ft., one creature. Hit: 10 (1d10 + 5) piercing damage.",ActionType.Action,7,"5 ft.","1d10+5","Piercing",null,null),
                 new("Claws","Melee Weapon Attack: +7 to hit, reach 5 ft., one target. Hit: 14 (2d8 + 5) slashing damage.",ActionType.Action,7,"5 ft.","2d8+5","Slashing",null,null)],
                [], [], [], false),
            new("Vampire", "Undead", "Medium", "LE", 16, 144, "17d8+68", 30, 13m, 10000, 5,
                new(){{"STR",18},{"DEX",18},{"CON",18},{"INT",17},{"WIS",15},{"CHA",18}},
                [("Legendary Resistance (3/Day)","If the vampire fails a saving throw, it can choose to succeed instead."),("Misty Escape","When it drops to 0 hit points outside its resting place, the vampire transforms into a cloud of mist instead of falling unconscious."),("Regeneration","The vampire regains 20 hit points at the start of its turn if it has at least 1 hit point and isn't in sunlight or running water."),("Spider Climb","The vampire can climb difficult surfaces, including upside down on ceilings, without needing to make an ability check.")],
                [new("Multiattack","The vampire makes two attacks, only one of which can be a bite attack.",ActionType.Action,null,null,null,null,null,null),
                 new("Unarmed Strike","Melee Weapon Attack: +9 to hit, reach 5 ft., one creature. Hit: 8 (1d8 + 4) bludgeoning damage.",ActionType.Action,9,"5 ft.","1d8+4","Bludgeoning",null,null),
                 new("Bite","Melee Weapon Attack: +9 to hit, reach 5 ft., one willing creature. Hit: 7 (1d6 + 4) piercing damage plus 10 (3d6) necrotic damage.",ActionType.Action,9,"5 ft.","1d6+4","Piercing",null,null)],
                [], ["Necrotic","Bludgeoning","Piercing","Slashing"], [], true),
            new("Beholder", "Aberration", "Large", "LE", 18, 180, "19d10+76", 0, 13m, 10000, 5,
                new(){{"STR",10},{"DEX",14},{"CON",18},{"INT",17},{"WIS",15},{"CHA",17}},
                [("Antimagic Cone","The beholder's central eye creates an area of antimagic, as in the antimagic field spell, in a 150-foot cone."),("Legendary Resistance (3/Day)","If the beholder fails a saving throw, it can choose to succeed instead.")],
                [new("Bite","Melee Weapon Attack: +5 to hit, reach 5 ft., one target. Hit: 14 (4d6) piercing damage.",ActionType.Action,5,"5 ft.","4d6","Piercing",null,null),
                 new("Eye Rays","The beholder shoots three of the following magical eye rays at random, choosing 1-3 targets it can see within 120 feet of it.",ActionType.Action,null,null,null,null,null,null)],
                [], [], [], true),
            new("Dire Wolf", "Beast", "Large", "N", 14, 37, "5d10+10", 50, 1m, 200, 2,
                new(){{"STR",17},{"DEX",15},{"CON",15},{"INT",3},{"WIS",12},{"CHA",7}},
                [("Keen Hearing and Smell","The wolf has advantage on Wisdom (Perception) checks that rely on hearing or smell."),("Pack Tactics","The wolf has advantage on attack rolls against a creature if at least one of the wolf's allies is adjacent to the creature and the ally isn't incapacitated.")],
                [new("Bite","Melee Weapon Attack: +5 to hit, reach 5 ft., one target. Hit: 10 (2d6 + 3) piercing damage. If the target is a creature, it must succeed on a DC 13 Strength saving throw or be knocked prone.",ActionType.Action,5,"5 ft.","2d6+3","Piercing",13,"STR")],
                [], [], [], false),
            new("Bandit", "Humanoid", "Medium", "CN", 12, 11, "2d8+2", 30, 0.125m, 25, 2,
                new(){{"STR",11},{"DEX",12},{"CON",12},{"INT",10},{"WIS",10},{"CHA",10}}, [],
                [new("Scimitar","Melee Weapon Attack: +3 to hit, reach 5 ft., one target. Hit: 4 (1d6 + 1) slashing damage.",ActionType.Action,3,"5 ft.","1d6+1","Slashing",null,null),
                 new("Light Crossbow","Ranged Weapon Attack: +3 to hit, range 80/320 ft., one target. Hit: 5 (1d8 + 1) piercing damage.",ActionType.Action,3,"80/320 ft.","1d8+1","Piercing",null,null)],
                [], [], [], false),
        ];

        foreach (var md in monstersData)
        {
            var alignment = alignments.GetValueOrDefault(md.Alignment);
            var monster = new Monster
            {
                Name = md.Name,
                CreatureTypeId = creatureTypes[md.Type].Id,
                SizeId = sizes[md.Size].Id,
                AlignmentId = alignment?.Id,
                AC = md.AC, MaxHP = md.HP, HitDice = md.HitDice,
                WalkSpeed = md.Walk, ChallengeRating = md.CR, XP = md.XP,
                ProficiencyBonus = md.Prof, IsLegendary = md.IsLegendary,
                LegendaryActionCount = md.IsLegendary ? 3 : 0,
                ACSource = md.Type is "Dragon" or "Undead" ? "natural armor" : "armor"
            };
            _db.Monsters.Add(monster);
            await _db.SaveChangesAsync();

            foreach (var (abbr, score) in md.Scores)
                _db.MonsterAbilityScores.Add(new MonsterAbilityScore { MonsterId = monster.Id, AbilityScoreId = abilityScores[abbr].Id, Score = score });

            foreach (var (name, desc) in md.Traits)
                _db.MonsterTraits.Add(new MonsterTrait { MonsterId = monster.Id, Name = name, Description = desc });

            foreach (var a in md.Actions)
            {
                int? dmgTypeId = a.DamageType != null && damageTypes.TryGetValue(a.DamageType, out var dt) ? dt.Id : null;
                int? saveAbilId = a.SaveAbility != null && abilityScores.TryGetValue(a.SaveAbility, out var sa) ? sa.Id : null;
                _db.MonsterActions.Add(new MonsterAction { MonsterId = monster.Id, Name = a.Name, Description = a.Description, ActionType = a.ActionType, AttackBonus = a.AttackBonus, ReachOrRange = a.ReachOrRange, HitDamageDice = a.HitDamageDice, HitDamageTypeId = dmgTypeId, SaveDC = a.SaveDC, SaveAbilityScoreId = saveAbilId });
            }

            foreach (var dmg in md.Immunities)
                if (damageTypes.TryGetValue(dmg, out var dt))
                    _db.MonsterDamageModifiers.Add(new MonsterDamageModifier { MonsterId = monster.Id, DamageTypeId = dt.Id, ModifierType = DamageModifierType.Immunity });

            foreach (var dmg in md.Resistances)
                if (damageTypes.TryGetValue(dmg, out var dt))
                    _db.MonsterDamageModifiers.Add(new MonsterDamageModifier { MonsterId = monster.Id, DamageTypeId = dt.Id, ModifierType = DamageModifierType.Resistance });

            foreach (var cond in md.ConditionImmunities)
                if (conditions.TryGetValue(cond, out var c))
                    _db.MonsterConditionImmunities.Add(new MonsterConditionImmunity { MonsterId = monster.Id, ConditionId = c.Id });

            await _db.SaveChangesAsync();
        }
    }

    private async Task SeedBackgroundsAsync()
    {
        var srdBackgrounds = new[]
        {
            new
            {
                Name = "Acolyte",
                Description = "You have spent your life in the service of a temple to a specific god or pantheon of gods. You act as an intermediary between the realm of the holy and the mortal world, performing sacred rites and offering sacrifices in order to conduct worshipers into the presence of the divine.",
                LanguagesGranted = 2,
                ToolProficiency = "",
                StartingEquipment = "A holy symbol, a prayer book or prayer wheel, 5 sticks of incense, vestments, a set of common clothes, and a pouch containing 15 gp",
                Skills = new[] { "Insight", "Religion" },
                Features = new[] { ("Shelter of the Faithful", "As an acolyte, you command the respect of those who share your faith, and you can perform the religious ceremonies of your deity. You and your adventuring companions can expect to receive free healing and care at a temple, shrine, or other established presence of your faith, though you must provide any material components needed for spells. Those who share your religion will support you (but only you) at a modest lifestyle. You might also have ties to a specific temple dedicated to your chosen deity or pantheon, and you have a residence there.") }
            },
            new
            {
                Name = "Charlatan",
                Description = "You have always had a way with people. You know what makes them tick, you can tease out their heart's desires after a few minutes of conversation, and with a few weeks of work you can make them feel like they've known you their whole lives.",
                LanguagesGranted = 0,
                ToolProficiency = "Disguise kit, forgery kit",
                StartingEquipment = "A set of fine clothes, a disguise kit, tools of the con of your choice (ten stoppered bottles filled with colored liquid, a set of weighted dice, a deck of marked cards, or a signet ring of an imaginary duke), and a pouch containing 15 gp",
                Skills = new[] { "Deception", "Sleight of Hand" },
                Features = new[] { ("False Identity", "You have created a second identity that includes documentation, established acquaintances, and disguises that allow you to assume that persona. Additionally, you can forge documents including official papers and personal letters, as long as you have seen an example of the kind of document or the handwriting you are trying to copy.") }
            },
            new
            {
                Name = "Criminal",
                Description = "You are an experienced criminal with a history of breaking the law. You have spent a lot of time among other criminals and still have contacts within the criminal underworld.",
                LanguagesGranted = 0,
                ToolProficiency = "One type of gaming set, thieves' tools",
                StartingEquipment = "A crowbar, a set of dark common clothes including a hood, and a pouch containing 15 gp",
                Skills = new[] { "Deception", "Stealth" },
                Features = new[] { ("Criminal Contact", "You have a reliable and trustworthy contact who acts as your liaison to a network of other criminals. You know how to get messages to and from your contact, even over great distances; specifically, you know the local messengers, corrupt caravan masters, and seedy sailors who can deliver messages for you.") }
            },
            new
            {
                Name = "Entertainer",
                Description = "You thrive in front of an audience. You know how to entrance them, entertain them, and even inspire them. Your poetics can stir the hearts of those who hear you, awakening grief or joy, laughter or anger.",
                LanguagesGranted = 0,
                ToolProficiency = "Disguise kit, one type of musical instrument",
                StartingEquipment = "A musical instrument (one of your choice), the favor of an admirer (love letter, lock of hair, or trinket), a costume, and a pouch containing 15 gp",
                Skills = new[] { "Acrobatics", "Performance" },
                Features = new[] { ("By Popular Demand", "You can always find a place to perform, usually in an inn or tavern but possibly with a circus, at a theater, or even in a noble's court. At such a place, you receive free lodging and food of a modest or comfortable standard (depending on the quality of the establishment), as long as you perform each night.") }
            },
            new
            {
                Name = "Folk Hero",
                Description = "You come from a humble social rank, but you are destined for so much more. Already the people of your home village regard you as their champion, and your destiny calls you to stand against the tyrants and monsters that threaten the common folk everywhere.",
                LanguagesGranted = 0,
                ToolProficiency = "One type of artisan's tools, vehicles (land)",
                StartingEquipment = "A set of artisan's tools (one of your choice), a shovel, an iron pot, a set of common clothes, and a pouch containing 10 gp",
                Skills = new[] { "Animal Handling", "Survival" },
                Features = new[] { ("Rustic Hospitality", "Since you come from the ranks of the common folk, you fit in among them with ease. You can find a place to hide, rest, or recuperate among other commoners, unless you have shown yourself to be a danger to them. They will shield you from the law or anyone else searching for you, though they will not risk their lives for you.") }
            },
            new
            {
                Name = "Guild Artisan",
                Description = "You are a member of an artisan's guild, skilled in a particular field and closely associated with other artisans. You are a well-established part of the mercantile world, freed from the constraints of a feudal social order.",
                LanguagesGranted = 1,
                ToolProficiency = "One type of artisan's tools",
                StartingEquipment = "A set of artisan's tools (one of your choice), a letter of introduction from your guild, a set of traveler's clothes, and a pouch containing 15 gp",
                Skills = new[] { "Insight", "Persuasion" },
                Features = new[] { ("Guild Membership", "As an established and respected member of a guild, you can rely on certain benefits that membership provides. Your fellow guild members will provide you with lodging and food if necessary, and pay for your funeral if needed. In some cities and towns, a guildhall offers a central place to meet other members of your profession, which can be a good place to meet potential patrons, allies, or hirelings.") }
            },
            new
            {
                Name = "Hermit",
                Description = "You lived in seclusion—either in a sheltered community such as a monastery, or entirely alone—for a formative part of your life. In your time apart from the clamor of society, you found quiet, solitude, and perhaps some of the answers you were looking for.",
                LanguagesGranted = 1,
                ToolProficiency = "Herbalism kit",
                StartingEquipment = "A scroll case stuffed full of notes from your studies or prayers, a winter blanket, a set of common clothes, an herbalism kit, and 5 gp",
                Skills = new[] { "Medicine", "Religion" },
                Features = new[] { ("Discovery", "The quiet seclusion of your extended hermitage gave you access to a unique and powerful discovery. The exact nature of this revelation depends on the nature of your seclusion. It might be a great truth about the cosmos, the deities, the powerful beings of the outer planes, or the forces of nature.") }
            },
            new
            {
                Name = "Noble",
                Description = "You understand wealth, power, and privilege. You carry a noble title, and your family owns land, collects taxes, and wields significant political influence. You might be a pampered aristocrat unfamiliar with work or discomfort, a former merchant just elevated to the nobility, or a disinherited scoundrel with a disproportionate sense of entitlement.",
                LanguagesGranted = 1,
                ToolProficiency = "One type of gaming set",
                StartingEquipment = "A set of fine clothes, a signet ring, a scroll of pedigree, and a purse containing 25 gp",
                Skills = new[] { "History", "Persuasion" },
                Features = new[] { ("Position of Privilege", "Thanks to your noble birth, people are inclined to think the best of you. You are welcome in high society, and people assume you have the right to be wherever you are. The common folk make every effort to accommodate you and avoid your displeasure, and other people of high birth treat you as a member of the same social sphere.") }
            },
            new
            {
                Name = "Outlander",
                Description = "You grew up in the wilds, far from civilization and the comforts of town and technology. You've witnessed the migration of herds larger than forests, survived weather more extreme than any city-dweller could comprehend, and enjoyed the solitude of being the only thinking creature for miles in any direction.",
                LanguagesGranted = 1,
                ToolProficiency = "One type of musical instrument",
                StartingEquipment = "A staff, a hunting trap, a trophy from an animal you killed, a set of traveler's clothes, and a pouch containing 10 gp",
                Skills = new[] { "Athletics", "Survival" },
                Features = new[] { ("Wanderer", "You have an excellent memory for maps and geography, and you can always recall the general layout of terrain, settlements, and other features around you. In addition, you can find food and fresh water for yourself and up to five other people each day, provided that the land offers berries, small game, water, and so forth.") }
            },
            new
            {
                Name = "Sage",
                Description = "You spent years learning the lore of the multiverse. You scoured manuscripts, studied scrolls, and listened to the greatest experts on the subjects that interest you. Your efforts have made you a master in your fields of study.",
                LanguagesGranted = 2,
                ToolProficiency = "",
                StartingEquipment = "A bottle of black ink, a quill, a small knife, a letter from a dead colleague posing a question you have not yet been able to answer, a set of common clothes, and a pouch containing 10 gp",
                Skills = new[] { "Arcana", "History" },
                Features = new[] { ("Researcher", "When you attempt to learn or recall a piece of lore, if you do not know that information, you often know where and from whom you can obtain it. Usually, this information comes from a library, scriptorium, university, or a sage or other learned person or creature. Your DM might rule that the knowledge you seek is secreted away in an almost inaccessible place, or that it simply cannot be found.") }
            },
            new
            {
                Name = "Sailor",
                Description = "You sailed on a seagoing vessel for years. In that time, you faced down mighty storms, monsters of the deep, and those who wanted to sink your craft to the bottomless depths. Your first love is the distant line of the horizon, but the time has come to try your hand at something new.",
                LanguagesGranted = 0,
                ToolProficiency = "Navigator's tools, vehicles (water)",
                StartingEquipment = "A belaying pin (club), 50 feet of silk rope, a lucky charm such as a rabbit foot or a small stone with a hole in the center, a set of common clothes, and a pouch containing 10 gp",
                Skills = new[] { "Athletics", "Perception" },
                Features = new[] { ("Ship's Passage", "When you need to, you can secure free passage on a sailing ship for yourself and your adventuring companions. You might sail on the ship you served on, or another ship you have good relations with. Because you're calling in a favor, you can't be certain of a schedule or route that will meet your every need. Your Dungeon Master will determine how long it takes to get where you need to go.") }
            },
            new
            {
                Name = "Soldier",
                Description = "War has been your life for as long as you care to remember. You trained as a youth, studied the use of weapons and armor, learned basic survival techniques, including how to stay alive on the battlefield. You might have been part of a standing national army or a mercenary company, or perhaps a member of a local militia who rose to prominence during a recent war.",
                LanguagesGranted = 0,
                ToolProficiency = "One type of gaming set, vehicles (land)",
                StartingEquipment = "An insignia of rank, a trophy taken from a fallen enemy (a dagger, broken blade, or piece of a banner), a set of bone dice or deck of cards, a set of common clothes, and a pouch containing 10 gp",
                Skills = new[] { "Athletics", "Intimidation" },
                Features = new[] { ("Military Rank", "You have a military rank from your career as a soldier. Soldiers loyal to your former military organization still recognize your authority and influence, and they defer to you if they are of a lower rank. You can invoke your rank to exert influence over other soldiers and requisition simple equipment or horses for temporary use.") }
            },
            new
            {
                Name = "Urchin",
                Description = "You grew up on the streets alone, orphaned, and poor. You had no one to watch over you or to provide for you, so you learned to provide for yourself. You fought fiercely over food and kept a constant watch out for other desperate souls who might steal from you. You slept on rooftops and in alleyways, exposed to the elements, and endured sickness without the advantage of medicine or a place to recuperate.",
                LanguagesGranted = 0,
                ToolProficiency = "Disguise kit, thieves' tools",
                StartingEquipment = "A small knife, a map of the city you grew up in, a pet mouse, a token to remember your parents by, a set of common clothes, and a pouch containing 10 gp",
                Skills = new[] { "Sleight of Hand", "Stealth" },
                Features = new[] { ("City Secrets", "You know the secret patterns and flow to cities and can find passages through the urban sprawl that others would miss. When you are not in combat, you (and companions you lead) can travel between any two locations in the city twice as fast as your speed would normally allow.") }
            }
        };

        foreach (var bd in srdBackgrounds)
        {
            var background = new Background
            {
                Name = bd.Name,
                Description = bd.Description,
                LanguagesGranted = bd.LanguagesGranted,
                ToolProficiencyDescription = bd.ToolProficiency,
                StartingEquipmentDescription = bd.StartingEquipment,
                IsCustom = false,
                IsPublic = true,
                SourceBook = "SRD"
            };
            _db.Backgrounds.Add(background);
            await _db.SaveChangesAsync();

            foreach (var skill in bd.Skills)
                _db.BackgroundSkillProficiencies.Add(new BackgroundSkillProficiency { BackgroundId = background.Id, SkillName = skill });

            foreach (var (name, desc) in bd.Features)
                _db.BackgroundFeatures.Add(new BackgroundFeature { BackgroundId = background.Id, Name = name, Description = desc });

            await _db.SaveChangesAsync();
        }
    }
}
