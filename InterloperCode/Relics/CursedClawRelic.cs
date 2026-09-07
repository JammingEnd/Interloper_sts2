using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Relics;

// after killing an enemy with a card that consumed abyssal corruption, all other enemies gain 1 vulnerable 
public class CursedClawRelic : InterloperRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    private bool _isCorruptionCardPlay;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card is not CorruptionHandlerCard handler)
        {
            _isCorruptionCardPlay = false;
            return;
        }

        if (cardPlay.Card.Owner != Owner)
        {
            _isCorruptionCardPlay = false;
            return;
        }

        var target = cardPlay.Target;
        bool willConsume = target != null && target.GetPowerAmount<AbyssalCorruptionPower>() >= handler.CorruptionThreshold;

        _isCorruptionCardPlay = willConsume;
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (!_isCorruptionCardPlay)
            return;

        _isCorruptionCardPlay = false;

        var enemies = creature.CombatState?.HittableEnemies.Where(e => e != creature).ToArray() ?? [];
        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, 1, Owner.Creature, null);
        }
    }
}