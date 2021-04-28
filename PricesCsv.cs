using System;
using FileHelpers;

namespace PoE_Price_Lister
{
	[DelimitedRecord(",")]
	[IgnoreFirst(1)]
	[IgnoreEmptyLines]
	public class PricesCsv
	{
		public string Class { get; set; }
		public string Category { get; set; }
		[FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
		public string BaseType { get; set; }
		public int? Min { get; set; }
		public int? Max { get; set; }
		public int? Value { get; set; }
	}
}
