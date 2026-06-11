using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class LifeLock() : InterloperCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<AbyssalCorruptionPower>("AbyssalCorruptionPower", 15)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<AbyssalCorruptionPower>(choiceContext, play.Target,
            DynamicVars["AbyssalCorruptionPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["AbyssalCorruptionPower"].UpgradeValueBy(7m);
    }
}