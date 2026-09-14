using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Cards.Rare;

public class Goodwill() : InterloperCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("Potential", 22),
        new PowerVar<StrengthPower>("DistortStr", 2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if ((Owner.PlayerCombatState?.GetDarkPotential() ?? 0) <= 0)
            return;
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["DistortStr"].IntValue, Owner.Creature, this);
        await DarkPotentialCmd.Add(choiceContext, Owner, DynamicVars["Potential"].IntValue);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(InterloperKeywords.DarkPotential)
    ];
}