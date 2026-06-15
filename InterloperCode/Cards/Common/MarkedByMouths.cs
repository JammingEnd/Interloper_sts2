using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

using MegaCrit.Sts2.Core.HoverTips;

namespace Interloper.InterloperCode.Cards.Common;

// mark a card with consumed and exhaust a card. upgrades to retain
public class MarkedByMouths() : InterloperCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        // mark a card
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this);
        if (selected != null && selected.Count() > 0)
        {
            var target = selected.FirstOrDefault();
            CardCmd.ApplyKeyword(target, InterloperKeywords.Consumed);
        }
        
        // exhaust a card
        var exhaustPrefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var exhaustSelected = await CardSelectCmd.FromHand(choiceContext, Owner, exhaustPrefs, null, this);
        if (exhaustSelected != null && selected.Count() > 0)
        {
            var exhaustTarget = exhaustSelected.FirstOrDefault();
            CardCmd.Exhaust(choiceContext, exhaustTarget);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(InterloperKeywords.Consumed)
        ];

    protected override void OnUpgrade()
    {
        CardCmd.ApplyKeyword(this, CardKeyword.Retain);
    }
}