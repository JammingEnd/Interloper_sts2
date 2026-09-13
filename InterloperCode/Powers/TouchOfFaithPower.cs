using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Cards.Ancient;
using Interloper.InterloperCode.Glyphs;
using Interloper.InterloperCode.Helpers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace Interloper.InterloperCode.Powers;

public class TouchOfFaithPower : InterloperPower
{
    protected override object InitInternalData() => (object) new Data();
    public override int DisplayAmount => this.GetInternalData<Data>().cardsLeft;
    private class Data
    {
        public int cardsLeft = 10;
    }
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override PowerInstanceType InstanceType =>
        PowerInstanceType.Instanced;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is TouchOfFaith || cardPlay.Card.Owner.Creature != this.Owner)
            return;
        var data = GetInternalData<Data>();
        data.cardsLeft -= (int)1;
        if (data.cardsLeft <= 0)
        {
            Flash();
            int rnd = CombatState.RunState.Rng.CombatEnergyCosts.NextInt(10);
            GlyphSequence sq = Helpers.Extentions.GetSequenceByIndex(rnd);
            await GlyphCmd.Activate(choiceContext, cardPlay.Player, sq, null, cardPlay);
            data.cardsLeft = 10;
        }
        InvokeDisplayAmountChanged();
    }

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card.Owner.Creature != this.Owner || card is not GlyphCard)
        {
            modifiedCost = originalCost;
            return false;
        }
        modifiedCost = originalCost + 1;
        return true;
    }

    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.Owner.Creature == this.Owner || card is GlyphCard)
        {
            card.AddKeyword(CardKeyword.Ethereal);
        }
    }
}