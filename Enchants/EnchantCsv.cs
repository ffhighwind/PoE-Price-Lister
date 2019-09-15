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
