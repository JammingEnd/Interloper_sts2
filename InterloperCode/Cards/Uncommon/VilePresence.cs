using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Entries;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class VilePresence() : InterloperCard(0,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(1, ValueProp.Move),
        new CalculationBaseVar(0M), new CalculationExtraVar(1M),
        new CalculatedVar("CalculatedDamage").WithMultiplier(
                (Func<CardModel, Creature, Decimal>) ((card, _) => 
                    (Decimal) CombatManager.Instance.History.Entries.OfType<CorruptionModifiedEntry>().Where<CorruptionModifiedEntry>((Func<CorruptionModifiedEntry, bool>) (e => e.HappenedThisTurn(card.CombatState) && e.Amount > 0 && e.Actor == card.Owner.Creature)).Sum<CorruptionModifiedEntry>((Func<CorruptionModifiedEntry, int>) (e => e.Amount)
                        )
                    )
                )
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        VilePresence card = this;
        AttackCommand attackCommand = await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).WithHitCount((int) ((CalculatedVar) card.DynamicVars["CalculatedDamage"]).Calculate(play.Target)).FromCard((CardModel) card).TargetingAllOpponents(card.CombatState).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}