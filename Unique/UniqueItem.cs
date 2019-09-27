using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PoE_Price_Lister
{
	public class UniqueItem
	{
		private static readonly string[] CORE_LEAGUES = { "Abyss", "Breach", "Beyond", "Delve", "Betrayal", "Legion", "Blight" };

		public UniqueItem(JsonData jdata)
		{
			Load(jdata);
		}

		public UniqueItem(UniqueBaseTypeCsv csvdata)
		{
			Load(csvdata);
		}

		public void Load(UniqueBaseTypeCsv csvdata)
		{
			Name = csvdata.Name;
			SetLeague(csvdata.League);
			Usage = csvdata.Usage;
			IsUnobtainable = csvdata.Unobtainable;
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

		public string Name { get; private set; }

		public float ChaosValue { get; private set; } = -1.0f;

		public int ValueTier => UniqueValue.ValueOf(ChaosValue).Tier;

		public int Links { get; private set; }

		public int Count { get; private set; }

		private List<string> _Leagues { get; set; } = new List<string>();
		public IReadOnlyList<string> Leagues => _Leagues;

		public void SetLeague(string league)
		{
			_Leagues = league.Split('|').ToList();
			foreach (string l in Leagues) {
				if (string.IsNullOrWhiteSpace(l) || CORE_LEAGUES.Any(le => le.StartsWith(l))) {
					IsCoreDrop = true;
					break;
				}
			}
		}

		public UniqueUsage Usage { get; private set; }

		public bool IsUnobtainable { get; private set; }

		public string Source { get; private set; } = "";

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is UniqueItem other) {
				return other.Name == Name && other.Links == Links;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() * (Links + 1);
		}
	}
}
