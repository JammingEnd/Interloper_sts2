using BaseLib.Extensions;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Glyphs;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Relics;

// when a total of 50+ abyssal corruption is present in combat, the next glyph you play activates the sequence of its full type, then consume half corruption on the targets
public class GreatestGlyphRelic : InterloperRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    private const int CorruptionThreshold = 50;

    private bool _overridePlay;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        _overridePlay = false;

        if (cardPlay.Card.Owner != Owner)
            return;
        if (cardPlay.Card is not GlyphCard)
            return;
        if (GetTotalCorruption() < CorruptionThreshold)
            return;

        _overridePlay = true;
        Owner.PlayerCombatState?.GetGlyphQueue()?.Clear();
    }

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!_overridePlay)
            return;

        _overridePlay = false;

        var queue = Owner.PlayerCombatState?.GetGlyphQueue();
        if (queue == null)
            return;

        var glyphType = ((GlyphCard)cardPlay.Card).GlyphType;
        queue.Clear();
        for (int i = 0; i < 3; i++)
        {
            var model = CreateGlyphModel(glyphType);
            model.Owner = Owner;
            queue.TryEnqueue(model);
        }

        await GlyphCmd.Activate(choiceContext, Owner);

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

    private GlyphModel CreateGlyphModel(GlyphType type) => type switch
    {
        GlyphType.EYE => ModelDb.Get<GlyphEyeModel>().ToMutable(),
        GlyphType.MOUTH => ModelDb.Get<GlyphMouthModel>().ToMutable(),
        GlyphType.TAIL => ModelDb.Get<GlyphTailModel>().ToMutable(),
        _ => throw new InvalidOperationException($"Unknown glyph type {type}")
    };

    private int GetTotalCorruption()
    {
        var combatState = Owner.Creature.CombatState;
        return combatState?.HittableEnemies.Sum(e => (int)e.GetPowerAmount<AbyssalCorruptionPower>()) ?? 0;
    }
}