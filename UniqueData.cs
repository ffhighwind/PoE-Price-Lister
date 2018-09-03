﻿using Newtonsoft.Json;

namespace PoE_Price_Lister
{
    public class UniqueData
    {
        private static readonly string[] CORE_LEAGUES = { "Abyss", "Breach", "Beyond", "Delve" };

        public UniqueData() { }

        public UniqueData(JsonData jdata)
        {
            Load(jdata);
        }

        public UniqueData(UniqueCsvData csvdata)
        {
            Load(csvdata);
        }

        public void Load(UniqueCsvData csvdata)
        {
            Name = csvdata.Name;
            League = csvdata.League;
            Usage = csvdata.Usage;
            Unobtainable = csvdata.Unobtainable;
            Source = csvdata.Source;
        }

        public void Load(JsonData jdata)
        {
            Name = jdata.Name;
            ChaosValue = jdata.ChaosValue;
            Links = jdata.Links;
            Count = jdata.Count;
        }

        public bool IsLowConfidence => Count < 3;

        public bool IsCoreDrop { get; private set; } = false;

        public bool IsCrafted => Source == "Crafted";

        public bool IsFated => Source == "Fated";

        public bool IsPurchased => Source.StartsWith("Purchased");

        public bool IsProphecyDrop => Source.StartsWith("Prophecy");

        public bool IsLabyrinthDrop => Source == "Labyrinth";

        public bool IsBossDrop => Source.Length > 0 && !IsCrafted && !IsFated && !IsProphecyDrop && !IsLabyrinthDrop;

        public bool IsLimitedDrop => Source.Length > 0 && !IsCrafted && !IsFated;

        public string Name { get; set; }

        public float ChaosValue { get; set; } = -1.0f;

        public int ValueTier => UniqueFilterValue.ValueOf(ChaosValue).ValueTier;

        public int Links { get; set; }

        public int Count { get; set; } = 0;

        public string League {
            get => League1;
            set {
                League1 = value;
                if (League1 == null || League1.Length == 0)
                    IsCoreDrop = true;
                else {
                    IsCoreDrop = false;
                    foreach (string coreLeague in CORE_LEAGUES) {
                        if (League1.Contains(coreLeague)) {
                            IsCoreDrop = true;
                            break;
                        }
                    }
                }
            }
        }

        public UniqueUsage Usage { get; set; }

        public bool Unobtainable { get; set; }

        public string Source { get; set; } = "";
        public string League1 { get; set; } = "";

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            UniqueData other = (UniqueData) obj;
            return other.Name == Name && other.Links == Links;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() * (Links + 1);
        }
    }
}
