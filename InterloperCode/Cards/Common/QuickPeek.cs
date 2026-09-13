using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Cards.Common;

public class QuikPeek() : CorruptionHandlerCard(10, 1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [InterloperKeywords.Pure];
    
    protected override Task CorruptionConsumptionEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        
    }
}