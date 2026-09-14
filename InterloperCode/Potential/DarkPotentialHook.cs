using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Potential;

public static class DarkPotentialHook
{
    public static async Task AfterCleared(ICombatState combatState, PlayerChoiceContext choiceContext, Player player, int level, int energy)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IAfterDarkPotentialCleared>())
        {
            var abstractModel = (AbstractModel)(object)model;
            choiceContext.PushModel(abstractModel);
            await model.AfterDarkPotentialCleared(choiceContext, player, level, energy);
            abstractModel.InvokeExecutionFinished();
            choiceContext.PopModel(abstractModel);
        }
    }

    public static async Task OnGained(ICombatState combatState, PlayerChoiceContext choiceContext, Player player, int amount)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IOnDarkPotentialGained>())
        {
            var abstractModel = (AbstractModel)(object)model;
            choiceContext.PushModel(abstractModel);
            await model.OnDarkPotentialGained(choiceContext, player, amount);
            abstractModel.InvokeExecutionFinished();
            choiceContext.PopModel(abstractModel);
        }
    }
}