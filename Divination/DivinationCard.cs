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


		private readonly Dictionary<string, DivinationValue> DivinationCardsValueMap = new Dictionary<string, DivinationValue>()
		{
			// < 0.2c
			{ "Prosperity", DivinationValue.Worthless},
			{ "Struck by Lightning", DivinationValue.Worthless },
			{ "The Inoculated", DivinationValue.Worthless },
			{ "The Metalsmith's Gift", DivinationValue.Worthless },
			{ "The Surgeon", DivinationValue.Worthless },
			{ "Lantador's Lost Love", DivinationValue.Worthless },
			{ "The Carrion Crow", DivinationValue.Worthless },
			{ "The Lover", DivinationValue.Worthless },
			{ "The Rabid Rhoa", DivinationValue.Worthless },
			{ "The Warden", DivinationValue.Worthless },
			{ "Turn the Other Cheek", DivinationValue.Worthless },
			{ "Thunderous Skies", DivinationValue.Worthless },

			// 0.2c+
			{ "The Gambler", DivinationValue.NearlyWorthless },
			{ "Destined to Crumble", DivinationValue.NearlyWorthless },
			{ "The Lord in Black", DivinationValue.NearlyWorthless },
			{ "Rain of Chaos", DivinationValue.NearlyWorthless },
			{ "Her Mask", DivinationValue.NearlyWorthless },
			{ "Loyalty", DivinationValue.NearlyWorthless },
			{ "The Gemcutter", DivinationValue.NearlyWorthless },
			{ "The Scholar", DivinationValue.NearlyWorthless},
			{ "The Survivalist", DivinationValue.NearlyWorthless},
			{ "Cartographer's Delight", DivinationValue.NearlyWorthless },
			{ "The Puzzle", DivinationValue.NearlyWorthless },
			{ "The Hermit", DivinationValue.NearlyWorthless },
			{ "Boon of Justice", DivinationValue.NearlyWorthless },
			{ "The Mountain", DivinationValue.NearlyWorthless },
			{ "Shard of Fate", DivinationValue.NearlyWorthless },
			{ "The Doppelganger", DivinationValue.NearlyWorthless },

			// 0.4c+
			{ "Three Voices", DivinationValue.ChaosLess2},
			{ "The Catalyst", DivinationValue.ChaosLess2 },
			{ "Boundless Realms", DivinationValue.ChaosLess2 },
			{ "Coveted Possession", DivinationValue.ChaosLess2 },
			{ "Emperor's Luck", DivinationValue.ChaosLess2 },
			{ "Three Faces in the Dark", DivinationValue.ChaosLess2 },
			{ "The Master Artisan", DivinationValue.ChaosLess2 },

			// 1.1c+
			{ "No Traces", DivinationValue.Chaos2to10 },
			{ "The Fool", DivinationValue.Chaos2to10 },
			{ "The Heroic Shot", DivinationValue.Chaos2to10 },
			{ "The Inventor", DivinationValue.Chaos2to10 },
			{ "The Wrath", DivinationValue.Chaos2to10 },
			{ "Lucky Connections", DivinationValue.Chaos2to10 },
			{ "The Innocent", DivinationValue.Chaos2to10 },
			{ "Vinia's Token", DivinationValue.Chaos2to10 },
			{ "The Cartographer", DivinationValue.Chaos2to10 },
			{ "Chaotic Disposition", DivinationValue.Chaos2to10 },
			{ "Demigod's Wager", DivinationValue.Chaos2to10 },

			// 10c+
			{ "Wealth and Power", DivinationValue.Chaos10 },
			{ "Alluring Bounty", DivinationValue.Chaos10 },
			{ "The Dragon's Heart",DivinationValue.Chaos10 },
			{ "House of Mirrors",DivinationValue.Chaos10 },
			{ "The Doctor", DivinationValue.Chaos10 },
			{ "The Demon", DivinationValue.Chaos10 },
			{ "The Fiend", DivinationValue.Chaos10 },
			{ "The Immortal", DivinationValue.Chaos10 },
			{ "The Nurse", DivinationValue.Chaos10 },
			{ "The Iron Bard", DivinationValue.Chaos10 },
			{ "Seven Years Bad Luck", DivinationValue.Chaos10 },
			{ "The Saint's Treasure", DivinationValue.Chaos10 },
			{ "The Eye of Terror", DivinationValue.Chaos10 },
		};

		public bool HasHardCodedValue => DivinationCardsValueMap.ContainsKey(Name);

		public DivinationValue ExpectedFilterValue {
			get {
				if(DivinationCardsValueMap.TryGetValue(Name, out DivinationValue val))
					return val;
				if (ChaosValue < 0.01f || IsLowConfidence || (ChaosValue >= FilterValue.LowValue && ChaosValue <= FilterValue.HighValue))
					return FilterValue;
				return DivinationValue.ValueOf(ChaosValue);
			}
		}

		public int SeverityLevel =>	Math.Abs(FilterValue.Tier - ExpectedFilterValue.Tier);

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
