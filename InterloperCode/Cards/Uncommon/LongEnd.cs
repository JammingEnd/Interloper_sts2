using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Cards.Glyph;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class LongEnd() : CorruptionHandlerCard(15,1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9, ValueProp.Move),
        new PowerVar<AbyssalCorruptionPower>("AbyssalCorruptionPower", 5)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ConsumptionOverride = 5;
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, play.Target, DynamicVars["AbyssalCorruptionPower"].IntValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["AbyssalCorruptionPower"].UpgradeValueBy(3);
    }

    protected override async Task CorruptionConsumptionEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var tailCard = CombatState.CreateCard<GlyphTail>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(tailCard, PileType.Hand, Owner);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromCard<GlyphTail>(),
            HoverTipFactory.FromPower<AbyssalCorruptionPower>()
        ];

}