using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class ToTheDepths() : InterloperCard(0,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move),
        new IntVar("Potential", 9),
    ];
    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var card = this;
        int energyX = card.ResolveEnergyXValue();
        if (energyX <= 0) return;

        await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
            .WithHitCount(energyX)
            .FromCard(card, play).Targeting(play.Target)
            .Execute(choiceContext);

        if (energyX < 2)
            return;

        int potential = energyX >= 6 ? 15 : energyX >= 4 ? 10 : DynamicVars["Potential"].IntValue;
        await DarkPotentialCmd.Add(choiceContext, Owner, potential);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(InterloperKeywords.DarkPotential)
        ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Potential"].UpgradeValueBy(4m);
    }
}