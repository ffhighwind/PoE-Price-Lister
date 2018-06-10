using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoE_Price_Lister
{
    public class DivinationData
    {
        DivinationFilterValue filterValue = new DivinationFilterValue();
        string name;
        float listedValue = -1.0f;
        int count;

        public DivinationData() { }

        public DivinationData(string name)
        {
            this.name = name;
        }

        public DivinationData(JsonData item)
        {
            Load(item);
        }

        public void Load(JsonData item)
        {
            name = item.Name;
            listedValue = item.ChaosValue;
            count = item.Count;
        }

        public bool IsLowConfidence {
            get { return count < 3; }
        }

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public string QuotedName {
            get {
                if (name.Contains(' '))
                    return "\"" + name + "\"";
                return name;
            }
        }

        public float ChaosValue {
            get { return listedValue; }
            set { listedValue = value; }
        }

        public DivinationFilterValue FilterValue {
            get { return filterValue; }
            set { filterValue = value; }
        }

        public DivinationFilterValue ExpectedFilterValue {
            get {
                if (listedValue < 0.01)
                    return filterValue;
                if (filterValue.LowValue < listedValue && filterValue.HighValue > listedValue)
                    return filterValue;
                return DivinationFilterValue.ValueOf(listedValue);
            }
        }

        public int SeverityLevel {
            get {
                var expect = ExpectedFilterValue;
                if (filterValue == expect || (listedValue < 0.7f && expect.Value == DivinationValueEnum.NearlyWorthless))
                    return 0;
                int expectTier = expect.ValueTier;
                int filterTier = filterValue.ValueTier;
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
                var expectVal = new DivinationData();
                expectVal.filterValue = DivinationFilterValue.ValueOf(listedValue);
                expectVal.listedValue = listedValue;
                return expectVal.ValueTier;
            }
        }

        public int ValueTier {
            get {
                return (int)filterValue.ValueTier;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            DivinationData other = (DivinationData)obj;
            return other.name == name;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}
