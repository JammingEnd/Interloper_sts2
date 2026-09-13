using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Powers;

public class TouchOfFaithDexPower : InterloperPower
{
    protected override object InitInternalData() => (object) new Data();
    public override int DisplayAmount => 6 - this.GetInternalData<Data>().corruptionSpend % 6;
    private class Data
    {
        public int corruptionSpend;
        public int triggerCount;
    }
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override PowerInstanceType InstanceType =>
        PowerInstanceType.Instanced;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner != Owner)
            return;
        if(power is not VoidReachPower)
            return;
        if (amount >= 0)
            return;
        var data = GetInternalData<Data>();
        data.corruptionSpend += (int)amount;
        int triggers = data.corruptionSpend / 6 - data.triggerCount;
        if (triggers > 0)
        {
            Flash();
            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, triggers, Owner, null);
            data.triggerCount += triggers;
        }
        InvokeDisplayAmountChanged();
    }
}