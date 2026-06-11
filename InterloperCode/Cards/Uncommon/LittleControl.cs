using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Cards.Glyph;
using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class LittleControl() : InterloperCard(2,
    CardType.Skill, CardRarity.Uncommon,
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
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, PileType.Draw.GetPile(Owner).Cards, Owner, prefs);
        if (selected != null)
        {
            foreach (var card in selected)
            {
                card.AddKeyword(CardKeyword.Exhaust);
                card.AddKeyword(ConsumedKeyword.Consumed);
                card.EnergyCost.SetThisCombat(0);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}