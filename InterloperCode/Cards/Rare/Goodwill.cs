using Interloper.InterloperCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Interloper.InterloperCode.Cards.Rare;

public class Goodwill() : InterloperCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var hand = PileType.Hand.GetPile(Owner);
        if (!hand.Cards.Any(c => c is GlyphCard))
            return;

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1);
        var selected = await CardSelectCmd.FromHand(
            choiceContext, Owner, prefs, c => c is GlyphCard, this);

        var target = selected.FirstOrDefault();
        if (target == null)
            return;

        var newCard = CardFactory.GetForCombat(Owner,
            ModelDb.CardPool<ColorlessCardPool>()
                .GetUnlockedCards(Owner.UnlockState, CardMultiplayerConstraint.SingleplayerOnly), 1,
            Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();

        if (newCard != null)
            await CardCmd.Transform(target, newCard);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}