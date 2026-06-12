using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Common;

// all enemies take 6 damage and gain 3 corruption
public class ShadowAbove() : InterloperCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6, ValueProp.Move),
            new PowerVar<AbyssalCorruptionPower>("AbyssalCorruptionPower",3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CommonActions.CardAttack(this, play).Execute(choiceContext);
        foreach (var enemy in CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, enemy, DynamicVars["AbyssalCorruptionPower"].IntValue, Owner.Creature, this);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<AbyssalCorruptionPower>()
        ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["AbyssalCorruptionPower"].UpgradeValueBy(3m);
    }
}