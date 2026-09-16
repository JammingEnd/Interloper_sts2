using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Powers;

// void corruption now also increases the intensity of other debuffs
public class AbyssalCorruptionPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
            return;

        int percent = Math.Min(this.Amount, 75);
        int removed = this.Amount * percent / 100;
        if (removed <= 0)
            return;

        await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, Owner, -removed, Owner, null);
    }
}