using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Common;

// deal 12 damage, if the enemy has 15+ corruption, deal damage to all enemies
public class VeilSweep() : InterloperCard(2,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<AbyssalCorruptionPower>("AbyssalCorruptionPower", 12),
        new CardsVar(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, play.Target, DynamicVars["AbyssalCorruptionPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["AbyssalCorruptionPower"].UpgradeValueBy(3m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if(card != this)
            return;
        if(oldPileType != PileType.Exhaust)
            return;
        if(card.Owner != this.Owner)
            return;
        var ctx = new HookPlayerChoiceContext(
            this,
            LocalContext.NetId.Value,
            this.CombatState,
            GameActionType.CombatPlayPhaseOnly);
        await ctx.AssignTaskAndWaitForPauseOrCompletion(CardPileCmd.Draw(ctx, DynamicVars.Cards.IntValue, Owner));
    }
}