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

		public void Clear()
		{
			ChaosValue = -1.0f;
			Count = 0;
			Links = 0;
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

		public string League {
			get => _League;
			set {
				_League = value;
				if (string.IsNullOrWhiteSpace(_League))
					IsCoreDrop = true;
				else {
					IsCoreDrop = false;
					foreach (string coreLeague in CORE_LEAGUES) {
						if (_League.Contains(coreLeague)) {
							IsCoreDrop = true;
							break;
						}
					}
				}
			}
		}

		public UniqueUsage Usage { get; private set; }

		public bool Unobtainable { get; private set; }

		public string Source { get; private set; } = "";
		private string _League { get; set; } = "";

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
