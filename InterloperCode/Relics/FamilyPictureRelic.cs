using Interloper.InterloperCode.Character;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;

namespace Interloper.InterloperCode.Relics;

// After playing a card from a different class each turn, gain 1 void reach
public class FamilyPictureRelic : InterloperRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;

    private bool _usedThisTurn;
    private bool UsedThisTurn
    {
        get => this._usedThisTurn;
        set
        {
            this.AssertMutable();
            this._usedThisTurn = value;
        }
    }
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return;
        this.UsedThisTurn = false;
        this.Status = RelicStatus.Active;
    }
    public override async Task AfterCombatEnd(CombatRoom _)
    {
        this.UsedThisTurn = false;
        this.Status = RelicStatus.Normal;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            this.Status = RelicStatus.Active;
            this.UsedThisTurn = false;
        }
        
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;

        if (!_usedThisTurn && cardPlay.Card._pool is not InterloperCardPool)
        {
            await PowerCmd.Apply<VoidReachPower>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
        }
    }
}