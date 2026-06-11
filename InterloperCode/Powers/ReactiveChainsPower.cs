using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Powers;

public class ReactiveChainsPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.GetType() == typeof(GlyphCard))
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, this.Amount, Owner, null);
            HasPlayed = true;
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        HasPlayed = false;
    }

    private bool _hasPlayed = false;
    private bool HasPlayed
    {
        get => this._hasPlayed;
        set
        {
            this.AssertMutable();
            this._hasPlayed = value;
        }
    }
}