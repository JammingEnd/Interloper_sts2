using BaseLib.Utils;
using Interloper.InterloperCode.Glyphs;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Rare;

public class Absolute() : InterloperCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy), IAfterSequenceActivated
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    private int _currentDamage = 3;
    private int _increasedDamage;
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
    public int IncreasedDamage
    {
        get => this._increasedDamage;
        set
        {
            this.AssertMutable();
            this._increasedDamage = value;
        }
    }
    private void BuffFromPlay(int addAmount)
    {
        this.IncreasedDamage += addAmount;
        this.UpdateDamage();
    }
    private void UpdateDamage() => this.CurrentDamage = 3 + this.IncreasedDamage;
    protected override void AfterDowngraded() => this.UpdateDamage();

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar((Decimal) this.CurrentDamage, ValueProp.Move),
        new IntVar("Increase", 2M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }

    public Task AfterSequenceActivated(PlayerChoiceContext choiceContext, Player player, IReadOnlyList<GlyphModel> glyphs)
    {
        if (player != Owner)
            return Task.CompletedTask;

        int value = DynamicVars["Increase"].IntValue;
        BuffFromPlay(value);
        if (DeckVersion is Absolute deckVersion)
            deckVersion.BuffFromPlay(value);

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Increase"].UpgradeValueBy(1m);
    }
}