using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Potential;

public interface IAfterDarkPotentialCleared
{
    Task AfterDarkPotentialCleared(PlayerChoiceContext choiceContext, Player player, int level);
}