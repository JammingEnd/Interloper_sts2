using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class Shhhh() : InterloperCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(13, ValueProp.Move),
        new IntVar("Potential", 9),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DarkPotentialCmd.Add(choiceContext, Owner, DynamicVars["Potential"].IntValue);
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Potential"].UpgradeValueBy(8m);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(InterloperKeywords.DarkPotential),
        ];

}