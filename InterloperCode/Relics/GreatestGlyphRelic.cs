using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Relics;

// when at least a level 4 potential has been cleared, your potential goes down to level 1
public class GreatestGlyphRelic : InterloperRelic, IAfterDarkPotentialCleared
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    public async Task AfterDarkPotentialCleared(PlayerChoiceContext choiceContext, Player player, int level)
    {
        if (player != Owner)
            return;

        if (level < 4)
            return;

        await DarkPotentialCmd.SetCurrentToLevel(choiceContext, player, 1);
    }
}