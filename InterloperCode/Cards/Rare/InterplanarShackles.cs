using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Helpers;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Cards.Rare;

// a random enemy gains the InterplanarShacklesPower debuff, which, when on the enemy causes each time abyssalcorruption is consumed to deal 7 (amount of stacks) damage
public class InterplanarShackles() : InterloperCard(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<InterplanarShacklesPower>("InterplanarShacklesPower", 5m)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<InterplanarShacklesPower>(choiceContext, play.Target, DynamicVars["InterplanarShacklesPower"].IntValue, Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<InterplanarShacklesPower>()
        ];

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Innate);
    }
}