using Interloper.InterloperCode.Character;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Interloper.InterloperCode.Powers;

public class VoidCreationsPower() : InterloperPower
{
    private const int _baseCardsLeft = 5;
    private const string _cardsLeftKey = "CardsLeft";

    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override int DisplayAmount => DynamicVars[_cardsLeftKey].IntValue;

    public override PowerInstanceType InstanceType =>
        PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(_cardsLeftKey, _baseCardsLeft + 1)];

    protected override object InitInternalData() =>
        new Data();

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;

        var data = GetInternalData<Data>();
        if (!data.alreadyApplied)
        {
            DynamicVars[_cardsLeftKey].BaseValue--;
            InvokeDisplayAmountChanged();
            if (DynamicVars[_cardsLeftKey].IntValue <= 0 && !data.alreadyApplied)
            {
                var discardPile = PileType.Discard.GetPile(Owner.Player);
                var discardCards = discardPile.Cards.ToArray();
                if (discardCards.Length > 0)
                {
                    var target = discardCards[
                        Owner.Player.RunState.Rng.CombatCardGeneration
                            .NextInt(discardCards.Length)];

                    var newCard = CardFactory.GetForCombat(Owner.Player,
                        ModelDb.CardPool<InterloperCardPool>()
                            .GetUnlockedCards(Owner.Player.UnlockState,
                                CardMultiplayerConstraint.SingleplayerOnly), 1,
                        Owner.Player.RunState.Rng.CombatCardGeneration)
                        .FirstOrDefault();
    
                    if (newCard != null)
                    {
                        await CardCmd.Transform(target, newCard);
                        await CardPileCmd.Add(newCard, PileType.Hand);
                        data.alreadyApplied = true;
                    }
                }
            }
            
        }
    }

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return Task.CompletedTask;

        DynamicVars[_cardsLeftKey].BaseValue = _baseCardsLeft;
        var data = GetInternalData<Data>();
        data.alreadyApplied = false;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    private class Data
    {
        public bool alreadyApplied = false;
    }
}