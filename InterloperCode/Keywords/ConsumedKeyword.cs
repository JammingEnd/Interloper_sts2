using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Interloper.InterloperCode.Keywords;

// consumed cards cannot be retrieved from the exhaust pile
public static class ConsumedKeyword
{
    [CustomEnum] public static CardKeyword Consumed;
}