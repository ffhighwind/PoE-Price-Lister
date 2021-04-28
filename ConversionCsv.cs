using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileHelpers;

namespace PoE_Price_Lister
{
	[DelimitedRecord(",")]
	[IgnoreFirst(1)]
	[IgnoreEmptyLines]
	public class ConversionCsv
	{
		public string InputClass { get; set; }
		[FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
		public string InputBaseType { get; set; }
		public int InputQuantity { get; set; }
		public string OutputClass { get; set; }
		[FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
		public string OutputBaseType { get; set; }
		public int OutputQuantity { get; set; }
		public string Type { get; set; }
	}
}
