using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Interloper.InterloperCode.Potential;

public static class DarkPotentialField
{
    public static readonly SpireField<PlayerCombatState, DarkPotentialState> State = new(() => null);
}