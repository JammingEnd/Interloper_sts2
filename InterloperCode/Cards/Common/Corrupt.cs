using BaseLib.Extensions;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Interloper.InterloperCode.Cards.Common;

// apply 5  corruption
public class Corrupt() : InterloperCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<AbyssalCorruptionPower>("AbyssalCorruptionPower",7)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, play.Target, DynamicVars["AbyssalCorruptionPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["AbyssalCorruption"].UpgradeValueBy(3m);
    }
}