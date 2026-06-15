using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Rare;

public class FeedingOnNothingness() : InterloperCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    private const int _baseDamage = 7;
    private int _currentDamage = 7;
    private int _multiplier;
    [SavedProperty]
    public int CurrentDamage
    {
        get => this._currentDamage;
        set
        {
            this.AssertMutable();
            this._currentDamage = value;
            this.DynamicVars.Damage.BaseValue = (Decimal) this._currentDamage;
        }
    }

    [SavedProperty]
    public int Multiplier
    {
        get => this._multiplier;
        set
        {
            this.AssertMutable();
            this._multiplier = value;
        }
    }
    private void BuffFromPlay()
    {
        this.Multiplier += this.Multiplier;
        this.UpdateDamage();
    }
    private void UpdateDamage() => this.CurrentDamage = 7 * this.Multiplier;


    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar((Decimal) this.CurrentDamage, ValueProp.Move),
        new IntVar("Increase", 2M)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        this.BuffFromPlay();
    }
    protected override void AfterDowngraded() => this.UpdateDamage();
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}