using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
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

		//private const string jsonURL = @"http://poe.ninja/api/Data/Get{0}Overview?league={1}";
		private const string baseURL = @"https://poe.ninja/api/data/";
		private const string jsonURL = @"https://poe.ninja/api/data/{0}overview?type={1}&league={2}";
		//ALTERNATE API? https://poe.watch/prices?category=enchantment&league={1}

		private Dictionary<ItemKey, IReadOnlyList<ItemData>> _Data { get; } = new Dictionary<ItemKey, IReadOnlyList<ItemData>>();
		public IReadOnlyDictionary<ItemKey, IReadOnlyList<ItemData>> Data { get; }

		public bool Hardcore { get; private set; }
		public string League { get; private set; }
		private readonly HttpClient httpClient = new HttpClient();

		public PoeNinja()
		{
			httpClient.BaseAddress = new Uri(baseURL);
			Data = _Data;
		}

		/// <summary>
		/// Loads data from POE Ninja synchronously.
		/// </summary>
		public void Load(bool hardcore, string league)
		{
			Exception ex = Task.Run(async () => await LoadAsync(hardcore, league)).Result;
			if (ex != null)
				throw ex;
		}

		/// <summary>
		/// Loads data from POE Ninja asynchronously.
		/// </summary>
		public async Task<Exception> LoadAsync(bool hardcore, string league)
		{
			_Data.Clear();
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
			catch (Exception ex) {
				return ex;
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
				if (jdata.ChaosValue == 0 || jdata.Count == 0) // || (jdata.BaseType == null && jdata.Name == null)
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
			Dictionary<string, List<ItemData>> map = new Dictionary<string, List<ItemData>>();
			foreach (ItemData item in items.OrderBy(x => x.BaseType ?? "").ThenBy(x => x.Name).ThenBy(x => x.Links)) {
				string name = item.BaseType ?? item.Name;				
				if (!map.TryGetValue(name, out List<ItemData> list)) {
					ItemKey key = new ItemKey(type, name);
					list = new List<ItemData>(1);
					map.Add(name, list);
					_Data.Add(key, list);
				}
				else if (list.Last().Name == item.Name)
					continue; // ordering ensures fewest links is added first, and the most recently added should be ignored if it's a duplicates
				list.Add(item);
			}
		}
	}
}
