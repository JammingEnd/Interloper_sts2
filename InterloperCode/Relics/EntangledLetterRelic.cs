using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Interloper.InterloperCode.Relics;

// the first card you exhaust each combat is upgraded
public class EntangledLetterRelic() : InterloperRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;
    private bool _usedThisCombat;
    private bool UsedThisCombat
    {
        get => this._usedThisCombat;
        set
        {
            this.AssertMutable();
            this._usedThisCombat = value;
        }
    }
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return Task.CompletedTask;
        this.UsedThisCombat = false;
        this.Status = RelicStatus.Active;
        return Task.CompletedTask;
    }
    public override Task AfterCombatEnd(CombatRoom _)
    {
        this.UsedThisCombat = false;
        this.Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if(card.Owner != this.Owner)
            return;
        if(this.UsedThisCombat)
            return; 
        CardCmd.Upgrade(card);
        this.UsedThisCombat = true;
        this.Status = RelicStatus.Disabled;
    }
}