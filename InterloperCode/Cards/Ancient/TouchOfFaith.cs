using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Ancient;

//everytime 6 void reach gained, gain strength, every 6 void reach consumed, gain dexterity
public class TouchOfFaith() : InterloperCard(2, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VoidReachPower>("TouchOfFaithStrPower",4),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TouchOfFaithStrPower>(choiceContext, cardPlay.Target, 1m, Owner.Creature, this);
        await PowerCmd.Apply<TouchOfFaithDexPower>(choiceContext, cardPlay.Target, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}