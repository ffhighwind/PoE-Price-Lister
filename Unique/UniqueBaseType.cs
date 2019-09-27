using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PoE_Price_Lister
{
	public class UniqueBaseType
	{
		private static string[] SIX_SOCKETS = new string[] { "Tabula Rasa", "Loreweave", "Oni-Goroshi" };

		public UniqueBaseType(string baseType)
		{
			BaseType = baseType;
		}

		public void Load(UniqueBaseTypeCsv item)
		{
			foreach (UniqueItem i in _Items) {
				if (i.Name == item.Name) {
					i.Load(item);
					return;
				}
			}
			_Items.Add(new UniqueItem(item));
		}

		public bool Add(JsonData item)
		{
			foreach (UniqueItem i in _Items) {
				if (i.Name == item.Name) {
					if (i.Count == 0 || ((i.Links >= item.Links && !SIX_SOCKETS.Contains(i.Name)) || i.Links < item.Links))
						i.Load(item);
					return true;
				}
			}
			_Items.Add(new UniqueItem(item));
			return false;
		}

		private UniqueValue CalculateExpectedValue()
		{
			if (!Items.Any()) {
				return FilterValue;
			}
			bool isUnobtainable = true;
			float minVal = float.MaxValue;
			float maxVal = float.MinValue;
			float minValShared = float.MaxValue;
			float maxValShared = float.MinValue;
			float minValCrafted = float.MaxValue;
			float maxValCrafted = float.MinValue;
			int minTier = 0;
			float filterVal = FilterValue.LowValue + (FilterValue.HighValue - FilterValue.LowValue) / 2.0f;

			foreach (UniqueItem uniq in Items) {
				if (uniq.IsUnobtainable)
					continue;
				isUnobtainable = false;
				if (uniq.Count == 0)
					continue;
				if (uniq.IsPurchased || uniq.IsCrafted || uniq.IsFated) {
					minValCrafted = Math.Min(minValCrafted, uniq.ChaosValue);
					maxValCrafted = Math.Max(maxValCrafted, uniq.ChaosValue);
					continue;
				}
				if (uniq.IsProphecyDrop) {
					minValShared = Math.Min(minValShared, uniq.ChaosValue);
					maxValShared = Math.Max(maxValShared, uniq.ChaosValue);
					if (uniq.ChaosValue > 7.0f)
						minTier = Math.Max(minTier, 1);
					continue;
				}
				if (uniq.IsBossDrop) {
					minValShared = Math.Min(minValShared, uniq.ChaosValue);
					maxValShared = Math.Max(maxValShared, uniq.ChaosValue);
					if (uniq.ChaosValue > 15.0f)
						minTier = 2;
					else if (uniq.ChaosValue > 4.0f)
						minTier = 1;
					continue;
				}
				if (!uniq.IsCoreDrop) {
					minValShared = Math.Min(minValShared, uniq.ChaosValue);
					maxValShared = Math.Max(maxValShared, uniq.ChaosValue);
					if (uniq.ChaosValue > 50.0f)
						minTier = 2;
					else if (uniq.ChaosValue > 15.0f)
						minTier = Math.Max(minTier, 1);
					continue;
				}
				if (uniq.IsLowConfidence) {
					if (minVal > maxVal) {
						minVal = filterVal;
						maxVal = filterVal;
					}
					continue;
				}
				minVal = Math.Min(minVal, uniq.ChaosValue);
				maxVal = Math.Max(maxVal, uniq.ChaosValue);
			}

			if (isUnobtainable)
				return UniqueValue.Chaos10;
			if (minVal > maxVal) {
				if (minValShared <= maxValShared) {
					minVal = minValShared;
					maxVal = maxValShared;
				}
				else if (minValCrafted <= maxValCrafted) {
					minVal = minValCrafted;
					maxVal = maxValCrafted;
					minTier = Math.Max(minTier, 1);
				}
				else
					return FilterValue;
			}

			if (minVal > 9.7f)
				return UniqueValue.Chaos10;
			if (minVal > 3.0f || maxVal > 100.0f)
				return UniqueValue.Chaos3to10;
			if (minVal > 2.01f || maxVal > 20.0f)
				return UniqueValue.Chaos2to3;
			if (maxVal > 14.0f)
				return UniqueValue.Shared;
			return UniqueValue.FromTier(minTier);
		}

		private UniqueValue _ExpectedFilterValue = null;
		public UniqueValue ExpectedFilterValue {
			get {
				if (_ExpectedFilterValue == null)
					_ExpectedFilterValue = CalculateExpectedValue();
				return _ExpectedFilterValue;
			}
		}

		public int SeverityLevel {
			get {
				return Math.Abs(FilterValue.Tier - ExpectedFilterValue.Tier);
			}
		}

		public string QuotedBaseType {
			get {
				if (BaseType.Contains(' '))
					return "\"" + BaseType + "\"";
				return BaseType;
			}
		}

		public UniqueValue FilterValue { get; set; } = UniqueValue.Unknown;

		private List<UniqueItem> _Items { get; set; } = new List<UniqueItem>();
		public IReadOnlyList<UniqueItem> Items => _Items;

		public IEnumerable<UniqueItem> OrderedItems {
			get {
				foreach (UniqueItem uni in Items.Where(i => !i.IsLimitedDrop).OrderBy(it => it.ChaosValue)) {
					yield return uni;
				}
				foreach (UniqueItem uni in Items.Where(i => i.IsLimitedDrop).OrderBy(it => it.ChaosValue)) {
					yield return uni;
				}
			}
		}

		public string BaseType { get; private set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is UniqueBaseType other) {
				return other.BaseType == BaseType;
			}
			return false;
		}

		public void Sort()
		{
			_Items.Sort((x, y) => x.Name.CompareTo(y.Name));
			_Items.Sort((x, y) =>
			{
				if (y.IsLimitedDrop) {
					if (!x.IsLimitedDrop)
						return -1;
				}
				else if (x.IsLimitedDrop)
					return 1;
				return x.Leagues.First().CompareTo(y.Leagues.First());
			});
		}

		public override int GetHashCode()
		{
			return BaseType.GetHashCode();
		}

		public string GetString()
		{
			List<string> values = new List<string>();
			foreach (UniqueItem udata in OrderedItems) {
				string value = "";
				if (udata.Links > 4)
					value = "(" + udata.Links + "L)";
				string v = udata.Count == 0 ? "" : udata.ChaosValue.ToString();
				if (udata.IsLowConfidence)
					v = v + "?";
				values.Add(value + udata.Name + ": " + v);
			}
			return string.Join(", ", values);
		}
	}
}
