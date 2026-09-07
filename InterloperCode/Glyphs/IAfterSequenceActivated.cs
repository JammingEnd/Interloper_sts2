using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Glyphs;

public interface IAfterSequenceActivated
{
    Task AfterSequenceActivated(PlayerChoiceContext choiceContext, Player player, IReadOnlyList<GlyphModel> glyphs);
}