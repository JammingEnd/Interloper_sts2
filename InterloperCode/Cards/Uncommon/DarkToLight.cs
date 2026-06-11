using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class DarkToLight() : CorruptionHandlerCard(10, 1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VoidReachPower>("VoidReachPower", 4)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        base.AfterCardPlayed(choiceContext, cardPlay);
        await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, cardPlay.Target, 5, Owner.Creature, this);
    }

    protected override async Task CorruptionConsumptionEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<VoidReachPower>(
            choiceContext, Owner.Creature,
            DynamicVars["VoidReachPower"].IntValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VoidReachPower"].UpgradeValueBy(2m);
    }
}