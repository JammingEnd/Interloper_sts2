using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Cards.Glyph;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class Sideye() : InterloperCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar("SideyeIni", 7, ValueProp.Move),
        new BlockVar("SideyeMoved", 15, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, DynamicVars["SideyeIni"], play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SideyeIni"].UpgradeValueBy(3m);
        DynamicVars["SideyeMoved"].UpgradeValueBy(5m);
    }

    protected override async void AfterMovedFromExhaust(CardModel card)
    {
        var eyeCard = CombatState.CreateCard<GlyphEye>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(eyeCard, PileType.Hand, Owner);
        await CreatureCmd.GainBlock(card.Owner.Creature, CanonicalVars.First(v => v.Name == "SideyeMoved") as BlockVar, null);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromCard<GlyphEye>(false)
        ];

}