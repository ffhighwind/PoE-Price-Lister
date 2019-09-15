using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			Name = csvdata.Name;
			Description = csvdata.Description;
			Source = csvdata.Source;
			Gem = csvdata.Gem;
		}

		public void Load(JsonData jdata)
		{
			Description = jdata.Name;
			ChaosValue = jdata.ChaosValue;
			Count = jdata.Count;
			//if(jdata.Icon != null && jdata.Icon.StartsWith("https://web.poecdn.com/image/Art/2DItems/Gems/")) {
			//	Gem = jdata.Icon.Substring(46, jdata.Icon.IndexOf(".png") - 46);
			//}
		}

		public void ClearJson()
		{
			ChaosValue = -1.0f;
			Count = 0;
		}

		public string Name { get; private set; }
		public string Description { get; private set; }
		public string Gem { get; private set; }
		public int SeverityLevel {
			get {
				EnchantmentValue expect = ExpectedFilterValue;
				if (FilterValue == expect || (ChaosValue < 7.0f && expect.Value == EnchantmentValueEnum.Worthless))
					return 0;
				int severity = Math.Abs(FilterValue.Tier - expect.Tier);
				if (severity != 0) {
					if (expect != EnchantmentValue.Worthless && (ChaosValue < expect.LowValue || ChaosValue > expect.HighValue))
						severity++;
				}
				return severity;
			}
		}

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
				if (ChaosValue < 0.01f || IsLowConfidence)
					return FilterValue;
				if (FilterValue.LowValue <= ChaosValue && FilterValue.HighValue >= ChaosValue)
					return FilterValue;
				return EnchantmentValue.ValueOf(ChaosValue);
			}
		}
		public int Count { get; private set; }
		public float ChaosValue { get; private set; } = -1.0f;
	}
}
