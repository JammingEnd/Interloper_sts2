using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Cards.Glyph;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class Shhhh() : InterloperCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10, ValueProp.Move),
        new PowerVar<MonarchsGazePower>("ShhhPower", 1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var mouthCard = CombatState.CreateCard<GlyphMouth>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(mouthCard, PileType.Hand, Owner);
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        await PowerCmd.Apply<ShhhStrengthPower>(choiceContext, play.Target, DynamicVars["ShhhPower"].IntValue,
             Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}