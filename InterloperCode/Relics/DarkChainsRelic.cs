using Interloper.InterloperCode.Helpers;
using Interloper.InterloperCode.Powers;
using Interloper.InterloperCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace Interloper.InterloperCode.Relics;

public class DarkChainsRelic() : InterloperRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override RelicModel? GetUpgradeReplacement()
        => ModelDb.Relic<LoseChainsRelic>();
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
        await PowerCmd.Apply<VoidReachPower>(choiceContext, Owner.Creature, 2m, Owner.Creature, null);
        this.UsedThisCombat = true;
        this.Status = RelicStatus.Disabled;
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VoidReachPower>()
    ];
}