using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class Leech() : CorruptionHandlerCard(15, 2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move),
        new RepeatVar(2)
    ];

    private bool isFree = false;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play, DynamicVars.Repeat.IntValue).Execute(choiceContext);

        bool metThreshold = play.Target.GetPowerAmount<AbyssalCorruptionPower>() >= 15;
        if (!metThreshold)
        {
            await CardPileCmd.Add(this, PileType.Exhaust);
        }
    }

    protected override async Task CorruptionConsumptionEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        this.EnergyCost.SetThisTurn(0);
        await CardPileCmd.Add(this, PileType.Hand);
        await PowerCmd.Apply<AbyssalCorruptionPower>(
            choiceContext, play.Target,
            5, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
    
    
}