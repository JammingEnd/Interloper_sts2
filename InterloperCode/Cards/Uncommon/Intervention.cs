using Interloper.InterloperCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class Intervention() : InterloperCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        if (exhaustPile.Cards.Count == 0)
            return;

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1);
        var selected = await CardSelectCmd.FromSimpleGrid(
            choiceContext, exhaustPile.Cards, Owner, prefs);

        if (selected != null)
        {
            foreach (var card in selected)
            {
                exhaustPile.MoveToTopInternal(card);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}