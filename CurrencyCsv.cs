using System;
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
