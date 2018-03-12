using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoE_Price_Lister
{
    public class UniqueData
    {
        private string name;
        private float val = -1.0f;
        private int links;
        private int count = 0;
        private string league = "";
        private UniqueUsage usage;
        private bool unobtainable;
        private string source = "";

        private bool isCoreDrop = false;

        private static readonly string[] CORE_LEAGUES = { "Abyss", "Breach", "Beyond", "Bestiary" };

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
            name = csvdata.Name;
            League = csvdata.League;
            usage = csvdata.Usage;
            unobtainable = csvdata.Unobtainable;
            source = csvdata.Source;
        }

        public void Load(JsonData jdata)
        {
            name = jdata.Name;
            val = jdata.ChaosValue;
            links = jdata.Links;
            count = jdata.Count;
        }

        public bool IsLowConfidence
        {
            get { return count < 3; }
        }

        public bool IsCoreDrop
        {
            get { return isCoreDrop; }
        }

        public bool IsCrafted
        {
            get { return source == "Crafted"; }
        }

        public bool IsFated
        {
            get { return source == "Fated"; }
        }

        public bool IsBossDrop
        {
            get { return source != null && source.Length > 0 && !IsCrafted && !IsFated; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public float ChaosValue
        {
            get { return val; }
            set { val = value; }
        }

        public int ValueTier
        {
            get { return UniqueFilterValue.ValueOf(val).ValueTier; }

        }

        public int Links
        {
            get { return links; }
            set { links = value; }
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public string League
        {
            get { return league; }
            set
            {
                league = value;
                if (league == null || league.Length == 0)
                    isCoreDrop = true;
                else
                {
                    isCoreDrop = false;
                    foreach (string coreLeague in CORE_LEAGUES)
                    {
                        if (league.Contains(coreLeague))
                        {
                            isCoreDrop = true;
                            break;
                        }
                    }
                }
            }
        }

        public UniqueUsage Usage
        {
            get { return usage; }
            set { usage = value; }
        }

        public bool Unobtainable
        {
            get { return unobtainable; }
            set { unobtainable = value; }
        }

        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            UniqueData other = (UniqueData)obj;
            return other.name == name && other.links == links;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode() * (links + 1);
        }
    }
}
