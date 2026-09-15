using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Helpers;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class TwistedAbyss() : InterloperCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<TwistedAbyssPower>("TwistedAbyssPower", 6)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var oldestCard = PileType.Exhaust.GetPile(Owner)
            .GetOldestPlayableCard(includeStatus: true, includeCurse: true, includeConsumed:true);
        if (oldestCard != null)
        {
            CardModel newcard = CardFactory.GetForCombat(this.Owner,
                ModelDb.CardPool<ColorlessCardPool>()
                    .GetUnlockedCards(Owner.UnlockState, CardMultiplayerConstraint.SingleplayerOnly), 1,
                Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            if (newcard != null)
            {
                if(oldestCard.IsUpgraded)
                    CardCmd.Upgrade(newcard);
                await CardCmd.Transform(oldestCard, newcard);
            }
        }

        await PowerCmd.Apply<TwistedAbyssPower>(choiceContext, Owner.Creature, DynamicVars["TwistedAbyssPower"].IntValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        this.AddKeyword(CardKeyword.Retain);
    }
}