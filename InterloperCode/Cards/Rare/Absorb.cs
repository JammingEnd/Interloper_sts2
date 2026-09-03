using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Cards.Rare;

public class Absorb() : InterloperCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int voidReach = 0;

        if (Owner.Creature.GetPowerAmount<VulnerablePower>() >= 1)
        {
            await PowerCmd.Apply<VulnerablePower>(
                choiceContext, Owner.Creature, -1, Owner.Creature, this);
            voidReach++;
        }

        if (Owner.Creature.GetPowerAmount<WeakPower>() >= 1)
        {
            await PowerCmd.Apply<WeakPower>(
                choiceContext, Owner.Creature, -1, Owner.Creature, this);
            voidReach++;
        }

        if (Owner.Creature.GetPowerAmount<FrailPower>() >= 1)
        {
            await PowerCmd.Apply<FrailPower>(
                choiceContext, Owner.Creature, -1, Owner.Creature, this);
            voidReach++;
        }

        if (voidReach > 0)
            await PowerCmd.Apply<VoidReachPower>(
                choiceContext, Owner.Creature, voidReach, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VoidReachPower>()
    ];
}