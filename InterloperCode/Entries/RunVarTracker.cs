using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Interloper.InterloperCode.Entries;

public class RunVarTracker() : CustomSingletonModel(HookType.Run)
{
    public static readonly SpireField<Creature, decimal> totalSequencesPlayedInRun = new(() => 0);

    public static decimal GetTotalSequencesPlayedInRun(Creature creature)
    {
        var combatState = creature.CombatState;
        return combatState == null ? 0 : totalSequencesPlayedInRun[creature];
    }
    
    
}