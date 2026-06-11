using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Common;

// deal 12 damage, if the enemy has 15+ corruption, deal damage to all enemies
public class VeilSweep() : CorruptionHandlerCard(15, 2,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(14, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
            DynamicVars.Damage.UpgradeValueBy(4m);
    }

    protected override async Task CorruptionConsumptionEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        foreach (var enemy in CombatState!.HittableEnemies)
        {
            if(enemy == play.Target)
                continue;
            
           await CreatureCmd.Damage(choiceContext, enemy, DynamicVars.Damage.IntValue, ValueProp.Move, this.Owner.Creature, this);
        }
    }
}