using Interloper.InterloperCode.Potential;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Powers;

public class ReactiveChainsPower() : InterloperPower, IOnDarkPotentialGained
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public async Task OnDarkPotentialGained(PlayerChoiceContext choiceContext, Player player, int amount)
    {
        if (player != Owner.Player)
            return;

        await PowerCmd.Apply<ReactiveChainsStrengthPower>(choiceContext, Owner, this.Amount, Owner, null);
    }
}
