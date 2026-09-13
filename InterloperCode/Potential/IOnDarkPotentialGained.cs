using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Potential;

public interface IOnDarkPotentialGained
{
    Task OnDarkPotentialGained(PlayerChoiceContext choiceContext, Player player, int amount);
}