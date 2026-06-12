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
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class PrayersHeard() : CorruptionHandlerCard(10, 1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<PrayersHeardPower>("PrayersHeardPower",5)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<PrayersHeardPower>(choiceContext, play.Target, DynamicVars["PrayersHeardPower"].IntValue, Owner.Creature, this);

        var eyeCard = CombatState.CreateCard<GlyphEye>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(eyeCard, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PrayersHeardPower"].UpgradeValueBy(2m);
    }

    protected override async Task CorruptionConsumptionEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var mouthCard = CombatState.CreateCard<GlyphMouth>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(mouthCard, PileType.Hand, Owner);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromCard<GlyphEye>(false),
            HoverTipFactory.FromCard<GlyphMouth>(false)
        ];

}