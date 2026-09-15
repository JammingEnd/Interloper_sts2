using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class EmpowerCursePower : InterloperPower
{
    private const int MaxHandCost = 6;
    private const int StrengthAmount = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var hand = PileType.Hand.GetPile(Owner.Player);
        int totalCost = hand.Cards.Sum(c => c.EnergyCost.Canonical);
        if (totalCost > MaxHandCost)
            return;

        await PowerCmd.Apply<TemporaryStrengthPower>(choiceContext, Owner, StrengthAmount, Owner, null);
    }
}