using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Potential;

public class DarkPotentialLevels
{
    public const int LevelCount = 5;

    public virtual Task Level1(PlayerChoiceContext choiceContext, Player player)
    {
        return Task.CompletedTask;
    }

    public virtual Task Level2(PlayerChoiceContext choiceContext, Player player)
    {
        return Task.CompletedTask;
    }

    public virtual Task Level3(PlayerChoiceContext choiceContext, Player player)
    {
        return Task.CompletedTask;
    }

    public virtual Task Level4(PlayerChoiceContext choiceContext, Player player)
    {
        return Task.CompletedTask;
    }

    public virtual Task Level5(PlayerChoiceContext choiceContext, Player player)
    {
        return Task.CompletedTask;
    }

    public virtual string GetDescription(int level) => "";
}