using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Interloper.InterloperCode.Entries;

public class SequenceActivatedEntry(    
    int amount,
    Player player,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history,
    IEnumerable<Player> players) : CombatHistoryEntry(player.Creature, roundNumber, currentSide, history, players)
{
    public int Amount { get; } = amount;

    public override string Description
    {
        get
        {
            return
                $"{this.Actor.Player.Character.Id.Entry} activated a sequence worth {this.Amount}";
        }
    }
}