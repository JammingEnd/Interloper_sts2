using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Powers;

public class SyphonPower() : InterloperPower
{
    protected override object InitInternalData() => new SyphonPower.Data();

    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override PowerInstanceType InstanceType =>
        PowerInstanceType.Instanced;

    public override int DisplayAmount => 15 - this.GetInternalData<SyphonPower.Data>().corruptionApplied % 15;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        Log.Debug("START SYHPON");
        var data = GetInternalData<SyphonPower.Data>();
        data.corruptionApplied = 0;
        data.triggeredThisTurn = false;
        InvokeDisplayAmountChanged();
        Log.Debug("END SYHPON");
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.GetType() != typeof(AbyssalCorruptionPower))
            return;

        if (cardSource?.Owner.Creature != Owner)
            return;

        if (amount <= 0)
            return;

        var data = GetInternalData<SyphonPower.Data>();
        data.corruptionApplied += (int)amount;

        if (!data.triggeredThisTurn && data.corruptionApplied >= 15)
        {
            Flash();
            await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner, 1, Owner, null);
            data.triggeredThisTurn = true;
        }
        InvokeDisplayAmountChanged();
    }

    private class Data
    {
        public int corruptionApplied;
        public bool triggeredThisTurn;
    }
}