using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Powers;

public class WitnessMePower() : InterloperPower, IAfterDarkPotentialCleared
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public async Task AfterDarkPotentialCleared(PlayerChoiceContext choiceContext, Player player, int level)
    {
        if (player != Owner.Player)
            return;

        int voidreach = Owner.GetPowerAmount<VoidReachPower>();
        var calc = voidreach == 0 ? 1 : voidreach;

        await CreatureCmd.Damage(choiceContext,
                CombatState!.HittableEnemies, this.Amount * calc,
                ValueProp.Unpowered, Owner, null, null);
    }
}
