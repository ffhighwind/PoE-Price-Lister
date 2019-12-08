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

namespace PoE_Price_Lister
{
	public class LeagueData
	{
		public LeagueData(bool hardcore)
		{
			IsHardcore = hardcore;
		}

		public bool IsHardcore { get; private set; }
		public readonly Dictionary<string, DivinationCard> DivinationCards = new Dictionary<string, DivinationCard>();
		public readonly Dictionary<string, UniqueBaseType> Uniques = new Dictionary<string, UniqueBaseType>();
		public readonly Dictionary<string, Enchantment> Enchantments = new Dictionary<string, Enchantment>();
		public readonly Dictionary<string, Enchantment> EnchantmentsDescriptions = new Dictionary<string, Enchantment>();

		public void ClearFilterValues()
		{
			foreach (DivinationCard div in DivinationCards.Values) {
				div.FilterValue = DivinationValue.Error;
			}
			foreach (UniqueBaseType uniq in Uniques.Values) {
				uniq.FilterValue = UniqueValue.Worthless;
			}
			foreach (Enchantment ench in Enchantments.Values) {
				ench.FilterValue = EnchantmentValue.Worthless;
			}
		}
	}
}
