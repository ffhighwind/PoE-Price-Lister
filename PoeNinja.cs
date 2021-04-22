using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		private const string repoURL = @"https://raw.githubusercontent.com/ffhighwind/PoE-Price-Lister/master/";
		public const string FiltersUrl = repoURL + @"Resources/Filters/";
		//private const string jsonURL = @"http://poe.ninja/api/Data/Get{0}Overview?league={1}";
		private const string jsonURL = @"https://poe.ninja/api/data/{0}overview?league={1}&type={2}";
		//ALTERNATE API? https://poe.watch/prices?category=enchantment&league={1}

		private Dictionary<ItemKey, List<ItemData>> __Data { get; } = new Dictionary<ItemKey, List<ItemData>>();
		private Dictionary<ItemKey, IReadOnlyList<ItemData>> _Data { get; } = new Dictionary<ItemKey, IReadOnlyList<ItemData>>();

		public IReadOnlyDictionary<ItemKey, IReadOnlyList<ItemData>> Data => _Data;

		public bool Hardcore { get; private set; }
		public string League { get; private set; }

		public void Load(bool hardcore, string league) 
		{
			__Data.Clear();
			_Data.Clear();
			Hardcore = hardcore;
			League = league;
			Query("item", "DivinationCard");
			Query("item", "UniqueArmour");
			Query("item", "UniqueFlask");
			Query("item", "UniqueWeapon");
			Query("item", "UniqueAccessory");
			Query("item", "HelmetEnchant");

			//Query("currency", "Fragment");
			//Query("currency", "Currency");
			//Query("item", "Incubator");
			//Query("item", "Scarab");
			//Query("item", "Resonator");
			//Query("item", "Essence");
			//Query("item", "Prophecy");
			//Query("item", "SkillGem");
			//Query("item", "UniqueMap");
			//Query("item", "Map");
			//Query("item", "Beast");
			//Query("item", "UniqueJewel");
		}

		private void Query(string category, string type)
		{
			string url = string.Format(jsonURL, category, type, Hardcore ? ("Hardcore " + League) : League);
			string jsonURLString = Util.ReadWebPage(url, "application/json");
			if (jsonURLString.Length == 0)
				throw new InvalidOperationException("Failed to read the web URL: " + url);
			JObject jsonString = JObject.Parse(jsonURLString);
			JToken results = jsonString["lines"];
			foreach (JToken result in results) {
				ItemData jdata = result.ToObject<ItemData>();
				ItemKey key = new ItemKey(type, jdata.BaseType);
				if(!__Data.TryGetValue(key, out List<ItemData> list)) {
					list = new List<ItemData>();
					__Data.Add(key, list);
					_Data.Add(key, list);
				}
				list.Add(jdata);
			}
		}
	}
}
