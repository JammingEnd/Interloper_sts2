using Interloper.InterloperCode.Glyphs;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Interloper.InterloperCode.Helpers;

public static class LocHelper
{
    public static string GetAllPossibleOutcomes(Creature creature)
    {
        var player = creature.Player;
        if (player == null)
            return "";

        var queue = player.PlayerCombatState?.GetGlyphQueue();
        var (eyes, mouths, tails) = queue?.GetCounts() ?? (0, 0, 0);

        var lines = new List<string>();
        foreach (var (e, m, t, label) in AllSequences)
        {
            if (e >= eyes && m >= mouths && t >= tails)
                lines.Add($"{label}: {ComboDesc(e, m, t)}");
        }

        return string.Join("\n", lines);
    }

    private static readonly (int eyes, int mouths, int tails, string label)[] AllSequences =
    [
        (3, 0, 0, "3 Eye"),
        (0, 3, 0, "3 Mouth"),
        (0, 0, 3, "3 Tail"),
        (2, 1, 0, "2 Eye, 1 Mouth"),
        (2, 0, 1, "2 Eye, 1 Tail"),
        (1, 2, 0, "1 Eye, 2 Mouths"),
        (0, 2, 1, "2 Mouths, 1 Tail"),
        (1, 0, 2, "1 Eye, 2 Tails"),
        (0, 1, 2, "1 Mouth, 2 Tails"),
        (1, 1, 1, "1 of each")
    ];

    private static string ComboDesc(int eyes, int mouths, int tails) => (eyes, mouths, tails) switch
    {
        (3, 0, 0) => "Draw 3 cards",
        (0, 3, 0) => "Apply 2 Vulnerable to all enemies",
        (0, 0, 3) => "Deal 8 damage per distinct power you have, split among enemies",
        (2, 1, 0) => "Add a card to your hand",
        (2, 0, 1) => "Gain 15 Block",
        (1, 2, 0) => "Gain 5 Vigor",
        (0, 2, 1) => "Gain 2 Strength",
        (1, 0, 2) => "Apply One-Two Punch",
        (0, 1, 2) => "Gain 2 Energy and 2 Dexterity",
        (1, 1, 1) => "Shuffle your draw pile",
        _ => ""
    };
}