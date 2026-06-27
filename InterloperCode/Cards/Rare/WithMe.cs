using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Helpers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Rare;

public class WithMe() : InterloperCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitCount(3)
            .Execute(choiceContext);
    }

    protected override async void AfterMovedFromExhaust(CardModel card)
    {
        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        var sets = exhaustPile.GetOldestPlayableCards();
        if (sets.Length > 0)
        {
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1);
            PlayerChoiceContext ctx =
                new GameActionPlayerChoiceContext(new ConsoleCmdGameAction(card.Owner, "h", true));
            var selected = await CardSelectCmd.FromSimpleGrid(
                ctx, sets, Owner, prefs);
            var cards = selected.ToArray();
            if (cards.Length > 0)
            {
                await CardPileCmd.Add(selected.ToArray()[0], PileType.Hand);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}