using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Powers;

public class LinkedTradePower() : InterloperPower
{
    protected override object InitInternalData() => (object) new LinkedTradePower.Data();
    public override int DisplayAmount => 10 - this.GetInternalData<LinkedTradePower.Data>().corruptionSpend % 10;
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override PowerInstanceType InstanceType =>
        PowerInstanceType.Instanced;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.GetType() != typeof(AbyssalCorruptionPower))
            return;

        if (applier != Owner)
            return;

        if (amount <= 0)
            return;

        var data = GetInternalData<Data>();
        data.corruptionSpend += (int)amount;
        int triggers = data.corruptionSpend / 10 - data.triggerCount;
        if (triggers > 0)
        {
            Flash();
            await PowerCmd.Apply<VoidReachPower>(choiceContext, Owner, triggers, Owner, null);
            data.triggerCount += triggers;
        }
        InvokeDisplayAmountChanged();
    }
    
    private class Data
    {
        public int corruptionSpend;
        public int triggerCount;
    }
}