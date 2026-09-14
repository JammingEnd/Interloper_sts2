using Interloper.InterloperCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Relics;

// when the player kills the last enemy with a card that has a consumption mechanic, heal 6 hp
public class PocketPocketDimensionRelic : InterloperRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    private bool _isConsumptionCardPlay;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        _isConsumptionCardPlay = cardPlay.Card is CorruptionHandlerCard && cardPlay.Card.Owner == Owner;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _isConsumptionCardPlay = false;
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (!_isConsumptionCardPlay)
            return;

        bool enemiesLeft = creature.CombatState?.HittableEnemies.Any(e => e != creature) ?? false;
        if (enemiesLeft)
            return;

        _isConsumptionCardPlay = false;
        await CreatureCmd.Heal(Owner.Creature, 6, true);
    }
}