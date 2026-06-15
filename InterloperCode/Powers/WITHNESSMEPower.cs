using Interloper.InterloperCode.Cards.Glyph;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Powers;

public class WITHNESSMEPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    private bool _subscribed;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext, PowerModel power,
        decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.GetType() != typeof(WITHNESSMEPower))
            return;

        if (amount <= 0)
            return;

        if (!_subscribed)
        {
            GlyphStorage.OnGlyphsConsumed += OnGlyphsConsumed;
            _subscribed = true;
        }
    }

    private async Task OnGlyphsConsumed(Creature owner, PlayerChoiceContext choiceContext)
    {
        if (owner != this.Owner)
            return;

        int voidreach = Owner.GetPowerAmount<VoidReachPower>();
        var calc = voidreach == 0 ? 1 : voidreach;

        MainFile.Logger.Info("triggering WITHNESS ME");
        await CreatureCmd.Damage(choiceContext,
                CombatState!.HittableEnemies, this.Amount * calc,
                ValueProp.Unpowered, Owner, null);
    }
}