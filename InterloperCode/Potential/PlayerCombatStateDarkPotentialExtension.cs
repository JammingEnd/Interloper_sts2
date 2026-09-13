using MegaCrit.Sts2.Core.Entities.Players;

namespace Interloper.InterloperCode.Potential;

public static class PlayerCombatStateDarkPotentialExtension
{
    public static DarkPotentialState? GetDarkPotentialState(this PlayerCombatState state)
    {
        return DarkPotentialField.State[state];
    }

    public static int GetDarkPotential(this PlayerCombatState state)
    {
        var darkPotential = state.GetDarkPotentialState();
        return darkPotential?.Current ?? 0;
    }

    public static int GetDarkPotentialMax(this PlayerCombatState state)
    {
        var darkPotential = state.GetDarkPotentialState();
        return darkPotential?.Max ?? 100;
    }

    public static double GetDarkPotentialProgress(this PlayerCombatState state)
    {
        var darkPotential = state.GetDarkPotentialState();
        if (darkPotential == null || darkPotential.Max <= 0)
        {
            return 0;
        }

        var progress = (double)darkPotential.Current / darkPotential.Max;
        return Math.Clamp(progress, 0, 1);
    }
}