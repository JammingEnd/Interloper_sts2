using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Helpers;
using Interloper.InterloperCode.Keywords;
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
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var oldestCard = PileType.Exhaust.GetPile(Owner)
            .GetOldestPlayableCard(includeStatus: true, includeCurse: true);
        if (oldestCard != null)
        {
            CardModel newcard = CardFactory.GetForCombat(this.Owner,
                ModelDb.CardPool<ColorlessCardPool>()
                    .GetUnlockedCards(Owner.UnlockState, CardMultiplayerConstraint.SingleplayerOnly), 1,
                Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            if (newcard != null)
            {
                newcard.AddKeyword(InterloperKeywords.Consumed);
                if (this.IsUpgraded)
                {
                    newcard.BaseReplayCount += 1;
                }
                await CardCmd.Transform(oldestCard, newcard);
            }
        }
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}