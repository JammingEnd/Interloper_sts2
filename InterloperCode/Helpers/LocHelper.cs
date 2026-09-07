using Interloper.InterloperCode.Glyphs;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Interloper.InterloperCode.Helpers;

public static class LocHelper
{
    public static string GetPossibleOutcomeLoc(Creature creature)
    {
        string baseLoc = "";
        var player = creature.Player;
        if (player == null)
            return baseLoc;

        var queue = player.PlayerCombatState?.GetGlyphQueue();
        if (queue == null || queue.Glyphs.Count != 2)
            return baseLoc;

        var (eyes, mouths, tails) = queue.GetCounts();

        string[] outcomes =
        [
            $"[Eye]: {ComboDesc(eyes + 1, mouths, tails)}",
            $"[Mouth]: {ComboDesc(eyes, mouths + 1, tails)}",
            $"[Tail]: {ComboDesc(eyes, mouths, tails + 1)}"
        ];
        baseLoc = string.Join("\n", outcomes);

        return baseLoc;
    }

    private static string ComboDesc(int eyes, int mouths, int tails) => (eyes, mouths, tails) switch
    {
        (3, 0, 0) => "Draw 3 cards",
        (0, 3, 0) => "Heal 5",
        (0, 0, 3) => "Deal percentage damage",
        (2, 1, 0) => "Add a card to your hand",
        (2, 0, 1) => "Gain 15 Block",
        (1, 2, 0) => "Exhaust status cards and upgrade cards",
        (0, 2, 1) => "Gain 3 Strength",
        (1, 0, 2) => "Apply One-Two Punch",
        (0, 1, 2) => "Gain 2 Energy and 2 Dexterity",
        (1, 1, 1) => "Shuffle your draw pile",
        _ => ""
    };
}