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
using FileHelpers;

namespace PoE_Price_Lister
{
	[DelimitedRecord(",")]
	[IgnoreFirst(1)]
	public class EnchantCsv
	{
		[FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.AllowForRead)]
		public string Name { get; set; }

		[FieldConverter(typeof(DifficultyConverter))]
		public EnchantmentSource Source { get; set; }

		[FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.AllowForRead)]
		public string Description { get; set; }

		public string Gem { get; set; }

		internal class DifficultyConverter : ConverterBase
		{
			public override object StringToField(string from)
			{
				if (from.Equals("Merciless Labyrinth"))
					return EnchantmentSource.MercilessLab;
				if (from.Equals("Eternal Labyrinth"))
					return EnchantmentSource.EternalLab;
				if (from.Equals("Blight Oils"))
					return EnchantmentSource.BlightOils;
				if (from.Equals("Cruel Labyrinth"))
					return EnchantmentSource.CruelLab;
				if (from.Equals("Normal Labyrinth"))
					return EnchantmentSource.NormalLab;
				throw new InvalidOperationException(from);
			}

			public override string FieldToString(object fieldValue)
			{
				return ((EnchantmentSource) fieldValue).ToString();
			}
		}
	}
}
