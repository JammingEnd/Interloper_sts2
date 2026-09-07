using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Powers;

public class LastWordsPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && dealer != Owner && dealer != null)
        {
            int amount = (int)result.UnblockedDamage;
            if (amount > 0)
            {
                PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, dealer, amount, Owner, null);
            }
        }
        return Task.CompletedTask;
    }
}