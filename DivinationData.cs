using System;
using System.Linq;
using Newtonsoft.Json;

namespace PoE_Price_Lister
{
    public class DivinationData
    {
        public DivinationData() { }

        public DivinationData(string name)
        {
            Name = name;
        }

        public DivinationData(JsonData item)
        {
            Load(item);
        }

        public void Load(JsonData item)
        {
            Name = item.Name;
            ChaosValue = item.ChaosValue;
            Count = item.Count;
        }

        public bool IsLowConfidence => Count < 3;

        public string Name { get; set; }

        public string QuotedName {
            get {
                if (Name.Contains(' '))
                    return "\"" + Name + "\"";
                return Name;
            }
        }

        public float ChaosValue { get; set; } = -1.0f;

        public DivinationFilterValue FilterValue { get; set; } = new DivinationFilterValue();

        public DivinationFilterValue ExpectedFilterValue {
            get {
                if (ChaosValue < 0.01)
                    return FilterValue;
                if (FilterValue.LowValue < ChaosValue && FilterValue.HighValue > ChaosValue)
                    return FilterValue;
                return DivinationFilterValue.ValueOf(ChaosValue);
            }
        }

        public int SeverityLevel {
            get {
                DivinationFilterValue expect = ExpectedFilterValue;
                if (FilterValue == expect || (ChaosValue < 0.7f && expect.Value == DivinationValueEnum.NearlyWorthless))
                    return 0;
                int expectTier = expect.ValueTier;
                int filterTier = FilterValue.ValueTier;
                int severity = Math.Abs(filterTier - expectTier);
                if (severity != 0) {
                    if (expectTier >= 4)
                        severity += 1;
                    else if (expectTier == 0)
                        severity -= 1;
                }
                return severity;
            }
        }

        public int ExpectedValueTier {
            get {
                DivinationData expectVal = new DivinationData {
                    FilterValue = DivinationFilterValue.ValueOf(ChaosValue),
                    ChaosValue = ChaosValue
                };
                return expectVal.ValueTier;
            }
        }

        public int ValueTier => FilterValue.ValueTier;

        public int Count { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            DivinationData other = (DivinationData) obj;
            return other.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
