using Interloper.InterloperCode.Cards.Glyph;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;

namespace Interloper.InterloperCode.Potions;

public sealed class DarkInsightPotion : InterloperPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var combatState = target?.CombatState ?? Owner.Creature.CombatState;
        if (combatState == null)
            return;

        for (int i = 0; i < 3; i++)
        {
            CardModel glyphCard = Owner.RunState.Rng.CombatCardGeneration.NextInt(3) switch
            {
                0 => combatState.CreateCard<GlyphEye>(Owner),
                1 => combatState.CreateCard<GlyphMouth>(Owner),
                _ => combatState.CreateCard<GlyphTail>(Owner)
            };

            await CardPileCmd.AddGeneratedCardToCombat(glyphCard, PileType.Hand, Owner);
        }
    }
}