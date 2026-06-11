using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Cards.Glyph;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Interloper.InterloperCode.Cards.Common;
// Gain 1 eye glyph

public class DeepGaze() : GlyphCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var eyeCard = CombatState.CreateCard<GlyphEye>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(eyeCard, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}