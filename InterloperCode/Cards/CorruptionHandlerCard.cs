using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Interloper.InterloperCode.Cards;

// cardtype that for cards that have corruption effects
public abstract class CorruptionHandlerCard(int corruptionThreshold, int cost, CardType type, CardRarity rarity, TargetType target) : InterloperCard(cost, type, rarity, target)
{
    public int CorruptionThreshold => corruptionThreshold;

    protected int ConsumptionOverride = 0;
    protected abstract Task CorruptionConsumptionEffect(PlayerChoiceContext choiceContext, CardPlay play);

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card != this)
        {
            return;
        }
        
        if (Owner.Creature.GetPowerAmount<EngulfPower>() > 0)
        {
            await CorruptionConsumptionEffect(choiceContext, cardPlay);
            await PowerCmd.Apply<EngulfPower>(choiceContext, cardPlay.Card.Owner.Creature, -1, Owner.Creature, null);
        }
        else if (cardPlay.Target.GetPowerAmount<GammaPower>() >= corruptionThreshold)
        {
            await CorruptionConsumptionEffect(choiceContext, cardPlay);
            await CreatureCmd.TriggerAnim(base.Owner.Creature, "cast_2", 0);
            int consumed = -corruptionThreshold;
            if (this.ConsumptionOverride != 0)
            {
                consumed = -this.ConsumptionOverride;
                // reset after usage
                ConsumptionOverride = 0;
            }
            else if (this.ConsumptionOverride == -1)
            {
                consumed = 0;
            }
            await PowerCmd.Apply<GammaPower>(choiceContext, cardPlay.Target, consumed * 0.5m, Owner.Creature, this);
        }
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<GammaPower>()
    ];
    
}
