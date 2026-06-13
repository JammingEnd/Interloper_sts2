using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Cards;

// cardtype that for cards that have corruption effects
public abstract class CorruptionHandlerCard(int corruptionThreshold, int cost, CardType type, CardRarity rarity, TargetType target) : InterloperCard(cost, type, rarity, target)
{
    protected int ConsumptionOverride = 0;
    protected abstract Task CorruptionConsumptionEffect(PlayerChoiceContext choiceContext, CardPlay play);

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card != this)
        {
            return;
        }
        
        if (cardPlay.Target.GetPowerAmount<AbyssalCorruptionPower>() >= corruptionThreshold)
        {
            await CorruptionConsumptionEffect(choiceContext, cardPlay);
            int consumed = -corruptionThreshold;
            if (this.ConsumptionOverride != 0)
            {
                consumed = this.ConsumptionOverride;
                // reset after usage
                ConsumptionOverride = 0;
            }
            await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, cardPlay.Target, consumed, Owner.Creature, this);
        }
    }
    
}
