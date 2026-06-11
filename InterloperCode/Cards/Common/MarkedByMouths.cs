using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Interloper.InterloperCode.Cards.Common;

// mark a card with consumed and exhaust a card. upgrades to retain
public class MarkedByMouths() : InterloperCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    private static readonly LocString SelectPrompt = new("cards", "INTERLOPER-MARKED_BY_MOUTHS.selectionScreenPrompt");

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        // mark a card
        var prefs = new CardSelectorPrefs(SelectPrompt, 1);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this);
        var target = selected.FirstOrDefault();
        CardCmd.ApplyKeyword(target, ConsumedKeyword.Consumed);
        
        // exhaust a card
        var exhaustPrefs = new CardSelectorPrefs(SelectPrompt, 1);
        var exhaustSelected = await CardSelectCmd.FromHand(choiceContext, Owner, exhaustPrefs, null, this);
        var exhaustTarget = exhaustSelected.FirstOrDefault();
        CardCmd.Exhaust(choiceContext, exhaustTarget);
    }

    protected override void OnUpgrade()
    {
        CardCmd.ApplyKeyword(this, CardKeyword.Retain);
    }
}