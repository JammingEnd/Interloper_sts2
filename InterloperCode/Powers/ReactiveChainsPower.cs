using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Powers;

public class ReactiveChainsPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;

        if (cardPlay.Card is GlyphCard)
        {
            await PowerCmd.Apply<ReactiveChainsStrengthPower>(choiceContext, Owner, this.Amount, Owner, null);
        }
    }
}