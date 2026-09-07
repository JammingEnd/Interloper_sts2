using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Glyphs;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Relics;

// gain an extra effect when activating a glyph sequence, based on the first glyph played. (eye=apply 1 weak to a random enemy without weak, mouth heal 1, tail=apply 3 corruption to all enemies)
public class OldInscriptionsRelic : InterloperRelic, IAfterSequenceActivated
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    public async Task AfterSequenceActivated(PlayerChoiceContext choiceContext, Player player, IReadOnlyList<GlyphModel> glyphs)
    {
        if (player != Owner)
            return;

        if (glyphs.Count == 0)
            return;

        var combatState = player.Creature.CombatState;
        if (combatState == null)
            return;

        switch (glyphs[0].Type)
        {
            case GlyphType.EYE:
                var weakTargets = combatState.HittableEnemies
                    .Where(e => e.GetPowerAmount<WeakPower>() == 0)
                    .ToArray();
                if (weakTargets.Length == 0)
                    return;

                var weakTarget = player.RunState.Rng.CombatTargets.NextItem(weakTargets);
                await PowerCmd.Apply<WeakPower>(choiceContext, weakTarget, 1, Owner.Creature, null);
                break;

            case GlyphType.MOUTH:
                await CreatureCmd.Heal(Owner.Creature, 1, true);
                break;

            case GlyphType.TAIL:
                foreach (var enemy in combatState.HittableEnemies)
                {
                    await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, enemy, 3, Owner.Creature, null);
                }
                break;
        }
    }
}