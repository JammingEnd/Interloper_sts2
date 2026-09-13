using Interloper.InterloperCode.Potential;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Relics;

// gain an extra effect when clearing Dark Potential, based on the level reached.
// level 1: apply 1 Weak to a random enemy without Weak. level 2: heal 1.
// level 3+: apply Abyssal Corruption to all enemies, scaling with the level.
public class OldInscriptionsRelic : InterloperRelic, IAfterDarkPotentialCleared
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    public async Task AfterDarkPotentialCleared(PlayerChoiceContext choiceContext, Player player, int level)
    {
        if (player != Owner)
            return;

        var combatState = player.Creature.CombatState;
        if (combatState == null)
            return;

        if (level >= 1)
        {
            var weakTargets = combatState.HittableEnemies
                .Where(e => e.GetPowerAmount<WeakPower>() == 0)
                .ToArray();
            if (weakTargets.Length > 0)
            {
                var weakTarget = player.RunState.Rng.CombatTargets.NextItem(weakTargets);
                await PowerCmd.Apply<WeakPower>(choiceContext, weakTarget, 1, Owner.Creature, null);
            }
        }

        if (level >= 2)
        {
            await CreatureCmd.Heal(Owner.Creature, 1, true);
        }

        if (level >= 3)
        {
            int corruption = 3 + (level - 3) * 3;
            foreach (var enemy in combatState.HittableEnemies)
            {
                await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, enemy, corruption, Owner.Creature, null);
            }
        }
    }
}
