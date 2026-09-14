using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Powers;

public class ColorlessPotentialPower() : InterloperPower
{
    private const int PotentialAmount = 4;

    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override PowerInstanceType InstanceType =>
        PowerInstanceType.Instanced;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;

        if (cardPlay.Card.Pool?.IsColorless != true)
            return;

        await DarkPotentialCmd.Add(choiceContext, cardPlay.Player, PotentialAmount);
    }
}