using System;
using System.Linq;
using Newtonsoft.Json;

namespace PoE_Price_Lister
{
	public class DivinationCard
	{
		public DivinationCard() { }

		public DivinationCard(string name)
		{
			Name = name;
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

		public float ChaosValue { get; private set; } = -1.0f;

		public DivinationValue FilterValue { get; set; } = DivinationValue.Error;

		public DivinationValue ExpectedFilterValue {
			get {
				if (ChaosValue < 0.01f || IsLowConfidence)
					return FilterValue;
				if (FilterValue.LowValue <= ChaosValue && FilterValue.HighValue >= ChaosValue)
					return FilterValue;
				return DivinationValue.ValueOf(ChaosValue);
			}
		}

		public int SeverityLevel {
			get {
				DivinationValue expect = ExpectedFilterValue;
				if (FilterValue == expect || (FilterValue.Tier > expect.Tier && FilterValue.LowValue <= ChaosValue))
					return 0;
				return Math.Abs(FilterValue.Tier - expect.Tier);
			}
		}

		public int Tier => FilterValue.Tier;

		public int Count { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is DivinationCard other) {
				return other.Name == Name;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}
}
