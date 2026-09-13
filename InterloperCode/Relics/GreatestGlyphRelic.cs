using Interloper.InterloperCode.Potential;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Relics;

// when a total of 50+ abyssal corruption is present in combat, clearing Dark Potential consumes half the corruption on the targets
public class GreatestGlyphRelic : InterloperRelic, IAfterDarkPotentialCleared
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    private const int CorruptionThreshold = 50;

    public async Task AfterDarkPotentialCleared(PlayerChoiceContext choiceContext, Player player, int level)
    {
        if (player != Owner)
            return;

        if (GetTotalCorruption() < CorruptionThreshold)
            return;

        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        foreach (var enemy in combatState.HittableEnemies)
        {
            int amount = enemy.GetPowerAmount<AbyssalCorruptionPower>();
            if (amount <= 0)
                continue;

            await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, enemy, -(int)(amount * 0.5m), Owner.Creature, null);
        }
    }

    private int GetTotalCorruption()
    {
        var combatState = Owner.Creature.CombatState;
        return combatState?.HittableEnemies.Sum(e => (int)e.GetPowerAmount<AbyssalCorruptionPower>()) ?? 0;
    }
}