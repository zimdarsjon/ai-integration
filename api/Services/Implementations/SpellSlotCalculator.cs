// Multiclass spell slot table per 5e PHB rules.
// Indexed as [combinedCasterLevel - 1][slotLevel - 1]
public static class SpellSlotCalculator
{
    private static readonly int[,] MulticlassTable = {
        { 2, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 3, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 4, 2, 0, 0, 0, 0, 0, 0, 0 },
        { 4, 3, 0, 0, 0, 0, 0, 0, 0 },
        { 4, 3, 2, 0, 0, 0, 0, 0, 0 },
        { 4, 3, 3, 0, 0, 0, 0, 0, 0 },
        { 4, 3, 3, 1, 0, 0, 0, 0, 0 },
        { 4, 3, 3, 2, 0, 0, 0, 0, 0 },
        { 4, 3, 3, 3, 1, 0, 0, 0, 0 },
        { 4, 3, 3, 3, 2, 0, 0, 0, 0 },
        { 4, 3, 3, 3, 2, 1, 0, 0, 0 },
        { 4, 3, 3, 3, 2, 1, 0, 0, 0 },
        { 4, 3, 3, 3, 2, 1, 1, 0, 0 },
        { 4, 3, 3, 3, 2, 1, 1, 0, 0 },
        { 4, 3, 3, 3, 2, 1, 1, 1, 0 },
        { 4, 3, 3, 3, 2, 1, 1, 1, 0 },
        { 4, 3, 3, 3, 2, 1, 1, 1, 1 },
        { 4, 3, 3, 3, 3, 1, 1, 1, 1 },
        { 4, 3, 3, 3, 3, 2, 1, 1, 1 },
        { 4, 3, 3, 3, 3, 2, 2, 1, 1 }
    };

    // Warlock pact magic tracked separately; this handles standard casters.
    public static int[] CalculateSlots(IEnumerable<(CasterType casterType, int level)> classCasters)
    {
        int combinedLevel = 0;
        bool hasWarlockOnly = true;

        foreach (var (type, level) in classCasters)
        {
            hasWarlockOnly = hasWarlockOnly && type == CasterType.Warlock;
            combinedLevel += type switch
            {
                CasterType.Full => level,
                CasterType.Half => level / 2,
                CasterType.Third => level / 3,
                _ => 0
            };
        }

        if (combinedLevel == 0) return new int[9];

        combinedLevel = Math.Clamp(combinedLevel, 1, 20);
        var row = combinedLevel - 1;
        var slots = new int[9];
        for (int i = 0; i < 9; i++)
            slots[i] = MulticlassTable[row, i];

        return slots;
    }

    public static int ProficiencyBonusForLevel(int totalLevel) => totalLevel switch
    {
        <= 4 => 2,
        <= 8 => 3,
        <= 12 => 4,
        <= 16 => 5,
        _ => 6
    };

    public static int AbilityModifier(int score) => (score - 10) / 2;
}
