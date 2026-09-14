using BaseLib.Utils;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Potential;

public class DarkPotentialLevels
{
    public const int LevelCount = 5;

    public virtual async Task Level1(PlayerChoiceContext choiceContext, Player player)
    {
        await CardPileCmd.Draw(choiceContext, 2, player);
        await PowerCmd.Apply<VigorPower>(choiceContext, player.Creature, 3, player.Creature, null);
    }

    public virtual async Task Level2(PlayerChoiceContext choiceContext, Player player)
    {
        await CardPileCmd.Draw(choiceContext, 2, player);
        await PowerCmd.Apply<VigorPower>(choiceContext, player.Creature, 6, player.Creature, null);
    }

    public virtual async Task Level3(PlayerChoiceContext choiceContext, Player player)
    {
        await CardPileCmd.Draw(choiceContext, 3, player);
        await PowerCmd.Apply<VigorPower>(choiceContext, player.Creature, 10, player.Creature, null);
        await PowerCmd.Apply<VoidReachPower>(choiceContext, player.Creature, 2, player.Creature, null);
    }

    public virtual async Task Level4(PlayerChoiceContext choiceContext, Player player)
    {
        await CardPileCmd.Draw(choiceContext, 3, player);
        await PowerCmd.Apply<VigorPower>(choiceContext, player.Creature, 14, player.Creature, null);
        await PowerCmd.Apply<VoidReachPower>(choiceContext, player.Creature, 3, player.Creature, null);
        await CreatureCmd.GainBlock(player.Creature, 15, ValueProp.Unpowered, null, true);
    }

    public virtual async Task Level5(PlayerChoiceContext choiceContext, Player player)
    {
        await CardPileCmd.Draw(choiceContext, 4, player);
        await PowerCmd.Apply<VigorPower>(choiceContext, player.Creature, 18, player.Creature, null);
        await PowerCmd.Apply<VoidReachPower>(choiceContext, player.Creature, 4, player.Creature, null);
        await CreatureCmd.GainBlock(player.Creature, 20,ValueProp.Unpowered, null, true);
        foreach (var enemy in player.Creature.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, enemy, -2, player.Creature, null);
        }
    }

    public virtual string GetDescription(int level)
    {
        switch (level)
        {
            case 1:
                return "Draw 2 cards.\nGain 3 vigor";
            case 2:
                return "Draw 2 cards.\nGain 6 vigor";
            case 3:
                return "Draw 3 cards.\nGain 10 vigor.\nGain 2 Void Reach.";
            case 4:
                return "Draw 3 cards.\nGain 14 vigor.\nGain 3 Void Reach.\nGain 15 block.";
            case 5:
                return "Draw 4 cards.\nGain 18 vigor.\nGain 4 Void Reach.\nGain 20 block.\nEnemies lose 2 Strength";
        }   
        return "";
    }
}