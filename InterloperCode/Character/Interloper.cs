using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Interloper.InterloperCode.Extensions;
using Godot;
using Interloper.InterloperCode.Cards.Basic;
using Interloper.InterloperCode.Relics;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Interloper.InterloperCode.Character;
public class Interloper : PlaceholderCharacterModel
{
    public const string CharacterId = "Interloper";

    public static readonly Color Color = new("8d00cf");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 70;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeInterloper>(),
        ModelDb.Card<StrikeInterloper>(),
        ModelDb.Card<StrikeInterloper>(),
        ModelDb.Card<DefendInterloper>(),
        ModelDb.Card<DefendInterloper>(),
        ModelDb.Card<DefendInterloper>(),
        ModelDb.Card<DefendInterloper>(),
        ModelDb.Card<LowBreach>(),
        ModelDb.Card<IntoTheUnknown>(),
        ModelDb.Card<DeepScratch>(),
        ModelDb.Card<DeepScratch>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<DarkChainsRelic>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<InterloperCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<InterloperRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<InterloperPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
    // art by https://www.reddit.com/r/DnD/comments/1is5rcz/artdisintegrate_most_memorable_experiences_with/
    public override string CustomCharacterSelectBg => "char_select_bg_interloper.tscn".ScenePath();
    public override string CustomEnergyCounterPath => "res://Interloper/scenes/interloper_energy_counter.tscn";
}