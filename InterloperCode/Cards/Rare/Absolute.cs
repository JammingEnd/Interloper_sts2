using BaseLib.Utils;
using Interloper.InterloperCode.Entries;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Rare;

public class Absolute() : InterloperCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3, ValueProp.Move),
        new CalculationBaseVar(3M),
        new CalculationExtraVar(1M),
        new CalculatedVar("TotalDamage").WithMultiplier(
            (card, _) => (card.IsUpgraded ? 5 : 0)
                + CombatManager.Instance.History.Entries
                    .OfType<SequenceActivatedEntry>()
                    .Sum(e => e.Amount))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int totalDamage = (int)((CalculatedVar)DynamicVars["TotalDamage"]).Calculate(play.Target);
        DynamicVars.Damage.BaseValue = totalDamage;
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
    }
}