using BaseLib.Utils;
using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Rare;

public class Absolute() : InterloperCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy), IAfterDarkPotentialCleared
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3, ValueProp.Move),
        new IntVar("Increase", 2M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }

    public Task AfterDarkPotentialCleared(PlayerChoiceContext choiceContext, Player player, int level, int energy)
    {
        if (player != Owner)
            return Task.CompletedTask;

        SyncDamage();
        return Task.CompletedTask;
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card != this)
            return;

        SyncDamage();
    }

    private void SyncDamage()
    {
        int clears = Owner.PlayerCombatState?.GetDarkPotentialState()?.Clears ?? 0;
        DynamicVars.Damage.BaseValue = (IsUpgraded == false ? 3 : 4) + clears * DynamicVars["Increase"].IntValue;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Increase"].UpgradeValueBy(1m);
    }
}