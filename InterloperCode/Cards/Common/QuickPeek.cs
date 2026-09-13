using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Common;

// Deal 6(8) damage and 1 (2) apply weak, when A.C.  10+, gain 1 dex next turn
public class QuickPeek() : CorruptionHandlerCard(10, 1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [InterloperKeywords.Pure];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move),
        new PowerVar<WeakPower>("QuickPeekWeak", 1),
        new PowerVar<DexterityPower>("QuickPeekDex", 1), 
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["QuickPeekWeak"].UpgradeValueBy(1m);
    }

    protected override async Task CorruptionConsumptionEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<DexNextTurnPower>(choiceContext, Owner.Creature, DynamicVars["QuickPeekDex"].IntValue, Owner.Creature, this);
    }
}