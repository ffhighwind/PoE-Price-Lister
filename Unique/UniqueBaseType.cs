#region License
// Copyright © 2018 Wesley Hamilton
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at https://github.com/ffhighwind/PoE-Price-Lister
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PoE_Price_Lister
{
	public class UniqueBaseType
	{
		private static readonly string[] SIX_SOCKETS = new string[] { "Tabula Rasa", "Loreweave", "Oni-Goroshi" };

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
			if (Items.Count == 1) {
				float val = Items[0].ChaosValue;
				if (val < 0.01 || Items[0].IsLowConfidence || (val >= FilterValue.LowValue && val <= FilterValue.HighValue))
					return FilterValue;
				return UniqueValue.ValueOf(val);
			}
			bool isUnobtainable = true;
			float minVal = float.MaxValue;
			float maxVal = float.MinValue;
			float minValLimited = float.MaxValue;
			float maxValLimited = float.MinValue;
			float minValLeague = float.MaxValue;
			float maxValLeague = float.MinValue;
			float minValCrafted = float.MaxValue;
			float maxValCrafted = float.MinValue;
			int minTier = 0;
			float filterVal = FilterValue.LowValue + (FilterValue.HighValue - FilterValue.LowValue) / 2.0f;

			foreach (UniqueItem uniq in Items) {
				if (uniq.IsUnobtainable)
					continue;
				isUnobtainable = false;
				if (uniq.IsPurchased || uniq.IsCrafted || uniq.IsFated) {
					if (uniq.Count > 0) {
						minValCrafted = Math.Min(minValCrafted, uniq.ChaosValue);
						maxValCrafted = Math.Max(maxValCrafted, uniq.ChaosValue);
					}
					continue;
				}
				if (uniq.Count == 0) {
					minTier = Math.Min(minTier, 1);
					continue;
				}
				if (!uniq.IsCoreDrop) {
					minValLeague = Math.Min(minValLeague, uniq.ChaosValue);
					maxValLeague = Math.Max(maxValLeague, uniq.ChaosValue);
					if (uniq.ChaosValue >= 8.0f)
						minTier = Math.Max(minTier, 1);
					continue;
				}
				if (uniq.Source.Length > 0 && !uniq.IsProphecyDrop) { // IsProphecyDrop || uniq.IsLabyrinthDrop || uniq.IsBossDrop
					minValLimited = Math.Min(minValLimited, uniq.ChaosValue);
					maxValLimited = Math.Max(maxValLimited, uniq.ChaosValue);
					if (uniq.ChaosValue >= 5.0f)
						minTier = Math.Max(minTier, 1);
					continue;
				}
				if (uniq.Leagues.Any(x => x.Length == 0)) {
					if (uniq.IsLowConfidence) {
						if (minVal > maxVal) {
							minVal = filterVal;
							maxVal = filterVal;
						}
						if (uniq.ChaosValue >= 40.0f)
							minTier = Math.Max(minTier, 3);
						if (uniq.ChaosValue >= 15.0f)
							minTier = Math.Max(minTier, 2);
						else if (uniq.ChaosValue >= 5.0f)
							minTier = Math.Max(minTier, 1);
						continue;
					}
					else {
						minVal = Math.Min(minVal, uniq.ChaosValue);
						maxVal = Math.Max(maxVal, uniq.ChaosValue);
					}
				}
				else {
					minValLimited = Math.Min(minValLimited, uniq.ChaosValue);
					maxValLimited = Math.Max(maxValLimited, uniq.ChaosValue);
					if (uniq.ChaosValue >= 15.0f)
						minTier = Math.Max(minTier, 2);
					if (uniq.ChaosValue >= 5.0f)
						minTier = Math.Max(minTier, 1);
				}
			}

			if (isUnobtainable)
				return UniqueValue.Chaos15;
			if (minVal > maxVal) {
				if (minValLimited <= maxValLimited) {
					minVal = minValLimited;
					maxVal = maxValLimited;
				}
				else if (minValLeague <= maxValLeague) {
					minVal = minValLeague;
					maxVal = maxValLeague;
					minTier = Math.Max(minTier, 2);
				}
				else if (minValCrafted <= maxValCrafted) {
					minVal = minValCrafted;
					maxVal = maxValCrafted;
					minTier = Math.Max(minTier, 2);
				}
				else
					return FilterValue;
			}

			if (minVal >= 14.0f)
				return UniqueValue.Chaos15;
			if (minVal >= 4.7f || maxVal >= 100.0f || minTier == 3)
				return UniqueValue.Chaos5to15;
			if (minVal >= 2.95f || maxVal >= 15.0f || minTier == 2)
				return UniqueValue.Chaos3to5;
			if (maxVal >= 7.0f || minTier == 1)
				return UniqueValue.Limited;
			return UniqueValue.Worthless;
		}

		public float? MaxValue {
			get {
				float max = 0;
				float maxLimited = 0;
				float maxLeague = 0;
				foreach (UniqueItem item in Items.Where(x => x.ChaosValue > 0 && !x.IsUnobtainable)) {
					if (item.IsPurchased || item.IsCrafted || item.IsFated)
						maxLimited = Math.Max(maxLimited, item.ChaosValue);
					else if (item.IsCoreDrop)
						max = Math.Max(max, item.ChaosValue);
					else
						maxLeague = Math.Max(maxLeague, item.ChaosValue);
				}
				if (max > 0)
					return max;
				if (maxLeague > 0)
					return maxLeague;
				if (maxLimited > 0)
					return maxLimited;
				return null;
			}
		}

		public float? MinValue {
			get {
				float min = float.MaxValue;
				float minLimited = float.MaxValue;
				float minLeague = float.MaxValue;
				foreach (UniqueItem item in Items.Where(x => x.ChaosValue > 0 && !x.IsUnobtainable)) {
					if (item.IsPurchased || item.IsCrafted || item.IsFated)
						minLimited = Math.Min(minLimited, item.ChaosValue);
					else if (item.IsCoreDrop)
						min = Math.Min(min, item.ChaosValue);
					else
						minLeague = Math.Min(minLeague, item.ChaosValue);
				}
				if (min != float.MaxValue)
					return min;
				if (minLeague != float.MaxValue)
					return minLeague;
				if (minLimited != float.MaxValue)
					return minLimited;
				return null;
			}
		}

		private UniqueValue _ExpectedFilterValue = null;
		public UniqueValue ExpectedFilterValue {
			get {
				if (_ExpectedFilterValue == null)
					_ExpectedFilterValue = CalculateExpectedValue();
				return _ExpectedFilterValue;
			}
		}

		public int SeverityLevel => Math.Abs(Math.Max(FilterValue.Tier - 1, 0) - Math.Max(ExpectedFilterValue.Tier - 1, 0));

		public string QuotedBaseType {
			get {
				if (BaseType.Contains(' '))
					return "\"" + BaseType + "\"";
				return BaseType;
			}
		}

		public UniqueValue FilterValue { get; set; } = UniqueValue.Worthless;

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

		private string _Leagues { get; set; }
		public string Leagues {
			get {
				if (_Leagues == null) {
					HashSet<string> leagues = new HashSet<string>();
					foreach (UniqueItem item in Items) {
						foreach (string league in item.Leagues) {
							leagues.Add(league);
						}
					}
					_Leagues = string.Join("|", leagues.OrderBy(x => x));
				}
				return _Leagues;
			}
		}

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
			_Items.Sort((x, y) => {
				if (y.IsCoreDrop) {
					if (!x.IsCoreDrop)
						return -1;
				}
				else if (x.IsCoreDrop)
					return 1;
				if (y.Source.Length > 0) {
					if (x.Source.Length == 0)
						return -1;
				}
				else if (x.Source.Length == 0)
					return 1;
				return x.Name.CompareTo(y.Name);
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
