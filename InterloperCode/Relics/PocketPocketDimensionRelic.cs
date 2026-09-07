using MegaCrit.Sts2.Core.Entities.Relics;

namespace Interloper.InterloperCode.Relics;

// cards with ethereal wont be brought back from your exhaust pile
public class PocketPocketDimensionRelic : InterloperRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;
    // edit the function that selects the cards form the exhaust pile
}