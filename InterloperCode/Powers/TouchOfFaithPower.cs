using Interloper.InterloperCode.Cards.Ancient;
using Interloper.InterloperCode.Potential;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Interloper.InterloperCode.Powers;

public class TouchOfFaithPower : InterloperPower
{
    protected override object InitInternalData() => (object) new Data();
    public override int DisplayAmount => this.GetInternalData<Data>().cardsLeft;
    private class Data
    {
        public int cardsLeft = 10;
    }
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override PowerInstanceType InstanceType =>
        PowerInstanceType.Instanced;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is TouchOfFaith || cardPlay.Card.Owner.Creature != this.Owner)
            return;
        var data = GetInternalData<Data>();
        data.cardsLeft -= (int)1;
        if (data.cardsLeft <= 0)
        {
            Flash();
            await DarkPotentialCmd.Add(choiceContext, cardPlay.Player, 5);
            data.cardsLeft = 10;
        }
        InvokeDisplayAmountChanged();
    }
}