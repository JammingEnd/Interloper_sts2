using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class GraspOfTheAbyss() : InterloperCard(3,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(28, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7m);
    }

    protected override void AfterMovedFromExhaust(CardModel card)
    {
        GraspOfTheAbyss cardsource = this;
        cardsource.TimedReturnedThisCombat++;
        if (TimedReturnedThisCombat > 1)
        {
            cardsource.EnergyCost.AddThisCombat(-1);
        }
    }

    private int _timesReturned;
    private int TimedReturnedThisCombat
    {
        get => this._timesReturned;
        set
        {
            this.AssertMutable();
            this._timesReturned = value;
        }
    }
    
}