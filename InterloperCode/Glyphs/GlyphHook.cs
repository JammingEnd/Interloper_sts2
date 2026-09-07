using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Glyphs;

public static class GlyphHook
{
    public static async Task AfterSequenceActivated(ICombatState combatState, PlayerChoiceContext choiceContext, Player player,
        IReadOnlyList<GlyphModel> glyphs)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IAfterSequenceActivated>())
        {
            var abstractModel = (AbstractModel)(object)model;
            choiceContext.PushModel(abstractModel);
            await model.AfterSequenceActivated(choiceContext, player, glyphs);
            abstractModel.InvokeExecutionFinished();
            choiceContext.PopModel(abstractModel);
        }
    }
}