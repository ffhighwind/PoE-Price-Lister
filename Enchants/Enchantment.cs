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
using System.Linq;

namespace PoE_Price_Lister
{
	public class Enchantment
	{
		public Enchantment(string name)
		{
			Name = name;
		}

		public void Load(EnchantCsv csvdata)
		{
			Name = csvdata.Enchantment;
			Description = csvdata.Description;
			Source = csvdata.Source;
			Gem = csvdata.Gem;
		}

		public void Load(ItemData jdata)
		{
			Description = jdata.Name;
			ChaosValue = jdata.ChaosValue;
			Count = jdata.Count;
			//if(jdata.Icon != null && jdata.Icon.StartsWith("https://web.poecdn.com/image/Art/2DItems/Gems/")) {
			//	Gem = jdata.Icon.Substring(46, jdata.Icon.IndexOf(".png") - 46);
			//}
		}

		public string Name { get; private set; }
		public string Description { get; private set; }
		public string Gem { get; private set; }
		public int SeverityLevel => Math.Abs(FilterValue.Tier - ExpectedFilterValue.Tier);

		public string QuotedName {
			get {
				if (Name.Contains(' '))
					return "\"" + Name + "\"";
				return Name;
			}
		}

		public bool IsLowConfidence => Count < 3;

		public EnchantmentSource Source { get; private set; }
		public EnchantmentValue FilterValue { get; set; } = EnchantmentValue.Worthless;
		public EnchantmentValue ExpectedFilterValue {
			get {
				if (ChaosValue < 0.01f || IsLowConfidence || (ChaosValue >= FilterValue.LowValue && ChaosValue <= FilterValue.HighValue))
					return FilterValue;
				return EnchantmentValue.ValueOf(ChaosValue);
			}
		}
		public int Count { get; private set; }
		public float ChaosValue { get; private set; } = -1.0f;
	}
}
