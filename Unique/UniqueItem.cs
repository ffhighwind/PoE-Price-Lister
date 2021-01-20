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

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PoE_Price_Lister
{
	public class UniqueItem
	{
		private static string[] CORE_LEAGUES { get; } = { "Abyss", "Breach", "Beyond", "Betrayal", "Legion", "Blight", "Essence", "Prophecy", "Metamorph", "Delirium", "Ritual" };
		private static string[] SEMI_CORE_LEAGUES { get; } = { "Delve", "Incursion", "Heist" };
		// Bestiary, Perandus, Talisman, Harbinger, Synthesis, Legacy, Harvest
		// Warbands, Tempest, Torment, Bloodlines, Rampage, Ambush, Invasion, Domination, Nemesis, Anarchy, Onslaught

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
		public bool IsLabyrinthDrop => Source == "Labyrinth";
		public bool IsPurchased => Source.StartsWith("Purchased");
		public bool IsProphecyDrop => Source.StartsWith("Prophecy");
		public bool IsBossDrop => Source.Length > 0 && !IsCrafted && !IsFated && !IsProphecyDrop && !IsLabyrinthDrop && !IsPurchased;
		public bool IsLimitedDrop => IsUnobtainable || (Source.Length > 0 && !IsProphecyDrop && !IsLabyrinthDrop);

		public string Name { get; private set; }

		public float ChaosValue { get; private set; } = -1.0f;

		public int ValueTier => UniqueValue.ValueOf(ChaosValue).Tier;

		public int Links { get; private set; }

		public int Count { get; private set; }

		private List<string> _Leagues { get; set; } = new List<string>();
		public IReadOnlyList<string> Leagues => _Leagues;

		public void SetLeague(string league)
		{
			IsCoreDrop = false;
			_Leagues = league.Split('|').Select(x => x.Trim()).ToList();
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
