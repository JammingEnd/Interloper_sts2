using BaseLib.Abstracts;
using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Interloper.InterloperCode.Powers;

public class PureKeywordHook() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
            return;

        var creature = participants.FirstOrDefault(p => p.IsPlayer);
        var combatState = creature?.CombatState;
        if (combatState == null)
            return;

        foreach (var player in combatState.Players)
        {
            var exhaustPile = PileType.Exhaust.GetPile(player);
            var pureCards = exhaustPile.Cards
                .Where(c => c.Keywords.Contains(InterloperKeywords.Pure))
                .ToArray();

            foreach (var card in pureCards)
                await CardPileCmd.Add(card, PileType.Draw);
        }
    }
}