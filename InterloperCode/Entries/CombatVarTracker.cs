using System.Reflection;
using BaseLib.Abstracts;
using BaseLib.Utils;
using HarmonyLib;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Entries;

public class CombatVarTracker() : CustomSingletonModel(HookType.Combat)
{
    public static readonly SpireField<Creature, decimal> totalGlypsPlayedInCombat = new(() => 0);
    
    public static readonly SpireField<Creature, decimal> totalCorruptionAppliedInTurn = new(() => 0);

    public static decimal GetTotalGlyphsPlayedCombat(Creature creature)
    {
        var combatState = creature.CombatState;
        return combatState == null ? 0 : totalGlypsPlayedInCombat[creature];
    }
    public static decimal GetTotalCorruptionAppliedTurn(Creature creature)
    {
        var combatState = creature.CombatState;
        return combatState == null ? 0 : totalCorruptionAppliedInTurn[creature];
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState != null)
        {
            if (combatState.CurrentSide == CombatSide.Player)
            {
                if (player.PlayerCombatState.TurnNumber == 1)
                {
                    CombatVarTracker.totalGlypsPlayedInCombat[player.Creature] = 0;
                }
            }

            totalCorruptionAppliedInTurn[player.Creature] = 0;
            MainFile.Logger.Info($"SET the static at turn start!");
        }
    }


    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.GetType() == typeof(AbyssalCorruptionPower))
        {
            totalCorruptionAppliedInTurn[applier] += amount;
        }
    }
}
