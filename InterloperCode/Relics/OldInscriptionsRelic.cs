using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Relics;

// for every energy gained when clearing Dark Potential, gain 3 temporary strength
public class OldInscriptionsRelic : InterloperRelic, IAfterDarkPotentialCleared
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    private const int StrengthPerEnergy = 3;

    public async Task AfterDarkPotentialCleared(PlayerChoiceContext choiceContext, Player player, int level, int energy)
    {
        if (player != Owner)
            return;

        if (energy <= 0)
            return;

        await PowerCmd.Apply<TemporaryStrengthPower>(
            choiceContext, Owner.Creature, energy * StrengthPerEnergy, Owner.Creature, null);
    }
}