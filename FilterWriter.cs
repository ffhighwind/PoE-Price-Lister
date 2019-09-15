using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PoE_Price_Lister
{
	public class FilterWriter
	{
		private const string uniqueWarning =
@"# Ordered most expensive first to prevent future name conflicts!
# Prices attained from poe.ninja.
# Future values will fluctuate based on league challenges and the meta.";

		private const string header10c =
@"#------#
# 10c+ #
#------#
# White background
# High value. Do not miss these.";

		private const string header2to10c =
@"#-------#
# 2-10c #
#-------#
# White border
# Usually worth selling. Not extremely rare or valuable.
# May share a BaseType with an extremely valuable item, League Specific, or boss drop only.";

		private const string header1c =
@"#------#
# 1-2c #
#------#
# Yellow border
# Sellable but rarely worth much.";

		private const string headerLess1c =
@"#-----#
# <1c #
#-----#
# Orange border
# Usually < 1c or nearly worthless.
";

		private const string lessLvl67 =
@"  # Uniques - <1c - ilvl <67
	Rarity = Unique
	ItemLevel < 67
	SetFontSize 40
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 180 90 45 # Unique (<1c)";

		private const string styleUniqueSound =
@"	PlayAlertSound 4 200 # Mid Value
	MinimapIcon 0 Brown Square
	PlayEffect Brown
";

		private const string loreweaveStr =
@"# Loreweave (60x rings)
Show  # Uniques - 1-2c
	Rarity = Unique
	Class Rings
	SetFontSize 40
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 255 255 0 # Unique (1-2c)
	PlayAlertSound 4 200 # Mid Value
	MinimapIcon 0 Brown Square
	PlayEffect Brown";

		private const string style10c =
@"	SetFontSize 45
	SetTextColor 255 128 64 # Unique (10c+)
	SetBackgroundColor 255 255 255 255 # Unique (10c+)
	SetBorderColor 255 128 64 # Unique (10c+)
	PlayAlertSound 1 200 # High Value
	MinimapIcon 0 Red Square
	PlayEffect Red";

		private const string style2to10c =
@"	SetFontSize 45
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 255 255 255 # Unique (2-10c)
	PlayAlertSound 1 200 # High Value
	MinimapIcon 0 Brown Square
	PlayEffect Brown";

		private const string style1c =
@"	SetFontSize 40
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 255 255 0 # Unique (1-2c)
	PlayAlertSound 4 200 # Mid Value
	MinimapIcon 0 Brown Square
	PlayEffect Brown";

		private const string styleLess1c =
@"	SetFontSize 40
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 180 90 45 # Unique (<1c)";

		private const string uniqueNewOrWorthless =
@"  # Uniques - New or Worthless
	Rarity = Unique
	SetFontSize 36
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 180 90 45 # Unique (<1c)";

		private const string styleDiv10c =
@"	SetFontSize 45 
	SetTextColor 255 0 175 # Divination Card (10c+) 
	SetBackgroundColor 255 255 255 255 # Divination Card (10c+) 
	SetBorderColor 255 0 175 # Divination Card (10c+) 
	PlayAlertSound 1 200 # High Value 
	MinimapIcon 0 Red Triangle 
	PlayEffect Red";

		private const string styleDiv1c =
@"	SetFontSize 45 
	SetTextColor 255 255 255 # Divination Card (1c+) 
	SetBackgroundColor 255 0 175 255 # Divination Card (1c+) 
	SetBorderColor 255 255 255 # Divination Card (1c+) 
	PlayAlertSound 5 200 # Divination Card (1c+) 
	MinimapIcon 0 Brown Triangle 
	PlayEffect Brown";

		private const string styleDivLess1c =
@"	SetFontSize 40 
	SetTextColor 0 0 0 # Divination Card (<1c) 
	SetBackgroundColor 255 0 175 230 # Divination Card (<1c) 
	SetBorderColor 150 30 100 # Divination Card (<1c) 
	PlayAlertSound 5 100 # Divination Card (Low) 
	PlayEffect Brown";

		private const string styleDivNearlyWorthless =
@"	SetFontSize 36 
	SetTextColor 0 0 0 # Divination Card (Nearly Worthless) 
	SetBackgroundColor 255 0 175 170 # Divination Card (Nearly Worthless) 
	SetBorderColor 0 0 0 # Divination Card (Nearly Worthless) 
	PlayAlertSound 5 0 # Divination Card (Nearly Worthless)";

		private const string styleDivWorthless =
@"	SetFontSize 32 
	SetTextColor 0 0 0 # Divination Card (Worthless) 
	SetBackgroundColor 255 0 175 120 # Divination Card (Worthless) 
	SetBorderColor 255 0 175 50 # Divination Card (Worthless) 
	DisableDropSound";

		private const string divNewOrWorthless =
@"Show  # Divination Cards - New (Error) 
	Class Divination 
	SetFontSize 40 
	SetTextColor 255 255 255 # Divination Card (1c+) 
	SetBackgroundColor 255 0 175 255 # Divination Card (1c+) 
	SetBorderColor 0 255 0 # Error 
	PlayAlertSound 5 200 # Divination Card (1c+) 
	MinimapIcon 0 White Triangle 
	PlayEffect White";

		private readonly DataModel Model;
		private readonly IReadOnlyList<string> Uniques;
		private readonly IReadOnlyList<string> DivinationCards;
		private readonly IReadOnlyList<string> Enchantments;
		private readonly IReadOnlyDictionary<string, UniqueBaseType> UniquesA;
		private readonly IReadOnlyDictionary<string, UniqueBaseType> UniquesB;
		private readonly IReadOnlyDictionary<string, DivinationCard> DivinationA;
		private readonly IReadOnlyDictionary<string, DivinationCard> DivinationB;
		private readonly IReadOnlyDictionary<string, Enchantment> EnchantsA;
		private readonly IReadOnlyDictionary<string, Enchantment> EnchantsB;
		private readonly IReadOnlyList<IReadOnlyList<string>> Conflicts;

		private StreamWriter Writer { get; set; }

		private bool Safe { get; set; }

		public FilterWriter(DataModel model, LeagueData l1, LeagueData l2)
		{
			Model = model;
			DivinationCards = model.DivinationCards;
			Uniques = model.Uniques;
			Enchantments = model.Enchantments;
			UniquesA = l1.Uniques;
			UniquesB = l2.Uniques;
			DivinationA = l1.DivinationCards;
			DivinationB = l2.DivinationCards;
			EnchantsA = l1.Enchantments;
			EnchantsB = l2.Enchantments;
			Conflicts = model.DivinationCardNameConflicts;
		}

		public void Create(FilterType type, string filterFile, bool safe)
		{
			Safe = safe;
			string filterData = File.Exists(filterFile) ? File.ReadAllText(filterFile)
					: Util.ReadWebPage(DataModel.FiltersUrl + filterFile, "text/plain");
			if (filterData.Length == 0) {
				filterData = Util.ReadWebPage(DataModel.FiltersUrl + filterFile, "text/plain");
			}
			using (Writer = new StreamWriter(filterFile, false, Encoding.UTF8)) {
				string hashes = @"############################################";
				int headerEnd = filterData.IndexOf("#-----------");
				int enchantsStart = filterData.IndexOf("# Section: Enchantments", headerEnd);
				int enchantsEnd = filterData.IndexOf(@"#########", enchantsStart);
				int uniquesStart = filterData.IndexOf("# Section: Uniques");
				int uniquesEnd = filterData.IndexOf(@"#########", uniquesStart);
				int divStart = filterData.IndexOf("# Section: Divination Cards", uniquesEnd);
				int divEnd = filterData.IndexOf(@"#########", divStart);

				int headerVersionStart = filterData.IndexOf("##  ");
				int headerVersionEnd = filterData.IndexOf(' ', filterData.IndexOf('.') - 4) + 1;
				string versStart = filterData.Substring(headerVersionStart, headerVersionEnd - headerVersionStart);
				string nextVers = Model.VersionMajor + "." + Model.VersionMinor + "." + (Model.VersionRelease + 1);
				// Header
				Writer.WriteLine(hashes);
				Writer.WriteLine("{0}{1}{2} ##", filterData.Substring(headerVersionStart, headerVersionEnd - headerVersionStart),
					nextVers, new string(' ', hashes.Length - 3 - versStart.Length - nextVers.Length));
				Writer.WriteLine(hashes);
				string dateStr = DateTime.Today.ToString("MMMM d, yyyy");
				Writer.WriteLine("## Release Date: {0}{1} ##", dateStr, new string(' ', hashes.Length - dateStr.Length - 17 - 3));
				Writer.WriteLine(hashes);
				Writer.WriteLine();
				// Enchantments
				Writer.Write(filterData.Substring(headerEnd, enchantsStart - headerEnd));
				Writer.WriteLine("# Section: Enchantments");
				Writer.WriteLine();
				Writer.WriteLine(GenerateEnchantsString());
				// Enchants -> Uniques
				Writer.Write(filterData.Substring(enchantsEnd, uniquesStart - enchantsEnd));
				// Uniques
				Writer.WriteLine(@"# Section: Uniques");
				Writer.WriteLine(GenerateUniquesString(type));
				// Divination Cards
				Writer.WriteLine(
@"##########################################
############ DIVINATION CARDS ############
##########################################
# Section: Divination Cards

# Ordered most expensive first to prevent future name conflicts!
# Prices attained from poe.ninja.
# Future values will fluctuate based on league challenges and the meta.");
				Writer.Write(GenerateDivinationString(type));
				Writer.WriteLine();
				Writer.Write(filterData.Substring(divEnd));
			}
			Writer = null;
		}

		private string GenerateUniquesString(FilterType type)
		{
			List<string> list10c = new List<string>();
			List<string> list2to10c = new List<string>();
			List<string> list1to2c = new List<string>();
			List<string> listLess1c = new List<string>();
			List<string> listLess1cShared = new List<string>();
			List<string> listLess1cLeague = new List<string>();
			List<string> listLess1cBoss = new List<string>();
			List<string> listLess1cCrafted = new List<string>();
			List<string> listLess1cLabyrinth = new List<string>();
			StringBuilder sb = new StringBuilder();

			foreach (KeyValuePair<string, UniqueBaseType> uniq in UniquesA) {
				UniqueBaseType entry = uniq.Value;
				string baseTy = uniq.Key;
				UniqueValue expectedVal = entry.ExpectedFilterValue;
				UniqueValue filterVal = entry.FilterValue;
				string outputBaseTy = baseTy;
				int index = baseTy.IndexOf('ö');
				if (index > 0) {
					outputBaseTy = outputBaseTy.Substring(0, index);
				}
				UniqueBaseType entryHC = UniquesB[baseTy];
				UniqueValue expectedValHC = entryHC.ExpectedFilterValue;

				if (entry.SeverityLevel == 0)
					expectedVal = filterVal;
				else if (Safe && filterVal.Tier > expectedVal.Tier) {
					if (entry.Items.Count > 1 || filterVal.LowValue < entry.Items[0].ChaosValue) {
						expectedVal = UniqueValue.FromTier(expectedVal.Tier + 1);
					}
				}
				//if SC <1c and HC 4c+ then 1-2c
				if (expectedVal.Tier < 2 && expectedValHC.Tier > 2 && entryHC.Items.Any(i => i.IsCoreDrop && i.ChaosValue > 4.0f)) {
					expectedVal = UniqueValue.Chaos1to2;
				}

				switch (expectedVal.Value) {
					case UniqueValueEnum.Chaos10:
						list10c.Add(outputBaseTy);
						break;
					case UniqueValueEnum.Chaos2to10:
						list2to10c.Add(outputBaseTy);
						break;
					case UniqueValueEnum.Chaos1to2:
						list1to2c.Add(outputBaseTy);
						break;
					case UniqueValueEnum.ChaosLess1:
						listLess1c.Add(outputBaseTy);
						break;
					case UniqueValueEnum.ChaosLess1League:
						listLess1cLeague.Add(outputBaseTy);
						break;
					case UniqueValueEnum.ChaosLess1Boss:
						listLess1cBoss.Add(outputBaseTy);
						break;
					case UniqueValueEnum.ChaosLess1Shared:
						listLess1cShared.Add(outputBaseTy);
						break;
					case UniqueValueEnum.ChaosLess1Crafted:
						listLess1cCrafted.Add(outputBaseTy);
						break;
					case UniqueValueEnum.ChaosLess1Labyrinth:
						listLess1cLabyrinth.Add(outputBaseTy);
						break;
					default:
						break;
				}
			}

			sb.AppendLine(uniqueWarning).AppendLine();
			if (list10c.Count > 0) {
				sb.AppendLine(header10c).AppendLine();
				sb.AppendLine("Show  # Uniques - 10c+").AppendLine("\tRarity = Unique").Append("\t BaseType ").AppendLine(ItemList(list10c)).AppendLine(style10c).AppendLine();
			}
			if (list2to10c.Count > 0) {
				sb.AppendLine(header2to10c).AppendLine();
				sb.AppendLine("Show  # Uniques - 2-10c").AppendLine("\tRarity = Unique").Append("\t BaseType ").AppendLine(ItemList(list2to10c)).AppendLine(style2to10c).AppendLine();
			}
			sb.AppendLine(header1c).AppendLine();
			if (list1to2c.Count > 0) {
				sb.AppendLine("Show  # Uniques - 1-2c").AppendLine("\tRarity = Unique").Append("\t BaseType ").AppendLine(ItemList(list1to2c)).AppendLine(style1c).AppendLine();
			}
			sb.AppendLine(loreweaveStr).AppendLine();
			sb.AppendLine(headerLess1c).AppendLine();

			string showHide = type == FilterType.VERY_STRICT ? "Hide" : "Show";
			string vsSound = type == FilterType.VERY_STRICT ? styleUniqueSound : "";
			string sSound = (type == FilterType.LEVELING || type == FilterType.MAPPING) ? styleUniqueSound : "";
			sb.AppendLine(showHide + lessLvl67).AppendLine();
			if (listLess1cLeague.Count > 0) {
				sb.AppendLine("Show  # Uniques - <1c - League").AppendLine("\tRarity = Unique").Append("\t BaseType ").AppendLine(ItemList(listLess1cLeague)).AppendLine(styleLess1c).Append(sSound).AppendLine();
			}
			if (listLess1cBoss.Count > 0) {
				sb.AppendLine("Show  # Uniques - <1c - Boss Prophecy").AppendLine("\tRarity = Unique").Append("\t BaseType ").AppendLine(ItemList(listLess1cBoss)).AppendLine(styleLess1c).Append(sSound).AppendLine();
			}
			if (listLess1cShared.Count > 0) {
				sb.AppendLine("Show  # Uniques - <1c - Shared").AppendLine("\tRarity = Unique").Append("\t BaseType ").AppendLine(ItemList(listLess1cShared)).AppendLine(styleLess1c).Append(vsSound).AppendLine();
			}
			if (listLess1cCrafted.Count > 0) {
				sb.AppendLine("Show  # Uniques - <1c - Crafted Fated Purchased").AppendLine("\tRarity = Unique").Append("\t BaseType ").AppendLine(ItemList(listLess1cCrafted)).AppendLine(styleLess1c).Append(sSound).AppendLine();
			}
			if (listLess1cLabyrinth.Count > 0) {
				sb.AppendLine(showHide + "  # Uniques - <1c - Labyrinth").AppendLine("\tRarity = Unique").Append("\t BaseType ").AppendLine(ItemList(listLess1cLabyrinth)).AppendLine(styleLess1c).Append(sSound).AppendLine();
			}
			if (listLess1c.Count > 0) {
				sb.AppendLine(showHide + "  # Uniques - <1c - Nearly Worthless").AppendLine("\tRarity = Unique").Append("\t BaseType ").AppendLine(ItemList(listLess1c)).AppendLine(styleLess1c).Append(sSound).AppendLine();
			}
			sb.AppendLine(showHide + uniqueNewOrWorthless).Append(sSound).AppendLine();

			return sb.ToString();
		}


		private string GenerateDivinationString(FilterType type)
		{
			List<string> list1to10cConflict = new List<string>();
			List<string> listLess1cConflict = new List<string>();
			List<string> listNearlyWorthlessConflict = new List<string>();
			List<string> listWorthlessConflict = new List<string>();

			List<string> list10c = new List<string>();
			List<string> list1to10c = new List<string>();
			List<string> listLess1c = new List<string>();
			List<string> listNearlyWorthless = new List<string>();
			List<string> listWorthless = new List<string>();
			StringBuilder sb = new StringBuilder();

			foreach (string divCard in DivinationCards) {
				DivinationCard data = DivinationA[divCard];
				DivinationValue expectedVal = data.ExpectedFilterValue;
				DivinationValue filterVal = data.FilterValue;
				DivinationCard dataB = DivinationB[divCard];
				if (data.SeverityLevel == 0)
					expectedVal = filterVal;
				else if (Safe && filterVal.Tier > expectedVal.Tier && filterVal.LowValue < data.ChaosValue) {
					expectedVal = DivinationValue.FromTier(expectedVal.Tier + 1);
				}
				//if SC <1c and HC 8c+ then +1 tier
				if (expectedVal.Tier < 2 && dataB.ChaosValue > 8.0f) {
					expectedVal = DivinationValue.FromTier(expectedVal.Tier + 1);
				}

				switch (expectedVal.Value) {
					case DivinationValueEnum.Chaos10:
						list10c.Add(divCard);
						break;
					case DivinationValueEnum.Chaos1to10:
						if (list10c.Exists(str => divCard.Contains(str)))
							list1to10cConflict.Add(divCard);
						else
							list1to10c.Add(divCard);
						break;
					case DivinationValueEnum.ChaosLess1:
						if (list10c.Exists(str => divCard.Contains(str)) || list1to10c.Exists(str => divCard.Contains(str)))
							listLess1cConflict.Add(divCard);
						else
							listLess1c.Add(divCard);
						break;
					case DivinationValueEnum.NearlyWorthless:
						if (list10c.Exists(str => divCard.Contains(str)) || list1to10c.Exists(str => divCard.Contains(str)) || listLess1c.Exists(str => divCard.Contains(str)))
							listNearlyWorthlessConflict.Add(divCard);
						else
							listNearlyWorthless.Add(data.Name);
						break;
					case DivinationValueEnum.Worthless:
						if (list10c.Exists(str => divCard.Contains(str)) || list1to10c.Exists(str => divCard.Contains(str))
							|| listLess1c.Exists(str => divCard.Contains(str)) || listNearlyWorthless.Exists(str => divCard.Contains(str))) {
							listWorthlessConflict.Add(divCard);
						}
						else
							listWorthless.Add(divCard);
						break;
					default:
						break;
				}
			}

			// Name Conflicts
			sb.AppendLine("# Potential Conflicts!!! (They have been separated but may need to be reorganized)");
			foreach (List<string> list in Conflicts) {
				sb.Append("# ");
				foreach (string str in list) {
					string baseTy = str;
					if (baseTy.Contains(' '))
						baseTy = "\"" + baseTy + "\"";
					sb.Append(baseTy).Append(' ');
				}
				sb.Remove(sb.Length - 1, 1).AppendLine();
			}

			if (list1to10cConflict.Count > 0)
				sb.AppendLine("Show  # Divination Cards - 1c+ (Conflicts)").AppendLine("\tClass Divination").Append("\t BaseType ").AppendLine(ItemList(list1to10cConflict)).AppendLine(styleDiv1c).AppendLine();
			if (listLess1cConflict.Count > 0) {
				string showHide = type == FilterType.VERY_STRICT ? "Hide" : "Show";
				sb.AppendLine(showHide + "  # Divination Cards - <1c (Conflicts)").AppendLine("\tClass Divination").Append("\t BaseType ").AppendLine(ItemList(listLess1cConflict)).AppendLine(styleDivLess1c).AppendLine();
			}
			if (listNearlyWorthlessConflict.Count > 0) {
				string showHide = (type == FilterType.LEVELING || type == FilterType.MAPPING) ? "Show" : "Hide";
				sb.AppendLine(showHide + "  # Divination Cards - Nearly Worthless (Conflicts)").AppendLine("\tClass Divination").Append("\t BaseType ").AppendLine(ItemList(listNearlyWorthlessConflict)).AppendLine(styleDivNearlyWorthless).AppendLine();
			}
			if (listWorthlessConflict.Count > 0) {
				string showHide = type == FilterType.LEVELING ? "Show" : "Hide";
				sb.AppendLine(showHide + "  # Divination Cards - Worthless (Conflicts)").AppendLine("\tClass Divination").Append("\t BaseType ").AppendLine(ItemList(listWorthlessConflict)).AppendLine(styleDivWorthless).AppendLine();
			}
			if (list10c.Count > 0)
				sb.AppendLine("Show  # Divination Cards - 10c+").AppendLine("\tClass Divination").Append("\t BaseType ").AppendLine(ItemList(list10c)).AppendLine(styleDiv10c).AppendLine();
			if (list1to10c.Count > 0)
				sb.AppendLine("Show  # Divination Cards - 1c+").AppendLine("\tClass Divination").Append("\t BaseType ").AppendLine(ItemList(list1to10c)).AppendLine(styleDiv1c).AppendLine();
			if (listLess1c.Count > 0) {
				string showHide = type == FilterType.VERY_STRICT ? "Hide" : "Show";
				sb.AppendLine(showHide + "  # Divination Cards - <1c").AppendLine("\tClass Divination").Append("\t BaseType ").AppendLine(ItemList(listLess1c)).AppendLine(styleDivLess1c).AppendLine();
			}
			if (listNearlyWorthless.Count > 0) {
				string showHide = (type == FilterType.LEVELING || type == FilterType.MAPPING) ? "Show" : "Hide";
				sb.AppendLine(showHide + "  # Divination Cards - Nearly Worthless").AppendLine("\tClass Divination").Append("\t BaseType ").AppendLine(ItemList(listNearlyWorthless)).AppendLine(styleDivNearlyWorthless).AppendLine();
			}
			if (listWorthless.Count > 0) {
				string showHide = type == FilterType.LEVELING ? "Show" : "Hide";
				sb.AppendLine(showHide + "  # Divination Cards - Worthless").AppendLine("\tClass Divination").Append("\t BaseType ").AppendLine(ItemList(listWorthless)).AppendLine(styleDivWorthless).AppendLine();
			}
			sb.AppendLine(divNewOrWorthless);

			return sb.ToString();
		}

		private string GenerateEnchantsString()
		{
			StringBuilder sb = new StringBuilder();

			List<string> list20c = new List<string>();
			List<string> list10c = new List<string>();
			foreach (string name in Enchantments) {
				Enchantment data = EnchantsA[name];
				if (data.Source == EnchantmentSource.BlightOils)
					continue;
				Enchantment dataB = EnchantsB[name];
				EnchantmentValue expectedVal = data.ExpectedFilterValue;
				EnchantmentValue filterVal = data.ExpectedFilterValue;

				if (data.SeverityLevel == 0)
					expectedVal = filterVal;
				else if (Safe && filterVal.Tier > expectedVal.Tier && filterVal.LowValue < data.ChaosValue) {
					expectedVal = EnchantmentValue.FromTier(expectedVal.Tier + 1);
				}
				if (expectedVal.Tier < 1 && !dataB.IsLowConfidence && dataB.ChaosValue >= 40.0f) {
					//if SC <10c and HC 40c+ then 10+c
					expectedVal = EnchantmentValue.Chaos10;
				}
				switch (expectedVal.Value) {
					case EnchantmentValueEnum.Chaos10:
						list10c.Add(data.Name);
						break;
					case EnchantmentValueEnum.Chaos20:
						list20c.Add(data.Name);
						break;
					case EnchantmentValueEnum.Worthless:
					case EnchantmentValueEnum.Error:
						break;
				}
			}

			string enchStyle1 =
@"	SetFontSize 45
	SetTextColor 255 255 255 # Crafting Base (High)
	SetBackgroundColor 75 75 75 255 # Crafting Base (High)
	SetBorderColor 0 255 255 # Crafting Base (High)
	PlayAlertSound 1 200 # High Value
	MinimapIcon 0 Red Square
	PlayEffect Red";

			string enchStyle2 =
@"	SetFontSize 40
	SetBackgroundColor 40 40 40 # Crafting Base (Explicit)
	SetBorderColor 25 65 175 # Crafting Base (Explicit)
	PlayAlertSound 4 200 # Mid Value
	MinimapIcon 0 Blue Square
	PlayEffect Blue";

			if (list20c.Count > 0) {
				sb.AppendLine(@"Show  # Enchantments - 20c+").Append("\tHasEnchantment ").AppendLine(ItemList(list20c)).AppendLine(enchStyle1).AppendLine();
			}
			if (list10c.Count > 0) {
				sb.AppendLine(@"Show  # Enchantments - 10c+").Append("\tHasEnchantment ").AppendLine(ItemList(list10c)).AppendLine(enchStyle2).AppendLine();
			}
			sb.AppendLine(
@"Show  # Enchantments - Other
	AnyEnchantment True
	SetFontSize 36
	SetBackgroundColor 40 40 40 # Crafting Base (Explicit)
	SetBorderColor 25 65 175 # Crafting Base (Explicit)");
			return sb.ToString();
		}

		private string ItemList(List<string> items)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string item in items) {
				string result = item;
				if (item.Contains(' '))
					result = "\"" + item + "\"";
				sb.Append(result).Append(' ');
			}
			return sb.Remove(sb.Length - 1, 1).ToString();
		}
	}
}
