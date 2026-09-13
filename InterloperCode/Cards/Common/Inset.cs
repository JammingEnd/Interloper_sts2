using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Common;
// deal 3 damage, draw 1 
public class Inset() : InterloperCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [InterloperKeywords.Pure];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VoidReachPower>("VoidReachPower", 2m),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<VoidReachPower>(choiceContext, Owner.Creature, DynamicVars["VoidReachPower"].IntValue, Owner.Creature, this);

        await CommonActions.Draw(this, choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VoidReachPower"].UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}