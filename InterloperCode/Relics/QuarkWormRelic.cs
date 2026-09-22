using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Relics;

// every time you consume 20 abyssal corruption, a random power gets -1 to play
public class QuarkWormRelic : InterloperRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    private const int ConsumeThreshold = 20;

    private int _consumedGamma;
    private int _lastTriggered;
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is not GammaPower)
            return;

        if (applier != Owner.Creature)
            return;

        if (amount >= 0m)
            return;

        _consumedGamma += (int)Math.Abs(amount);
        int triggers = _consumedGamma / ConsumeThreshold - _lastTriggered;
        if (triggers <= 0)
            return;

        for (int i = 0; i < triggers; i++)
        {
            var powerCards = Owner.PlayerCombatState.AllCards.Where(c => c.Type == CardType.Power).ToArray();
            if (powerCards.Length == 0)
                continue;

            var target = Owner.RunState.Rng.CombatCardGeneration.NextItem(powerCards);
            target.EnergyCost.AddThisCombat(-1);
        }

        _lastTriggered += triggers;
    }
}