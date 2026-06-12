using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Common;

// deal 7 damage, if its going to apply a debuff, apply 5 corruption
public class NoShot() : InterloperCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move),
         new PowerVar<AbyssalCorruptionPower>("AbyssalCorruption", 5)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
         await CommonActions.CardAttack(this, play).Execute(choiceContext);
         if (play.Target.Monster.NextMove.Intents.OfType<AbstractIntent>().Any(e => e.IntentType is IntentType.Debuff or IntentType.DebuffStrong))
         {
             await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, play.Target, DynamicVars["AbyssalCorruption"].IntValue, Owner.Creature, this);
         }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<AbyssalCorruptionPower>()
        ];

    protected override void OnUpgrade()
    {
            DynamicVars.Damage.UpgradeValueBy(3m);
            DynamicVars["AbyssalCorruption"].UpgradeValueBy(2m);
    }
}