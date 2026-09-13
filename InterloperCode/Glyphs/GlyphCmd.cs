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
    public static event Action<Player>? OnSequenceActivated;

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
        await ActivateFromQueue(choiceContext, player, clearQueue: true, card, cardPlay);
    }

    /// <summary>
    /// Activate the glyph sequence from the queue's counts. When <paramref name="clearQueue"/> is false the queue is left intact.
    /// </summary>
    public static async Task Activate(PlayerChoiceContext choiceContext, Player player, bool clearQueue,
        CardModel? card = null, CardPlay? cardPlay = null)
    {
        await ActivateFromQueue(choiceContext, player, clearQueue, card, cardPlay);
    }

    /// <summary>
    /// Activate a specific glyph sequence directly, independent of the queue.
    /// </summary>
    public static async Task Activate(PlayerChoiceContext choiceContext, Player player, GlyphSequence sequence,
        CardModel? card = null, CardPlay? cardPlay = null)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return;

        if (player.Creature.CombatState == null)
            return;

        var combatState = player.Creature.CombatState;
        var glyphs = CreateGlyphsForSequence(player, sequence);

        await ActivateSequence(combatState, choiceContext, player, glyphs, sequence);
    }

    private static async Task ActivateFromQueue(PlayerChoiceContext choiceContext, Player player, bool clearQueue,
        CardModel? card, CardPlay? cardPlay)
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
        if (clearQueue)
            queue.Clear();

        await ActivateSequence(combatState, choiceContext, player, glyphs, ToSequence(eyes, mouths, tails));
    }

    private static GlyphSequence ToSequence(int eyes, int mouths, int tails) => (eyes, mouths, tails) switch
    {
        (3, 0, 0) => GlyphSequence.ThreeEyes,
        (0, 3, 0) => GlyphSequence.ThreeMouths,
        (0, 0, 3) => GlyphSequence.ThreeTails,
        (2, 1, 0) => GlyphSequence.TwoEyesOneMouth,
        (2, 0, 1) => GlyphSequence.TwoEyesOneTail,
        (1, 2, 0) => GlyphSequence.OneEyeTwoMouths,
        (0, 2, 1) => GlyphSequence.TwoMouthsOneTail,
        (1, 0, 2) => GlyphSequence.OneEyeTwoTails,
        (0, 1, 2) => GlyphSequence.OneMouthTwoTails,
        (1, 1, 1) => GlyphSequence.OneOfEach,
        _ => GlyphSequence.OneOfEach
    };

    private static GlyphModel[] CreateGlyphsForSequence(Player player, GlyphSequence sequence)
    {
        var model = sequence switch
        {
            GlyphSequence.ThreeEyes or GlyphSequence.TwoEyesOneMouth or GlyphSequence.TwoEyesOneTail or GlyphSequence.OneOfEach
                => ModelDb.Get<GlyphEyeModel>().ToMutable(),
            GlyphSequence.ThreeMouths or GlyphSequence.OneEyeTwoMouths or GlyphSequence.TwoMouthsOneTail
                => ModelDb.Get<GlyphMouthModel>().ToMutable(),
            _ => ModelDb.Get<GlyphTailModel>().ToMutable()
        };
        model.Owner = player;
        return [model];
    }

    private static async Task ActivateSequence(ICombatState combatState, PlayerChoiceContext choiceContext, Player player,
        GlyphModel[] glyphs, GlyphSequence sequence)
    {
        await (sequence switch
        {
            GlyphSequence.ThreeEyes => ThreeEyes(choiceContext, player),
            GlyphSequence.ThreeMouths => ThreeMouths(choiceContext, player),
            GlyphSequence.ThreeTails => ThreeTails(choiceContext, player),
            GlyphSequence.TwoEyesOneMouth => TwoEyesOneMouth(choiceContext, player),
            GlyphSequence.TwoEyesOneTail => TwoEyesOneTail(choiceContext, player),
            GlyphSequence.OneEyeTwoMouths => OneEyeTwoMouths(choiceContext, player),
            GlyphSequence.TwoMouthsOneTail => TwoMouthsOneTail(choiceContext, player),
            GlyphSequence.OneEyeTwoTails => OneEyeTwoTails(choiceContext, player),
            GlyphSequence.OneMouthTwoTails => OneMouthTwoTails(choiceContext, player),
            GlyphSequence.OneOfEach => OneOfEach(choiceContext, player),
            _ => Task.CompletedTask
        });

        await PlayerCmd.GainEnergy(2, player);

        await GlyphHook.AfterSequenceActivated(combatState, choiceContext, player, glyphs);

        OnSequenceActivated?.Invoke(player);
    }

    private static async Task ThreeEyes(PlayerChoiceContext choiceContext, Player player)
    {
        await CardPileCmd.Draw(choiceContext, 3, player);
    }

    private static async Task ThreeMouths(PlayerChoiceContext choiceContext, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
            return;

        foreach (var enemy in combatState.HittableEnemies)
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, 2, player.Creature, null);
    }

    private static async Task ThreeTails(PlayerChoiceContext choiceContext, Player player)
    {
        // deal damage based on the amount of powers on the player. 8 * amount of different powers. then that damage is distributed over the hittable enemies
        var combatState = player.Creature.CombatState;
        if (combatState == null)
            return;

        int powerCount = player.Creature.Powers.Select(p => p.GetType()).Distinct().Count();
        decimal total = 8m * powerCount;

        var enemies = combatState.HittableEnemies.ToArray();
        if (enemies.Length == 0)
            return;

        decimal perEnemy = total / enemies.Length;
        foreach (var enemy in enemies)
        {
            await CreatureCmd.Damage(choiceContext, enemy, perEnemy, ValueProp.Unpowered, player.Creature, null, null);
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
        await PowerCmd.Apply<VigorPower>(choiceContext, player.Creature, 5, player.Creature, null);
    }

    private static async Task TwoMouthsOneTail(PlayerChoiceContext choiceContext, Player player)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, player.Creature, 2, player.Creature, null);
    }

    private static async Task OneEyeTwoTails(PlayerChoiceContext choiceContext, Player player)
    {
        await PowerCmd.Apply<OneTwoPunchPower>(choiceContext, player.Creature, 1, player.Creature, null);
    }

    private static async Task OneMouthTwoTails(PlayerChoiceContext choiceContext, Player player)
    {
        await PowerCmd.Apply<DexterityPower>(choiceContext, player.Creature, 2, player.Creature, null);
    }

    private static async Task OneOfEach(PlayerChoiceContext choiceContext, Player player)
    {
        await CardPileCmd.Shuffle(choiceContext, player);
    }
}