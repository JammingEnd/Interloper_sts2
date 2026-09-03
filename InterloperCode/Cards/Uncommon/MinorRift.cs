using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class MinorRift() : InterloperCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new RepeatVar(4),
        new DamageVar(5, ValueProp.Move),
        new PowerVar<AbyssalCorruptionPower>("AbyssalCorruption", 2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        MinorRift card = this;
        for (int i = 0; i < card.DynamicVars.Repeat.IntValue; i++)
        {
            Creature enemy = card.Owner.RunState.Rng.CombatTargets.NextItem<Creature>((IEnumerable<Creature>) card.CombatState.HittableEnemies);
            if (enemy  != null)
            {
                await CreatureCmd.Damage(choiceContext, enemy, this.DynamicVars.Damage, this);
                await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, enemy,
                    card.DynamicVars["AbyssalCorruption"].IntValue, Owner.Creature, this);
            }
            enemy = (Creature)null;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1M);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<AbyssalCorruptionPower>()
    ];
}