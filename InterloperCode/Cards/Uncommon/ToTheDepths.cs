using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class ToTheDepths() : InterloperCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, ConsumedKeyword.Consumed];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Move)
    ];
    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var card = this;
        int energyX = card.ResolveEnergyXValue();
        int cardsInHand = PileType.Hand.GetPile(Owner).Cards.Count - 1;
        int difference = cardsInHand - energyX;
        if (energyX > 0)
        {
            var exhaustPrefs = new CardSelectorPrefs(SelectionScreenPrompt, cardsInHand);
            var exhaustSelected = await CardSelectCmd.FromHand(choiceContext, Owner, exhaustPrefs, null, this);
            var exhaustTarget = exhaustSelected.FirstOrDefault();
            CardCmd.Exhaust(choiceContext, exhaustTarget);
            
            // give back leftover
            await PlayerCmd.GainEnergy(difference, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}