using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Interloper.InterloperCode.Cards.Common;

// pick a card from your discard pile, its free to play but gains exhaust
public class LostAndFound() : InterloperCard(2,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        
        var pile = PileType.Discard.GetPile(Owner);
        if (pile.Cards.Count == 0) return;
        
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, Owner, prefs);
        
        if (selected != null)
        {
            foreach (var card in selected)
            {
                card.EnergyCost.SetThisTurnOrUntilPlayed(0);
                card.AddKeyword(CardKeyword.Exhaust);
                await CardPileCmd.Add(card, PileType.Hand);
            }
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}