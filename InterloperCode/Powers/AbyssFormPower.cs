using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Powers;

public class AbyssFormPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var hand = PileType.Hand.GetPile(Owner.Player);

        foreach (var card in hand.Cards
            .Where(c => c.Keywords.Contains(CardKeyword.Retain) || c.Keywords.Contains(CardKeyword.Exhaust))
            .Take((int)Amount))
        {
            card.EnergyCost.AddThisTurnOrUntilPlayed(-1, true);
        }
    }
}