using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Powers;

public class HugeRiftPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var enemies = CombatState.HittableEnemies.ToList();
        if (enemies.Count < 1)
            return;

        var lowestHpEnemy = enemies.MinBy(e => e.CurrentHp);
        if (lowestHpEnemy == null)
            return;

        foreach (var enemy in enemies)
        {
            int amount = (int)Amount;
            if (enemy == lowestHpEnemy)
                amount *= 2;

            await PowerCmd.Apply<AbyssalCorruptionPower>(
                choiceContext, enemy,
                amount,
                Owner, null
            );
        }
    }
}