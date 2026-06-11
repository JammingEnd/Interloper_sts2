using BaseLib.Utils;
using Interloper.InterloperCode.Entries;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Glyph;

public class GlyphStorage() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.GetType() != typeof(GlyphStorage))
        {
            return;
        }
        
        var glyphsOnPlayer = this.Owner.Powers.OfType<GlyphPower>().ToArray();

        if (glyphsOnPlayer.Length == 3)
        {
            int[] codes = new int[glyphsOnPlayer.Length];

            for (int i = 0; i < glyphsOnPlayer.Length; i++)
            {
                codes[i] = glyphsOnPlayer[i] switch
                {
                    GlyphEyePower => 0,
                    GlyphMouthPower => 1,
                    GlyphTailPower => 2,
                    _ => -1 // unknown glyph type
                };
            }

            string code = string.Join("", codes);

            // three eyes
            if (code == "000")
            {
                await CreatureCmd.Damage(choiceContext, CombatState!.HittableEnemies, 10, ValueProp.Unblockable,
                    this.Owner, null);
            }
            //TODO:more codes
            
            
            // remove after activating 
            await PowerCmd.Remove<GlyphPower>(Owner);
            
            
            // TODO: this probably is not correct
            var entry = new SequenceActivatedEntry(
                2, Owner.Player, CombatState.RoundNumber,
                CombatState.CurrentSide, CombatManager.Instance.History,
                CombatState.Players);

            await PowerCmd.Remove<GlyphStorage>(Owner);
        }
    }
}