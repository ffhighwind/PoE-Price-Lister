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

namespace PoE_Price_Lister
{
	public class PoeData
	{
		public PoeData() { }

		public IReadOnlyList<ConversionCsv> Conversions { get; private set; } = Array.Empty<ConversionCsv>();
		public IReadOnlyList<CurrencyCsv> CurrencyCsv { get; private set; } = Array.Empty<CurrencyCsv>();
		public IReadOnlyList<EnchantCsv> EnchantsCsv { get; private set; } = Array.Empty<EnchantCsv>();
		public IReadOnlyList<PriceCsv> Prices { get; private set; } = Array.Empty<PriceCsv>();
		public IReadOnlyList<UniqueCsv> UniquesCsv { get; private set; } = Array.Empty<UniqueCsv>();

		public IReadOnlyList<ItemData> Softcore { get; private set; } = Array.Empty<ItemData>();
		public IReadOnlyList<ItemData> Hardcore { get; private set; } = Array.Empty<ItemData>();

		public string League { get; private set; }

		public void Load(string league)
		{
			// Load CSV Data
			if (Conversions.Count == 0)
				Conversions = Util.ConvertCsv<ConversionCsv>(Util.ReadFile("poe_conversions.csv"));
			if (CurrencyCsv.Count == 0)
				CurrencyCsv = Util.ConvertCsv<CurrencyCsv>(Util.ReadFile("poe_currency.csv"));
			if (EnchantsCsv.Count == 0)
				EnchantsCsv = Util.ConvertCsv<EnchantCsv>(Util.ReadFile("poe_enchants.csv"));
			if (Prices.Count == 0)
				Prices = Util.ConvertCsv<PriceCsv>(Util.ReadFile("poe_prices.csv"));
			if (UniquesCsv.Count == 0)
				UniquesCsv = Util.ConvertCsv<UniqueCsv>(Util.ReadFile("poe_uniques.csv"));

			// Load JSON Data
			if (Softcore.Count == 0)
				Softcore = PoeNinja.GetData(false, league);
			if (Hardcore.Count == 0)
				Hardcore = PoeNinja.GetData(true, league);
		}
	}
}