using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Interloper.InterloperCode.Cards.Rare;

public class HesListening() : InterloperCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int corruption = play.Target.GetPowerAmount<AbyssalCorruptionPower>();
        if (corruption <= 0)
            return;

        await PowerCmd.Apply<AbyssalCorruptionPower>(
            choiceContext, play.Target,
            -corruption, Owner.Creature, this);

        int count = corruption / 5;
        if (count > 0)
            await CardPileCmd.AutoPlayFromDrawPile(
                choiceContext, Owner, count, CardPilePosition.Top, false);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}