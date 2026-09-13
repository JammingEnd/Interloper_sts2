using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Potential;

public static class DarkPotentialCmd
{
    public static event Action<Player>? OnChanged;

    private static readonly DarkPotentialLevels Levels = new();

    /// <summary>
    /// Adds Dark Potential up to the max. The gained hook fires only when something was actually
    /// gained, and receives the actual gained amount (clamped), not the requested amount.
    /// </summary>
    public static async Task Add(PlayerChoiceContext choiceContext, Player player, int amount)
    {
        var state = player.PlayerCombatState?.GetDarkPotentialState();
        if (state == null || amount <= 0)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        int actual = Math.Min(state.Max, state.Current + amount) - state.Current;
        if (actual <= 0)
            return;

        state.Current += actual;
        OnChanged?.Invoke(player);

        var combatState = player.Creature.CombatState;
        if (combatState != null)
            await DarkPotentialHook.OnGained(combatState, choiceContext, player, actual);
    }

    public static async Task Clear(PlayerChoiceContext choiceContext, Player player)
    {
        var state = player.PlayerCombatState?.GetDarkPotentialState();
        if (state == null || state.Current <= 0)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        var level = GetReachedLevel(state);
        if (level > 0)
            await DispatchLevel(choiceContext, player, level);

        state.Current = 0;
        OnChanged?.Invoke(player);

        var combatState = player.Creature.CombatState;
        // AfterCleared is intentionally only fired when level > 0: a sub-threshold clear
        // wastes the bar but does not trigger level consumers like WitnessMe/Absolute.
        if (combatState != null && level > 0)
            await DarkPotentialHook.AfterCleared(combatState, choiceContext, player, level);
    }

    public static async Task SetMax(PlayerChoiceContext choiceContext, Player player, int value)
    {
        var state = player.PlayerCombatState?.GetDarkPotentialState();
        if (state == null)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        state.Max = Math.Max(1, value);
        state.Current = Math.Min(state.Current, state.Max);
        OnChanged?.Invoke(player);
    }

    private static Task DispatchLevel(PlayerChoiceContext choiceContext, Player player, int level) => level switch
    {
        1 => Levels.Level1(choiceContext, player),
        2 => Levels.Level2(choiceContext, player),
        3 => Levels.Level3(choiceContext, player),
        4 => Levels.Level4(choiceContext, player),
        5 => Levels.Level5(choiceContext, player),
        _ => Task.CompletedTask
    };

    private static int GetReachedLevel(DarkPotentialState state)
    {
        var level = 0;
        for (var i = 1; i <= DarkPotentialLevels.LevelCount; i++)
        {
            var threshold = state.Max * i / DarkPotentialLevels.LevelCount;
            if (state.Current >= threshold)
                level = i;
        }

        return level;
    }
}