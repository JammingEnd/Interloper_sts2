using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Powers;

public class PainfulRenewalPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if(card.Owner.Creature != this.Owner)
            return;
        var cardSource = this;
        Creature target = cardSource.Owner.Player.RunState.Rng.CombatTargets.NextItem( cardSource.CombatState.HittableEnemies);
        if (target == null)
            return;
        var ctx = new GameActionPlayerChoiceContext(new ConsoleCmdGameAction(Owner.Player, "h", true));
        await CreatureCmd.Damage(ctx, target, this.Amount, ValueProp.Unpowered, this.Owner);
        
    }
}