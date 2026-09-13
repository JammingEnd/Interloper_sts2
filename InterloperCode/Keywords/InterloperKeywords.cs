using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Interloper.InterloperCode.Keywords;

public static class InterloperKeywords
{
    [CustomEnum("Consumed")] 
    [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Consumed;
    
    [CustomEnum("Pure")] 
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Pure;

    [CustomEnum("DarkPotential")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword DarkPotential;

}