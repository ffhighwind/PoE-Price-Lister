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

using FileHelpers;

namespace PoE_Price_Lister
{
	[DelimitedRecord(",")]
	[IgnoreFirst(1)]
	[IgnoreEmptyLines]
	public class CurrencyCsv
	{
		public string League { get; set; }
		public string Class { get; set; }
		[FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
		public string BaseType { get; set; }
		public int Stack { get; set; }
		public int? Tier { get; set; }
		public int? DropLevel { get; set; }
		[FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
		public string Description { get; set; }
	}
}
