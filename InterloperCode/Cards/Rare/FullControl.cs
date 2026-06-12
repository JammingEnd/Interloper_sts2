using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Helpers;
using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Cards.Rare;

public class FullControl() : InterloperCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        var validCards = exhaustPile.GetOldestPlayableCards(includeConsumed: true);
        if (validCards.Length == 0)
            return;

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        var selected = await CardSelectCmd.FromSimpleGrid(
            choiceContext, validCards, Owner, prefs);

        if (selected == null)
            return;

        foreach (var card in selected)
        {
            card.RemoveKeyword(InterloperKeywords.Consumed);
            exhaustPile.MoveToBottomInternal(card);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}