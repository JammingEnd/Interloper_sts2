using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class ArmorOfSlumber() : InterloperCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<AbyssalCorruptionPower>("AbyssalCorruptionPower", 5m),
        new BlockVar("SlumberBlock", 0, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        DynamicVars["SlumberBlock"].BaseValue = 0;
        await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, play.Target,
            DynamicVars["AbyssalCorruptionPower"].IntValue, Owner.Creature, this);
        var targetCorruption = play.Target.GetPowerAmount<AbyssalCorruptionPower>();
        await CreatureCmd.GainBlock(Owner.Creature, targetCorruption, ValueProp.Move, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["AbyssalCorruptionPower"].UpgradeValueBy(3m);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<AbyssalCorruptionPower>()
    ];
}