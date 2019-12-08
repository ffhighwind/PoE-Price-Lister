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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PoE_Price_Lister
{
	public class FilterWriter
	{
		private const string uniqueWarning =
@"# Ordered most expensive first to prevent future name conflicts!
# Prices attained from poe.ninja.
# Future values will fluctuate based on league challenges and the meta.";

		private const string header15c =
@"#------#
# 15c+ #
#------#
# White background
# High value. Do not miss these.";

		private const string header5to15c =
@"#-------#
# 5-15c #
#-------#
# White border
# Usually worth selling. Not extremely rare or valuable.
# May share a BaseType with an extremely valuable item, League specific, or boss drop only.";

		private const string header3c =
@"#------#
# 3-5c #
#------#
# Yellow border
# Sellable but rarely worth much.
# May share a BaseType with an extremely valuable item, League specific, or boss drop only.";

		private const string headerLess3c =
@"#-----#
# <3c #
#-----#
# Orange border
# Usually < 3c or nearly worthless.
";

		private const string headerLimited =
@"# Limited drop. May share a BaseType with an extremely valuable item, League specific, or boss drop only.
";

		private const string lessLvl67 =
@"Show  # Uniques - <3c - ilvl <67
	Rarity = Unique
	ItemLevel < 67
	SetFontSize 40
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 180 90 45 # Unique (<3c)";

		private const string styleUniqueIconSound =
@"	PlayAlertSound 4 200 # Mid Value
	MinimapIcon 0 Brown Square
	PlayEffect Brown
";

		private const string styleUniqueIcon =
@"	MinimapIcon 0 Brown Square
	PlayEffect Brown
";

		private const string loreweaveStr =
@"# Loreweave (60x rings)
Show  # Uniques - <3c - Unique Rings
	Rarity = Unique
	Class Rings
	SetFontSize 40
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 180 90 45 # Unique (<3c)";

		private const string style15c =
@"	SetFontSize 45
	SetTextColor 255 128 64 # Unique (15c+)
	SetBackgroundColor 255 255 255 255 # Unique (15c+)
	SetBorderColor 255 128 64 # Unique (15c+)
	PlayAlertSound 1 200 # High Value
	MinimapIcon 0 Red Square
	PlayEffect Red";

		private const string style5to15c =
@"	SetFontSize 45
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 255 255 255 # Unique (5-15c)
	PlayAlertSound 1 200 # High Value
	MinimapIcon 0 Brown Square
	PlayEffect Brown";

		private const string style3c =
@"	SetFontSize 40
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 255 255 0 # Unique (3-5c)
	PlayAlertSound 4 200 # Mid Value
	MinimapIcon 0 Brown Square
	PlayEffect Brown";

		private const string styleLimited =
@"	SetFontSize 40
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 180 90 45 # Unique (<3c)
";

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

		private const string styleDiv2c =
@"	SetFontSize 45 
	SetTextColor 255 255 255 # Divination Card (2-10c) 
	SetBackgroundColor 255 0 175 255 # Divination Card (2-10c) 
	SetBorderColor 255 255 255 # Divination Card (2-10c) 
	PlayAlertSound 5 200 # Divination Card (2-10c) 
	MinimapIcon 0 Brown Triangle 
	PlayEffect Brown";

		private const string styleDivLess2c =
@"	SetFontSize 40 
	SetTextColor 0 0 0 # Divination Card (<2c) 
	SetBackgroundColor 255 0 175 230 # Divination Card (<2c) 
	SetBorderColor 150 30 100 # Divination Card (<2c) 
	PlayAlertSound 5 100 # Divination Card (<2c) 
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
	SetTextColor 255 255 255 # Divination Card (2c+) 
	SetBackgroundColor 255 0 175 255 # Divination Card (2c+) 
	SetBorderColor 0 255 0 # Error 
	PlayAlertSound 5 200 # Divination Card (2c+) 
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

		public void Create(FilterType type, string filterFile)
		{
			string filterData = File.Exists(filterFile) ? File.ReadAllText(filterFile)
					: Util.ReadWebPage(DataModel.FiltersUrl + filterFile, "text/plain");
			if (filterData.Length == 0) {
				filterData = Util.ReadWebPage(DataModel.FiltersUrl + filterFile, "text/plain");
			}

			string str;
			using (MemoryStream memoryStream = new MemoryStream())
			using (Writer = new StreamWriter(memoryStream, Encoding.UTF8)) {
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
				Writer.WriteLine();
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
				Writer.Flush();
				str = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			using (Writer = new StreamWriter(filterFile, false, Encoding.UTF8)) {
				Writer.Write(Regex.Replace(str, "(?<!\r)\n", "\r\n"));
			}
			Writer = null;
		}

		private string GenerateUniquesString(FilterType type)
		{
			List<string> list15c = new List<string>();
			List<string> list5to15c = new List<string>();
			List<string> list3to5c = new List<string>();
			List<string> listLimited = new List<string>();
			StringBuilder sb = new StringBuilder();

			foreach (var uniq in UniquesA) {
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
				//if is only core drop and SC <3c and HC 15c+ then Limited
				if (expectedVal.Tier == 0 && expectedValHC.Tier > 0) {
					if (expectedValHC.Tier > 2 || !entry.Items.All(x => x.IsCoreDrop))
						expectedVal = UniqueValue.Limited;
				}

				switch (expectedVal.Value) {
					case UniqueValueEnum.Chaos15:
						list15c.Add(outputBaseTy);
						break;
					case UniqueValueEnum.Chaos5to15:
						list5to15c.Add(outputBaseTy);
						break;
					case UniqueValueEnum.Chaos3to5:
						list3to5c.Add(outputBaseTy);
						break;
					case UniqueValueEnum.Limited:
						listLimited.Add(outputBaseTy);
						break;
					default:
						break;
				}
			}

			sb.AppendLine(uniqueWarning).AppendLine();
			if (list15c.Count > 0) {
				sb.AppendLine(header15c).AppendLine();
				sb.AppendLine("Show  # Uniques - 15c+").AppendLine("\tRarity = Unique").Append("\tBaseType ").AppendLine(ItemList(list15c)).AppendLine(style15c).AppendLine();
			}
			if (list5to15c.Count > 0) {
				sb.AppendLine(header5to15c).AppendLine();
				sb.AppendLine("Show  # Uniques - 5-15c").AppendLine("\tRarity = Unique").Append("\tBaseType ").AppendLine(ItemList(list5to15c)).AppendLine(style5to15c).AppendLine();
			}
			sb.AppendLine(header3c).AppendLine();
			if (list3to5c.Count > 0) {
				sb.AppendLine("Show  # Uniques - 3-5c").AppendLine("\tRarity = Unique").Append("\tBaseType ").AppendLine(ItemList(list3to5c)).AppendLine(style3c).AppendLine();
			}
			sb.AppendLine(loreweaveStr).AppendLine();
			sb.AppendLine(headerLess3c).AppendLine();

			string showHide = type == FilterType.VERY_STRICT ? "Hide" : "Show";
			string vsSound = type == FilterType.VERY_STRICT ? styleUniqueIconSound : styleUniqueIcon;
			string sSound = (type != FilterType.VERY_STRICT && type != FilterType.STRICT) ? styleUniqueIconSound : "";

			if (type != FilterType.VERY_STRICT) {
				sb.AppendLine(lessLvl67).AppendLine();
			}
			if (listLimited.Count > 0) {
				sb.AppendLine(headerLimited);
				sb.AppendLine("Show  # Uniques - Limited").AppendLine("\tRarity = Unique").Append("\tBaseType ").AppendLine(ItemList(listLimited)).AppendLine(styleLimited).Append(vsSound).AppendLine();
			}
			sb.AppendLine(showHide + uniqueNewOrWorthless).Append(sSound).AppendLine();

			return sb.ToString();
		}

		private string GenerateDivinationString(FilterType type)
		{
			List<string> list2to10cConflict = new List<string>();
			List<string> listLess2cConflict = new List<string>();
			List<string> listNearlyWorthlessConflict = new List<string>();
			List<string> listWorthlessConflict = new List<string>();

			List<string> list10c = new List<string>();
			List<string> list2to10c = new List<string>();
			List<string> listLess2c = new List<string>();
			List<string> listNearlyWorthless = new List<string>();
			List<string> listWorthless = new List<string>();
			StringBuilder sb = new StringBuilder();

			foreach (string divCard in DivinationCards) {
				DivinationCard data = DivinationA[divCard];
				DivinationValue expectedVal = data.ExpectedFilterValue;
				if (!data.HasHardCodedValue) {
					DivinationValue filterVal = data.FilterValue;
					DivinationCard dataB = DivinationB[divCard];
					if (data.SeverityLevel == 0)
						expectedVal = filterVal;
					//if SC < HC - 0.5 then +1 tier
					if (data.Tier < dataB.Tier && data.ChaosValue < dataB.ChaosValue - 0.5f) {
						expectedVal = DivinationValue.FromTier(expectedVal.Tier + 1);
					}
				}
				else if (data.ChaosValue - 0.5f > expectedVal.HighValue) {
					//MessageBox.Show(divCard + " is more valuable than expected");
					//throw new InvalidOperationException(divCard + " is more valuable than expected"); // card is somehow valuable?
				}

				switch (expectedVal.Value) {
					case DivinationValueEnum.Error:
					case DivinationValueEnum.Chaos10:
						list10c.Add(divCard);
						break;
					case DivinationValueEnum.Chaos2to10:
						if (list10c.Exists(str => divCard.Contains(str)))
							list2to10cConflict.Add(divCard);
						else
							list2to10c.Add(divCard);
						break;
					case DivinationValueEnum.ChaosLess2:
						if (list10c.Exists(str => divCard.Contains(str)) || list2to10c.Exists(str => divCard.Contains(str)))
							listLess2cConflict.Add(divCard);
						else
							listLess2c.Add(divCard);
						break;
					case DivinationValueEnum.NearlyWorthless:
						if (list10c.Exists(str => divCard.Contains(str)) || list2to10c.Exists(str => divCard.Contains(str)) || listLess2c.Exists(str => divCard.Contains(str)))
							listNearlyWorthlessConflict.Add(divCard);
						else
							listNearlyWorthless.Add(data.Name);
						break;
					case DivinationValueEnum.Worthless:
						if (list10c.Exists(str => divCard.Contains(str)) || list2to10c.Exists(str => divCard.Contains(str))
							|| listLess2c.Exists(str => divCard.Contains(str)) || listNearlyWorthless.Exists(str => divCard.Contains(str))) {
							listWorthlessConflict.Add(divCard);
						}
						else
							listWorthless.Add(divCard);
						break;
					default:
						throw new InvalidOperationException("Unknown Divination Value: " + expectedVal.Value);
				}
			}

			// Name Conflicts
			sb.AppendLine("# Potential Conflicts!!! (They have been separated but may need to be reorganized)").AppendLine();
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
			sb.AppendLine();

			if (list2to10cConflict.Count > 0)
				sb.AppendLine("Show  # Divination Cards - 2-10c (Conflicts)").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(list2to10cConflict)).AppendLine(styleDiv2c).AppendLine();
			if (listLess2cConflict.Count > 0) {
				string showHide = type == FilterType.VERY_STRICT ? "Hide" : "Show";
				sb.AppendLine(showHide + "  # Divination Cards - <2c (Conflicts)").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(listLess2cConflict)).AppendLine(styleDivLess2c).AppendLine();
			}
			if (listNearlyWorthlessConflict.Count > 0) {
				string showHide = (type == FilterType.STRICT || type == FilterType.VERY_STRICT) ? "Hide" : "Show";
				sb.AppendLine(showHide + "  # Divination Cards - Nearly Worthless (Conflicts)").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(listNearlyWorthlessConflict)).AppendLine(styleDivNearlyWorthless).AppendLine();
			}
			if (listWorthlessConflict.Count > 0) {
				string showHide = type == FilterType.LEVELING ? "Show" : "Hide";
				sb.AppendLine(showHide + "  # Divination Cards - Worthless (Conflicts)").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(listWorthlessConflict)).AppendLine(styleDivWorthless).AppendLine();
			}
			if (list10c.Count > 0)
				sb.AppendLine("Show  # Divination Cards - 10c+").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(list10c)).AppendLine(styleDiv10c).AppendLine();
			if (list2to10c.Count > 0)
				sb.AppendLine("Show  # Divination Cards - 2-10c").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(list2to10c)).AppendLine(styleDiv2c).AppendLine();
			if (listLess2c.Count > 0) {
				string showHide = type == FilterType.VERY_STRICT ? "Hide" : "Show";
				sb.AppendLine(showHide + "  # Divination Cards - <2c").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(listLess2c)).AppendLine(styleDivLess2c).AppendLine();
			}
			if (listNearlyWorthless.Count > 0) {
				string showHide = (type == FilterType.STRICT || type == FilterType.VERY_STRICT) ? "Hide" : "Show";
				sb.AppendLine(showHide + "  # Divination Cards - Nearly Worthless").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(listNearlyWorthless)).AppendLine(styleDivNearlyWorthless).AppendLine();
			}
			if (listWorthless.Count > 0) {
				string showHide = type == FilterType.LEVELING ? "Show" : "Hide";
				sb.AppendLine(showHide + "  # Divination Cards - Worthless").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(listWorthless)).AppendLine(styleDivWorthless).AppendLine();
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
				if (expectedVal.Tier < 1 && !dataB.IsLowConfidence && dataB.ChaosValue >= 50.0f) {
					//if SC <10c and HC 50c+ then 10+c
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
