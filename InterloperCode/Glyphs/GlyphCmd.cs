using BaseLib.Extensions;
using Interloper.InterloperCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Glyphs;

public static class GlyphCmd
{
    /// <summary>
    /// Enqueue a Glyph of type <c>T</c> into the player's Glyph queue. Automatically activates the sequence when the queue is full.
    /// </summary>
    public static async Task Produce<T>(PlayerChoiceContext choiceContext, Player player,
        CardModel? card = null, CardPlay? cardPlay = null) where T : GlyphModel
    {
        var model = ModelDb.Get<T>().ToMutable();
        await Produce(choiceContext, model, player, card, cardPlay);
    }

    /// <summary>
    /// Enqueue a Glyph into the player's Glyph queue. Automatically activates the sequence when the queue is full.
    /// </summary>
    public static async Task Produce(PlayerChoiceContext choiceContext, GlyphModel glyph, Player player,
        CardModel? card = null, CardPlay? cardPlay = null)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return;

        if (player.Creature.CombatState == null)
            return;

        var queue = player.PlayerCombatState?.GetGlyphQueue();
        if (queue == null)
            return;

        glyph.AssertMutable();
        glyph.Owner = player;

        if (!queue.TryEnqueue(glyph))
            return;

        if (queue.IsFull)
            await Activate(choiceContext, player, card, cardPlay);
    }
    /// <summary>
    /// Consume the queue and activate the glyph sequence based on the glyph counts.
    /// </summary>
    public static async Task Activate(PlayerChoiceContext choiceContext, Player player,
        CardModel? card = null, CardPlay? cardPlay = null)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return;

        if (player.Creature.CombatState == null)
            return;

        var combatState = player.Creature.CombatState;
        var queue = player.PlayerCombatState?.GetGlyphQueue();
        if (queue == null || !queue.HasAny)
            return;

        var glyphs = queue.Glyphs.ToArray();
        var (eyes, mouths, tails) = queue.GetCounts();
        queue.Clear();

        await ((eyes, mouths, tails) switch
        {
            (3, 0, 0) => ThreeEyes(choiceContext, player),
            (0, 3, 0) => ThreeMouths(choiceContext, player),
            (0, 0, 3) => ThreeTails(choiceContext, player),
            (2, 1, 0) => TwoEyesOneMouth(choiceContext, player),
            (2, 0, 1) => TwoEyesOneTail(choiceContext, player),
            (1, 2, 0) => OneEyeTwoMouths(choiceContext, player),
            (0, 2, 1) => TwoMouthsOneTail(choiceContext, player),
            (1, 0, 2) => OneEyeTwoTails(choiceContext, player),
            (0, 1, 2) => OneMouthTwoTails(choiceContext, player),
            (1, 1, 1) => OneOfEach(choiceContext, player),
            _ => Task.CompletedTask
        });

        await PlayerCmd.GainEnergy(2, player);

        await GlyphHook.AfterSequenceActivated(combatState, choiceContext, player, glyphs);
    }

    private static async Task ThreeEyes(PlayerChoiceContext choiceContext, Player player)
    {
        await CardPileCmd.Draw(choiceContext, 3, player);
    }

    private static async Task ThreeMouths(PlayerChoiceContext choiceContext, Player player)
    {
        await CreatureCmd.Heal(player.Creature, 5, true);
    }

    private static async Task ThreeTails(PlayerChoiceContext choiceContext, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
            return;

        Creature highestHp = combatState.HittableEnemies.MaxBy(c => c.CurrentHp);
        if (highestHp != null)
        {
            var hpPercent = (decimal)highestHp.MaxHp * 0.25m;
            DamageVar newDamage = new DamageVar(hpPercent, ValueProp.Unblockable);
            await CreatureCmd.Damage(choiceContext, highestHp, newDamage, player.Creature);
        }
    }

    private static async Task TwoEyesOneMouth(PlayerChoiceContext choiceContext, Player player)
    {
        var pile = PileType.Draw.GetPile(player);
        if (pile.Cards.Count == 0)
            pile = PileType.Discard.GetPile(player);

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, player, prefs);

        if (selected != null)
        {
            foreach (var card in selected)
            {
                card.EnergyCost.AddUntilPlayed(-1);
                await CardPileCmd.Add(card, PileType.Hand);
            }
        }
    }

    private static async Task TwoEyesOneTail(PlayerChoiceContext choiceContext, Player player)
    {
        await CreatureCmd.GainBlock(player.Creature, 15, ValueProp.Unpowered, null);
    }

    private static async Task OneEyeTwoMouths(PlayerChoiceContext choiceContext, Player player)
    {
        var pile = PileType.Draw.GetPile(player);
        var statusses = pile.Cards.Where(c => c.Type == CardType.Status).ToArray();
        if (statusses.Length != 0)
        {
            foreach (var status in statusses)
            {
                await CardCmd.Exhaust(choiceContext, status);
            }

            IEnumerable<CardModel> all = [
                .. PileType.Discard.GetPile(player).Cards,
                .. PileType.Draw.GetPile(player).Cards,
                .. PileType.Hand.GetPile(player).Cards];

            var unUpgraded = all.Where(c => c.IsUpgraded == false && c.GetType() != typeof(GlyphCard)).ToArray();
            for (int i = 0; i < statusses.Length; i++)
            {
                var selected = unUpgraded[player.RunState.Rng.CombatCardGeneration.NextInt(unUpgraded.Length)];
                CardCmd.Upgrade(selected);
            }
        }
    }

    private static async Task TwoMouthsOneTail(PlayerChoiceContext choiceContext, Player player)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, player.Creature, 3, player.Creature, null);
    }

    private static async Task OneEyeTwoTails(PlayerChoiceContext choiceContext, Player player)
    {
        await PowerCmd.Apply<OneTwoPunchPower>(choiceContext, player.Creature, 1, player.Creature, null);
    }

    private static async Task OneMouthTwoTails(PlayerChoiceContext choiceContext, Player player)
    {
        await PlayerCmd.GainEnergy(2, player);
        await PowerCmd.Apply<DexterityPower>(choiceContext, player.Creature, 2, player.Creature, null);
    }

    private static async Task OneOfEach(PlayerChoiceContext choiceContext, Player player)
    {
        await CardPileCmd.Shuffle(choiceContext, player);
    }
}