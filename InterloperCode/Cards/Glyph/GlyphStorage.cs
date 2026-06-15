using BaseLib.Utils;
using Interloper.InterloperCode.Entries;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Glyph;

public class GlyphStorage() : InterloperPower
{
    public static event Func<Creature, PlayerChoiceContext, Task>? OnGlyphsConsumed;
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.GetType() != typeof(GlyphStorage))
            return;

        if (amount <= 0)
            return;

        var glyphsOnPlayer = Owner.Powers.OfType<GlyphPower>().ToArray();
        if (glyphsOnPlayer.Length < 3)
            return;

        int eyes = glyphsOnPlayer.Count(g => g is GlyphEyePower);
        int mouths = glyphsOnPlayer.Count(g => g is GlyphMouthPower);
        int tails = glyphsOnPlayer.Count(g => g is GlyphTailPower);

        var effectTask = (eyes, mouths, tails) switch
        {
            (3, 0, 0) => ThreeEyes(choiceContext),
            (0, 3, 0) => ThreeMouths(choiceContext),
            (0, 0, 3) => ThreeTails(choiceContext),
            (2, 1, 0) => TwoEyesOneMouth(choiceContext),
            (2, 0, 1) => TwoEyesOneTail(choiceContext),
            (1, 2, 0) => OneEyeTwoMouths(choiceContext),
            (0, 2, 1) => TwoMouthsOneTail(choiceContext),
            (1, 0, 2) => OneEyeTwoTails(choiceContext),
            (0, 1, 2) => OneMouthTwoTails(choiceContext),
            (1, 1, 1) => OneOfEach(choiceContext),
            _ => Task.CompletedTask
        };
        await effectTask;
        await PlayerCmd.GainEnergy(2, Owner.Player);
        foreach (var inst in glyphsOnPlayer)
        {
            await PowerCmd.Remove(inst);
        }
        if (OnGlyphsConsumed != null)
        {
            foreach (var handler in OnGlyphsConsumed.GetInvocationList().Cast<Func<Creature, PlayerChoiceContext, Task>>())
                await handler(Owner, choiceContext);
        }
        await PowerCmd.Remove<GlyphStorage>(Owner);
    }
    

    // draw 3 cards
    private async Task ThreeEyes(PlayerChoiceContext choiceContext)
    {
        await CardPileCmd.Draw(choiceContext, 3, Owner.Player);
    }

    private async Task ThreeMouths(PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.Heal(Owner, 5, true);
    }

    private async Task ThreeTails(PlayerChoiceContext choiceContext)
    {
        // % based damage
    }
    
    // pick 1 cards and put them in your hand
    private async Task TwoEyesOneMouth(PlayerChoiceContext choiceContext)
    {
        var pile = PileType.Draw.GetPile(Owner.Player);
        if (pile.Cards.Count == 0) pile = PileType.Discard.GetPile(Owner.Player);
        
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, Owner.Player, prefs);
        
        if (selected != null)
        {
            foreach (var card in selected)
            {
                card.EnergyCost.AddUntilPlayed(-1);
                await CardPileCmd.Add(card, PileType.Hand);
            }
        }
    }

    private async Task TwoEyesOneTail(PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.GainBlock(Owner, 15, ValueProp.Unpowered, null);
    }

    private async Task OneEyeTwoMouths(PlayerChoiceContext choiceContext)
    {
        var pile = PileType.Draw.GetPile(Owner.Player); 
        var statusses = pile.Cards.Where(c => c.Type == CardType.Status);
        if (statusses.Count() != 0)
        {
            foreach (var status in statusses)
            {
                await CardCmd.Exhaust(choiceContext, status);
            }
            var all = PileType.Discard.GetPile(Owner.Player).Cards.Concat(PileType.Exhaust.GetPile(Owner.Player).Cards).Concat(PileType.Hand.GetPile(Owner.Player).Cards);
            var unUpgraded = all.Where(c => c.IsUpgraded == false).ToArray();
            for (int i = 0; i < statusses.Count(); i++)
            {
                var selected = unUpgraded[Owner.Player.RunState.Rng.CombatCardGeneration.NextInt(unUpgraded.Length)];
                CardCmd.Upgrade(selected);
            }
        }
        int energyGain = statusses.Count() == 0 ? 1 : statusses.Count();
        await PlayerCmd.GainEnergy(energyGain, Owner.Player);
        
    }

    private async Task TwoMouthsOneTail(PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, 3, Owner, null);
    }

    private async Task OneEyeTwoTails(PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<OneTwoPunchPower>(choiceContext, Owner, 1, Owner, null);
    }

    private async Task OneMouthTwoTails(PlayerChoiceContext choiceContext)
    {
        await PlayerCmd.GainEnergy(2, Owner.Player);
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, 2, Owner, null);
    }

    private async Task OneOfEach(PlayerChoiceContext choiceContext)
    {
        await CardPileCmd.Shuffle(choiceContext, Owner.Player);
    }
}