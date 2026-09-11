using Interloper.InterloperCode.Cards.Glyph;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;

namespace Interloper.InterloperCode.Powers;

public class ByStringsPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
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
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<GlyphEye>(false),
        HoverTipFactory.FromCard<GlyphMouth>(false),
        HoverTipFactory.FromCard<GlyphTail>(false)
    ];
}