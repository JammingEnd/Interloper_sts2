using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class EmpowerCursePower : InterloperPower
{
    private bool _blockBroke;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterBlockBroken(PlayerChoiceContext choiceContext, Creature target, Creature? breaker)
    {
        if (target != Owner || _blockBroke)
            return;

        _blockBroke = true;

        var enemies = target.CombatState?.HittableEnemies.ToArray() ?? [];
        if (enemies.Length == 0)
            return;

        var weakTarget = target.Player.RunState.Rng.CombatTargets.NextItem(enemies);
        await PowerCmd.Apply<WeakPower>(choiceContext, weakTarget, 1, target, null);
    }

    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
            return;

        if (!_blockBroke)
            await PowerCmd.Apply<TemporaryStrengthPower>(choiceContext, Owner, 1, Owner, null);

        await PowerCmd.Remove(this);
    }
}