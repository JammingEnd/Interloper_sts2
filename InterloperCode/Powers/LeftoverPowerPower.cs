using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Powers;

public class LeftoverPowerPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        var voidAmount = player.Creature.GetPowerAmount<VoidReachPower>();
        if (voidAmount > 1)
        {
            await CreatureCmd.Damage(choiceContext, this.CombatState!.HittableEnemies, this.Amount, ValueProp.Unpowered,
                Owner);
        }
    }
}