using Interloper.InterloperCode.Powers;
using Interloper.InterloperCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Interloper.InterloperCode.Relics;
public class LoseChainsRelic() : InterloperRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

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

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if(card.Owner != this.Owner)
            return;
        if (this.UsedThisTurn)
            return;
        await PowerCmd.Apply<VoidReachPower>(choiceContext, Owner.Creature, 2m, Owner.Creature, null);
        UsedThisTurn = true;
        this.Status = RelicStatus.Disabled;
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VoidReachPower>()
    ];
}