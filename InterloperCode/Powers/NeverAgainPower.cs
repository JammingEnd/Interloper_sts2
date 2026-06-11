using Interloper.InterloperCode.Cards.Glyph;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Interloper.InterloperCode.Powers;

public class NeverAgainPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount);
        Func<CardModel, bool> filter = c => !c.Keywords.Contains(ConsumedKeyword.Consumed);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner.Player, prefs, filter, this);
        var targets = selected.ToList();
        if (targets.Count > 0)
        {
            foreach (var target in targets)
            {
                target.AddKeyword(ConsumedKeyword.Consumed);
            }
        }
    }
}