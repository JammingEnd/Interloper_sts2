using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Potions;

public sealed class WeirdSludgePotion : InterloperPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.None;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var combatState = target?.CombatState ?? Owner.Creature.CombatState;
        if (combatState == null)
            return;

        await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, combatState.HittableEnemies, 15, Owner.Creature,
            null);

    }
}