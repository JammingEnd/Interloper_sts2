using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Potential;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;

namespace Interloper.InterloperCode.Powers;

public class AddedBenefitPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card.Owner != Owner.Player)
            return;

        if (oldPileType == PileType.Exhaust && card.EnergyCost.Canonical >= 2)
        {
            var ctx = new GameActionPlayerChoiceContext(new ConsoleCmdGameAction(Owner.Player, "h", true));
            await DarkPotentialCmd.Add(ctx, Owner.Player, Amount);
        }
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(InterloperKeywords.DarkPotential)
    ];
}