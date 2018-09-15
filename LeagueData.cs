using System.Collections.Generic;

namespace PoE_Price_Lister
{
    public class LeagueData
    {
        public LeagueData(bool hardcore)
        {
            IsHardcore = hardcore;
        }

        public bool IsHardcore { get; private set; }
        public readonly Dictionary<string, DivinationCard> DivinationCards = new Dictionary<string, DivinationCard>();
        public readonly Dictionary<string, UniqueBaseType> Uniques = new Dictionary<string, UniqueBaseType>();
    }
}
