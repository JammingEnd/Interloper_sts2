using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Powers;

// at the start of each turn, deal amount power x amount of cards in your exhaust pile with the consumed keyword to all enemies
public class CompletelyLostPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        var exhaustPile = PileType.Exhaust.GetPile(player);
        int count = exhaustPile.Cards.Count(c =>
            c.Keywords.Contains(InterloperKeywords.Consumed));
        if (count <= 0)
            return;

        var enemies = player.Creature.CombatState?.HittableEnemies;
        if (enemies == null)
            return;

        await CreatureCmd.Damage(choiceContext, enemies, this.Amount * count, ValueProp.Unpowered, Owner, null, null);
    }
}