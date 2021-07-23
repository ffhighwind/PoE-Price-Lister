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
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace PoE_Price_Lister
{
	public class PoeNinja
	{
		// All URLs for poe.ninja/api are here:
		// https://github.com/5k-mirrors/misc-poe-tools/blob/master/doc/poe-ninja-api.md
		// Alternate API (POE watch)
		// http://api.poe.watch/get?league=Metamorph&category=armour
		// http://api.poe.watch/compact?league=Metamorph&category=armour
		// ALTERNATE API? https://poe.watch/prices?category=enchantment&league={1}

		private readonly HttpClient httpClient = new HttpClient();
		private readonly List<ItemData> data = new List<ItemData>();
		private const string baseURL = @"https://poe.ninja/api/data/";
		private const string jsonURL = @"https://poe.ninja/api/data/{0}overview?type={1}&league={2}";

		private bool Hardcore;
		private string League;

		private PoeNinja()
		{
			httpClient.BaseAddress = new Uri(baseURL);
		}

		/// <summary>
		/// Loads data from POE Ninja synchronously.
		/// </summary>
		public static List<ItemData> GetData(bool hardcore, string league)
		{
			PoeNinja poeNinja = new PoeNinja();
			Exception ex = Task.Run(async () => await poeNinja.LoadAsync(hardcore, league)).Result;
			if (ex != null)
				throw ex;
			return poeNinja.data;
		}

		/// <summary>
		/// Loads data from POE Ninja asynchronously.
		/// </summary>
		private async Task<Exception> LoadAsync(bool hardcore, string league)
		{
			Hardcore = hardcore;
			League = league;
			try {
				Dictionary<string, Task<IReadOnlyList<ItemData>>> tasks = new Dictionary<string, Task<IReadOnlyList<ItemData>>> {
					// GENERAL
					{ "Currency", QueryAsync("currency", "Currency") },
					{ "Fragment", QueryAsync("currency", "Fragment") },
					{ "DivinationCard", QueryAsync("item", "DivinationCard") },
					{ "Prophecy", QueryAsync("item", "Prophecy") },
					{ "Oil", QueryAsync("item", "Oil") },
					{ "Incubator", QueryAsync("item", "Incubator") },

					// EQUIPMENT & GEMS
					{ "UniqueWeapon", QueryAsync("item", "UniqueWeapon") },
					{ "UniqueArmour", QueryAsync("item", "UniqueArmour") },
					{ "UniqueAccessory", QueryAsync("item", "UniqueAccessory") },
					{ "UniqueFlask", QueryAsync("item", "UniqueFlask") },
					//{ "UniqueJewel", QueryAsync("item", "UniqueJewel") }, // 
					//{ "SkillGem", QueryAsync("item", "SkillGem") }, //

					// ATLAS
					//{ "Map", QueryAsync("item", "Map") }, //
					//{ "BlightedMap", QueryAsync("item", "BlightedMap") }, //
					//{ "UniqueMap", QueryAsync("item", "UniqueMap") }, //
					{ "DeliriumOrb", QueryAsync("item", "DeliriumOrb") },
					//{ "Invitation", QueryAsync("item", "Invitation") }, //
					{ "Scarab", QueryAsync("item", "Scarab") },
					//{ "Watchstone", QueryAsync("item", "Watchstone") }, //

					// CRAFTING
					//{ "BaseType", QueryAsync("item", "BaseType") },
					{ "Fossil", QueryAsync("item", "Fossil") },
					{ "Resonator", QueryAsync("item", "Resonator") },
					{ "HelmetEnchant", QueryAsync("item", "HelmetEnchant") },
					//{ "Beast", QueryAsync("item", "Beast") }, //
					{ "Essence", QueryAsync("item", "Essence") },
					{ "Vial", QueryAsync("item", "Vial") }
				};

				foreach (var task in tasks) {
					LoadData(task.Key, await task.Value);
				}
			}
			catch (Exception e) {
				return e;
			}
			return null;
		}

		/// <summary>
		/// Returns JSON data from POE Ninja for the league/category.
		/// </summary>
		private async Task<IReadOnlyList<ItemData>> QueryAsync(string category, string type)
		{
			string url = string.Format(jsonURL, category, type, Hardcore ? ("Hardcore " + League) : League);
			HttpResponseMessage response = await httpClient.GetAsync(url).ConfigureAwait(false);
			response.EnsureSuccessStatusCode();
			return ParseData(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Parses a JSON string from POE Ninja.
		/// </summary>
		private static List<ItemData> ParseData(string json)
		{
			JObject jObject = JObject.Parse(json);
			JToken jTokens = jObject["lines"];
			List<ItemData> list = new List<ItemData>();
			foreach (JToken result in jTokens) {
				ItemData jdata = result.ToObject<ItemData>();
				if (jdata.ChaosValue == 0 || jdata.Count == 0)
					continue; // skip weird data
				list.Add(jdata);
			}
			return list;
		}

		/// <summary>
		/// Loads POE Ninja data into the dictionary.
		/// </summary>
		private void LoadData(string type, IEnumerable<ItemData> items)
		{
			string previous = null;
			foreach (ItemData item in items.OrderBy(x => x.BaseType ?? "").ThenBy(x => x.Name).ThenByDescending(x => x.Links < 5).ThenBy(x => x.Corrupted).ThenBy(x => x.Links).ThenByDescending(x => x.ChaosValue)) {
				// fewest links added first, and the current item should be ignored if it's a duplicates
				string name = item.BaseType ?? item.Name;
				if (previous == name)
					continue;
				item.Category = type;
				data.Add(item);
			}
		}
	}
}
