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
using Newtonsoft.Json;

namespace PoE_Price_Lister
{
	[DelimitedRecord(",")]
	[IgnoreFirst(1)]
	[IgnoreEmptyLines]
	public class UniqueBaseTypeCsv
	{
		[FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
		public string Name { get; set; }

		public string Category { get; set; }
		public string Type { get; set; }
		public string Class { get; set; }
		public string BaseType { get; set; }

		[FieldTrim(TrimMode.Both)]
		[FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
		[FieldConverter(ConverterKind.Date, "M/d/yyyy")]
		public DateTime? Release { get; set; }

		public string Version { get; set; }
		public string League { get; set; }


		[FieldConverter(typeof(UniqueUsageConverter))]
		[FieldNullValue(UniqueUsage.None)]
		public UniqueUsage Usage { get; set; }

		[FieldConverter(typeof(BoolConverter))]
		[FieldNullValue(false)]
		public bool Unobtainable { get; set; }

		[FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
		public string Source { get; set; }
		[FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
		public string WikiLink { get; set; }
		[FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
		public string Notes { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented).ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			UniqueBaseTypeCsv other = (UniqueBaseTypeCsv) obj;
			return other.BaseType == BaseType && other.Name == Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() * BaseType.GetHashCode();
		}
	}

	internal class BoolConverter : ConverterBase
	{
		public override object StringToField(string from)
		{
			return from != null;
		}

		public override string FieldToString(object fieldValue)
		{
			return ((bool) fieldValue).ToString();
		}
	}

	internal class UniqueUsageConverter : ConverterBase
	{
		public override object StringToField(string from)
		{
			UniqueUsage output;
			if (from.Equals("Prophecy"))
				output = UniqueUsage.Prophecy;
			else if (from.Equals("Recipe"))
				output = UniqueUsage.Recipe;
			else if (from.Equals("Upgradable"))
				output = UniqueUsage.Upgradable;
			else if (from.Equals("Piece"))
				output = UniqueUsage.Piece;
			else if (from.Equals("Fractured"))
				output = UniqueUsage.Fractured;
			else if (string.IsNullOrWhiteSpace(from))
				output = UniqueUsage.None;
			else
				throw new InvalidOperationException(from);
			return output;
		}

		public override string FieldToString(object fieldValue)
		{
			return ((UniqueUsage) fieldValue).ToString();
		}
	}
}
