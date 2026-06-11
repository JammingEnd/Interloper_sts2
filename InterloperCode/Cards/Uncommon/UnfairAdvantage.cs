using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class UnfairAdvantage() : CorruptionHandlerCard(5, 0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<WeakPower>("WeakPower", 1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<WeakPower>(
            choiceContext, play.Target,
            DynamicVars["WeakPower"].IntValue,
            Owner.Creature, this);
    }

    protected override async Task CorruptionConsumptionEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<StrengthPower>(
            choiceContext, play.Target,
            -1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["WeakPower"].UpgradeValueBy(1m);
    }
}