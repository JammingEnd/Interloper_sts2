using Interloper.InterloperCode.Cards.Glyph;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Powers;

public class AddedBenefitPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card.Owner != Owner.Player)
            return;

        if (oldPileType == PileType.Exhaust && card.EnergyCost.Canonical >= 2)
        {
            CardModel glyphCard;
            int roll = Owner.Player.RunState.Rng.CombatCardGeneration.NextInt(3);
            glyphCard = roll switch
            {
                0 => CombatState.CreateCard<GlyphEye>(Owner.Player),
                1 => CombatState.CreateCard<GlyphMouth>(Owner.Player),
                _ => CombatState.CreateCard<GlyphTail>(Owner.Player),
            };

            await CardPileCmd.AddGeneratedCardToCombat(glyphCard, PileType.Hand, Owner.Player);
        }
    }
}