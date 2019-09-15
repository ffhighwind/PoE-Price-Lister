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

		public void ClearJson()
		{
			foreach (UniqueItem item in _Items) {
				item.Clear();
			}
		}

		public void CalculateExpectedValue()
		{
			if (!Items.Any()) {
				ExpectedFilterValue = FilterValue;
				return;
			}

			float minValueBoss = float.MaxValue;
			float maxValueBoss = float.MinValue;
			float minValueLeague = float.MaxValue;
			float maxValueLeague = float.MinValue;
			float minValueCraftedFated = float.MaxValue;
			float maxValueCraftedFated = float.MinValue;
			float minValue = float.MaxValue;
			float maxValue = float.MinValue;
			bool isLeagueOnly = true;
			bool hasLeague = false;
			bool hasCoreLeague = false;
			bool hasLabyrinth = false;
			bool isBossOnly = true;
			bool hasBoss = false;
			bool isCraftedOnly = true;
			bool isUnobtainable = true;
			int minExpectedTier = 1;
			UniqueValue minExpected = UniqueValue.Unknown;

			// Determine Expected Value
			foreach (UniqueItem uniqData in Items) {
				if (uniqData.Unobtainable)
					continue;
				isUnobtainable = false;
				if (uniqData.IsCrafted || uniqData.IsFated || uniqData.IsPurchased) {
					if (!uniqData.IsLowConfidence) {
						if (minValueCraftedFated > uniqData.ChaosValue)
							minValueCraftedFated = uniqData.ChaosValue;
						if (maxValueCraftedFated < uniqData.ChaosValue)
							maxValueCraftedFated = uniqData.ChaosValue;
					}
					continue;
				}
				isCraftedOnly = false;

				if (uniqData.IsCoreDrop) {
					isLeagueOnly = false;
					hasCoreLeague = hasCoreLeague || uniqData.League.Length != 0;
					if (uniqData.IsLimitedDrop) {
						if (uniqData.IsLabyrinthDrop)
							hasLabyrinth = true;
						else
							hasBoss = true;
						if (!uniqData.IsLowConfidence) {
							if (minValueBoss > uniqData.ChaosValue)
								minValueBoss = uniqData.ChaosValue;
							if (maxValueBoss < uniqData.ChaosValue)
								maxValueBoss = uniqData.ChaosValue;
						}
					}
					else {
						isBossOnly = false;
						if (!uniqData.IsLowConfidence && uniqData.Links != 6) {
							if (minValue > uniqData.ChaosValue)
								minValue = uniqData.ChaosValue;
							if (maxValue < uniqData.ChaosValue)
								maxValue = uniqData.ChaosValue;
						}
					}
				}
				else {
					hasLeague = true;
					if (!uniqData.IsLowConfidence) {
						if (minValueLeague > uniqData.ChaosValue)
							minValueLeague = uniqData.ChaosValue;
						if (maxValueLeague < uniqData.ChaosValue)
							maxValueLeague = uniqData.ChaosValue;
					}
				}
			}

			if (isUnobtainable) {
				ExpectedFilterValue = UniqueValue.Chaos10; // unique exists in permanent leagues only
				return;
			}
			else if (isCraftedOnly) {
				minValue = minValueCraftedFated;
				maxValue = maxValueCraftedFated;
				minExpected = UniqueValue.ChaosLess1Crafted;
			}
			else if (isLeagueOnly) {
				minValue = minValueLeague;
				maxValue = maxValueLeague;
				minExpected = UniqueValue.ChaosLess1League;
			}
			else if (isBossOnly) {
				minValue = minValueBoss;
				maxValue = maxValueBoss;
				if (hasBoss)
					minExpected = UniqueValue.ChaosLess1Boss;
				else
					minExpected = UniqueValue.ChaosLess1Labyrinth;
			}
			else if (hasLeague) {
				if (minValueLeague > 6.0f)
					minExpected = UniqueValue.ChaosLess1Shared;
				else
					minExpected = UniqueValue.ChaosLess1;
			}
			else if (hasBoss) {
				minExpected = UniqueValue.ChaosLess1Boss;
			}
			else if (hasCoreLeague) {
				minExpected = UniqueValue.ChaosLess1League;
			}
			else if (hasLabyrinth)
				minExpected = UniqueValue.ChaosLess1;
			else
				minExpectedTier = 0;

			// Set Expected Value
			if (minValue > maxValue) //no confident value
				ExpectedFilterValue = FilterValue;
			else //confident value
			{
				ExpectedFilterValue = UniqueValue.ValueOf(minValue);
				if (ExpectedFilterValue.Tier <= 1) {
					if (maxValue > 50.0f)
						ExpectedFilterValue = UniqueValue.Chaos2to10;
					else if (maxValue > 9.0f || (minValue > 0.95f && maxValue > 1.8f))
						ExpectedFilterValue = UniqueValue.Chaos1to2;
					else if (maxValue > 2.0f || minValue > 0.95f)
						ExpectedFilterValue = UniqueValue.ChaosLess1;
				}
				else if (ExpectedFilterValue.Value == UniqueValueEnum.Chaos1to2 && minValue > 1.9f && maxValue > 4.9f)
					ExpectedFilterValue = UniqueValue.Chaos2to10;
			}
			if (ExpectedFilterValue.Tier <= minExpectedTier)
				ExpectedFilterValue = minExpected;
		}

		public UniqueValue ExpectedFilterValue { get; private set; } = UniqueValue.Error;

		public int SeverityLevel {
			get {
				if (FilterValue == ExpectedFilterValue)
					return 0;
				int expectTier = ExpectedFilterValue.Tier;
				int filterTier = FilterValue.Tier;
				int severity = Math.Abs(filterTier - expectTier);
				if (severity != 0 && filterTier < expectTier && expectTier > 2)
					severity += 1;
				return severity;
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

		public List<UniqueItem> _Items { get; private set; } = new List<UniqueItem>();
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
				return x.League.CompareTo(y.League);
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
				string v = udata.Count > 0 ? udata.ChaosValue.ToString() : "?";
				values.Add(value + udata.Name + ": " + v);
			}
			return string.Join(", ", values);
		}
	}
}
