using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Powers;

public class TunnelVisionPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var enemies = CombatState.HittableEnemies.ToList();
        if (enemies.Count == 1)
        {
            await PowerCmd.Apply<AbyssalCorruptionPower>(
                choiceContext, enemies[1],
                Amount,
                Owner, null
            );
        } else if (enemies.Count < 1)
        {
            return;
        }

        var highestHpEnemy = enemies.MaxBy(e => e.CurrentHp);
        if (highestHpEnemy == null)
            return;

        int otherCount = enemies.Count - 1;
        int totalLoss = otherCount * (int)this.Amount;

        foreach (var enemy in enemies)
        {
            if (enemy == highestHpEnemy) continue;
            await PowerCmd.Apply<AbyssalCorruptionPower>(
                choiceContext, enemy,
                -this.Amount,
                Owner, null
            );
        }

        await PowerCmd.Apply<AbyssalCorruptionPower>(
            choiceContext, highestHpEnemy,
            totalLoss,
            Owner, null
        );
    }
}