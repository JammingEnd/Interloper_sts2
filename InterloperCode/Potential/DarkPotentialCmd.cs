using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Potential;

public static class DarkPotentialCmd
{
    public static event Action<Player>? OnChanged;

    private static readonly DarkPotentialLevels Levels = new();

    public static readonly int[] EnergyThresholds = { 10, 30, 60, 100 };
    public static readonly int[] EnergyValues = { 1, 2, 3, 4 };

    /// <summary>Auto grants energy when the bar crosses energy thresholds; otherwise energy is granted on clear.</summary>
    public static Task SetAutoEnergy(PlayerChoiceContext choiceContext, Player player, bool value)
    {
        var state = player.PlayerCombatState?.GetDarkPotentialState();
        if (state == null)
            return Task.CompletedTask;

        if (CombatManager.Instance.IsOverOrEnding)
            return Task.CompletedTask;

        state.AutoGrantEnergy = value;
        ReSeedEnergyIndex(state);
        OnChanged?.Invoke(player);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Auto-resolves the Dark Potential bar: level buffs and energy fire as thresholds are
    /// reached, and the bar only clears when it reaches max.
    /// </summary>
    public static Task SetAutoUseThresholds(PlayerChoiceContext choiceContext, Player player, bool value)
    {
        var state = player.PlayerCombatState?.GetDarkPotentialState();
        if (state == null)
            return Task.CompletedTask;

        if (CombatManager.Instance.IsOverOrEnding)
            return Task.CompletedTask;

        state.AutoUseThresholds = value;
        state.LastActivatedLevelIndex = GetHighestReachedLevelIndex(state);
        OnChanged?.Invoke(player);
        return Task.CompletedTask;
    }

    private static int GetLevelThreshold(DarkPotentialState state, int level)
        => state.Max * level / DarkPotentialLevels.LevelCount;

    private static int GetHighestReachedLevelIndex(DarkPotentialState state)
    {
        var index = 0;
        for (var i = 1; i <= DarkPotentialLevels.LevelCount; i++)
        {
            if (state.Current >= GetLevelThreshold(state, i))
                index = i;
        }
        return index;
    }

    private static int GetEnergyThreshold(DarkPotentialState state, int index)
        => state.Max * EnergyThresholds[index] / 100;

    private static int GetHighestReachedEnergyIndex(DarkPotentialState state)
    {
        var index = -1;
        for (var i = 0; i < EnergyThresholds.Length; i++)
        {
            if (state.Current >= GetEnergyThreshold(state, i))
                index = i;
        }
        return index;
    }

    private static void ReSeedEnergyIndex(DarkPotentialState state)
        => state.LastGrantedEnergyIndex = GetHighestReachedEnergyIndex(state);

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

        if (state.AutoUseThresholds)
        {
            while (state.LastActivatedLevelIndex < DarkPotentialLevels.LevelCount &&
                   state.Current >= GetLevelThreshold(state, state.LastActivatedLevelIndex + 1))
            {
                state.LastActivatedLevelIndex++;
                await DispatchLevel(choiceContext, player, state.LastActivatedLevelIndex);
            }
            
            await GrantAutoEnergy(choiceContext, player, state);
            
            if (state.Current >= state.Max)
            {
                state.Clears++;
                state.Current = 0;
                state.LastActivatedLevelIndex = -1;
                state.LastGrantedEnergyIndex = -1;
            }
        }
        else if (state.AutoGrantEnergy)
        {
            await GrantAutoEnergy(choiceContext, player, state);
        }

        OnChanged?.Invoke(player);

        var combatState = player.Creature.CombatState;
        if (combatState != null)
            await DarkPotentialHook.OnGained(combatState, choiceContext, player, actual);
    }

    private static async Task GrantAutoEnergy(PlayerChoiceContext choiceContext, Player player, DarkPotentialState state)
    {
        for (var i = state.LastGrantedEnergyIndex + 1; i < EnergyThresholds.Length; i = state.LastGrantedEnergyIndex + 1)
        {
            if (state.Current < GetEnergyThreshold(state, i))
                break;

            state.LastGrantedEnergyIndex = i;
            await PlayerCmd.GainEnergy(EnergyValues[i], player);
        }
    }

    public static async Task Clear(PlayerChoiceContext choiceContext, Player player)
    {
        var state = player.PlayerCombatState?.GetDarkPotentialState();
        if (state == null || state.Current <= 0)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;
        
        var level = GetHighestReachedLevelIndex(state);
        var energyIndex = !state.AutoGrantEnergy ? GetHighestReachedEnergyIndex(state) : -1;
        if (level > 0)
        {
            state.Clears++;
            await DispatchLevel(choiceContext, player, level);
        }

        // Clear mode (default): pay the highest energy threshold the pre-reset current reached.
        int gainedEnergy = energyIndex >= 0 ? EnergyValues[energyIndex] : 0;
        if (gainedEnergy > 0)
            await PlayerCmd.GainEnergy(gainedEnergy, player);

        state.Current = 0;
        state.LastGrantedEnergyIndex = -1;
        state.LastActivatedLevelIndex = -1;
        OnChanged?.Invoke(player);

        var combatState = player.Creature.CombatState;
        if (combatState != null && level > 0)
            await DarkPotentialHook.AfterCleared(combatState, choiceContext, player, level, gainedEnergy);
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
        ReSeedEnergyIndex(state);
        OnChanged?.Invoke(player);
    }

    /// <summary>
    /// Activates the level buff one below the currently reached level (e.g. at level 4, activates
    /// level 3). Does nothing at level 1 or lower. The bar is not cleared.
    /// </summary>
    public static async Task ActivatePreviousLevel(PlayerChoiceContext choiceContext, Player player)
    {
        var state = player.PlayerCombatState?.GetDarkPotentialState();
        if (state == null)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        var level = GetHighestReachedLevelIndex(state);
        if (level < 2)
            return;

        await DispatchLevel(choiceContext, player, level - 1);
    }

    /// <summary>
    /// Sets the bar to a specific level's threshold (e.g. level 1 = 20% of max). Fires OnChanged.
    /// </summary>
    /// NOTE: this performs no awaits; return `Task.CompletedTask` (NOT an async method) to avoid CS1998.
    public static Task SetCurrentToLevel(PlayerChoiceContext choiceContext, Player player, int level)
    {
        var state = player.PlayerCombatState?.GetDarkPotentialState();
        if (state == null)
            return Task.CompletedTask;

        if (CombatManager.Instance.IsOverOrEnding)
            return Task.CompletedTask;

        state.Current = GetLevelThreshold(state, level);
        OnChanged?.Invoke(player);
        return Task.CompletedTask;
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
}