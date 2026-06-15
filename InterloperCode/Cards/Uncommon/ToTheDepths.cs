using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Cards.Glyph;
using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class ToTheDepths() : InterloperCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6, ValueProp.Move),
    ];
    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var card = this;
        int energyX = card.ResolveEnergyXValue();
        await CommonActions.CardAttack(this, play, energyX).Execute(choiceContext);
        if(energyX < 3)
            return;
        if (energyX < 6)
        {
            var eyeCard = CombatState.CreateCard<GlyphEye>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(eyeCard, PileType.Hand, Owner);
        }
        if (energyX < 9)
        {
            var eyeCard = CombatState.CreateCard<GlyphMouth>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(eyeCard, PileType.Hand, Owner);
        }
        if (energyX >= 9)
        {
            var eyeCard = CombatState.CreateCard<GlyphTail>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(eyeCard, PileType.Hand, Owner);
        }
            
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromCard<GlyphEye>(),
            HoverTipFactory.FromCard<GlyphMouth>(),
            HoverTipFactory.FromCard<GlyphTail>()
        ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}