using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class Sideye() : InterloperCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<WeakPower>("SideyeWeak", 1),
        new BlockVar("SideyeMoved", 15, ValueProp.Move),
        new IntVar("Potential", 7)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, DynamicVars["SideyeWeak"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SideyeWeak"].UpgradeValueBy(1m);
        DynamicVars["SideyeMoved"].UpgradeValueBy(3m);
    }

    protected override async void AfterMovedFromExhaust(CardModel card)
    {
        var ctx = new HookPlayerChoiceContext(
            this,
            LocalContext.NetId.Value,
            this.CombatState,
            GameActionType.CombatPlayPhaseOnly);
        await ctx.AssignTaskAndWaitForPauseOrCompletion(DarkPotentialCmd.Add(ctx, card.Owner, DynamicVars["Potential"].IntValue));
        await CreatureCmd.GainBlock(card.Owner.Creature, DynamicVars["SideyeMoved"] as BlockVar, null);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(InterloperKeywords.DarkPotential)
        ];

}