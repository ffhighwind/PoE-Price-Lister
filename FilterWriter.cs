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

		private const string header3to10c =
@"#-------#
# 2-10c #
#-------#
# White border
# Usually worth selling. Not extremely rare or valuable.
# May share a BaseType with an extremely valuable item, League Specific, or boss drop only.";

		private const string header2c =
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
@"Show  # Uniques - <1c - ilvl <67
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

		private const string style3to10c =
@"	SetFontSize 45
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 255 255 255 # Unique (2-10c)
	PlayAlertSound 1 200 # High Value
	MinimapIcon 0 Brown Square
	PlayEffect Brown";

		private const string style2c =
@"	SetFontSize 40
	SetTextColor 255 128 64 # Unique
	SetBackgroundColor 50 25 12 # Unique
	SetBorderColor 255 255 0 # Unique (1-2c)
	PlayAlertSound 4 200 # Mid Value
	MinimapIcon 0 Brown Square
	PlayEffect Brown";

		private const string styleShared =
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
			}
			Writer = null;
		}

		private string GenerateUniquesString(FilterType type)
		{
			List<string> list10c = new List<string>();
			List<string> list3to10c = new List<string>();
			List<string> list2to3c = new List<string>();
			List<string> listShared = new List<string>();
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
				//if is only core drop and SC <2c and HC 7c+ then Shared
				if (expectedVal.Tier == 0 && expectedValHC.Tier > 0) {
					if (expectedValHC.Tier > 3 || !entry.Items.All(x => x.IsCoreDrop))
						expectedVal = UniqueValue.Shared;
				}

				switch (expectedVal.Value) {
					case UniqueValueEnum.Chaos10:
						list10c.Add(outputBaseTy);
						break;
					case UniqueValueEnum.Chaos3to10:
						list3to10c.Add(outputBaseTy);
						break;
					case UniqueValueEnum.Chaos2to3:
						list2to3c.Add(outputBaseTy);
						break;
					case UniqueValueEnum.Shared:
						listShared.Add(outputBaseTy);
						break;
					default:
						break;
				}
			}

			sb.AppendLine(uniqueWarning).AppendLine();
			if (list10c.Count > 0) {
				sb.AppendLine(header10c).AppendLine();
				sb.AppendLine("Show  # Uniques - 10c+").AppendLine("\tRarity = Unique").Append("\tBaseType ").AppendLine(ItemList(list10c)).AppendLine(style10c).AppendLine();
			}
			if (list3to10c.Count > 0) {
				sb.AppendLine(header3to10c).AppendLine();
				sb.AppendLine("Show  # Uniques - 3-10c").AppendLine("\tRarity = Unique").Append("\tBaseType ").AppendLine(ItemList(list3to10c)).AppendLine(style3to10c).AppendLine();
			}
			sb.AppendLine(header2c).AppendLine();
			if (list2to3c.Count > 0) {
				sb.AppendLine("Show  # Uniques - 2-3c").AppendLine("\tRarity = Unique").Append("\tBaseType ").AppendLine(ItemList(list2to3c)).AppendLine(style2c).AppendLine();
			}
			sb.AppendLine(loreweaveStr).AppendLine();
			sb.AppendLine(headerLess1c).AppendLine();

			string showHide = type == FilterType.VERY_STRICT ? "Hide" : "Show";
			string vsSound = type == FilterType.VERY_STRICT ? styleUniqueSound : "";
			string sSound = (type != FilterType.VERY_STRICT && type != FilterType.STRICT) ? styleUniqueSound : "";

			if (type != FilterType.VERY_STRICT) {
				sb.AppendLine(lessLvl67).AppendLine();
			}
			if (listShared.Count > 0) {
				sb.AppendLine("Show  # Uniques - Shared").AppendLine("\tRarity = Unique").Append("\tBaseType ").AppendLine(ItemList(listShared)).AppendLine(styleShared).Append(vsSound).AppendLine();
			}
			sb.AppendLine(showHide + uniqueNewOrWorthless).Append(sSound).AppendLine();

			return sb.ToString();
		}

		private readonly Dictionary<string, DivinationValue> DivinationCardsValueMap = new Dictionary<string, DivinationValue>()
		{
			// < 0.2c
			{ "Prosperity", DivinationValue.Worthless},
			{ "Struck by Lightning", DivinationValue.Worthless },
			{ "The Inoculated", DivinationValue.Worthless },
			{ "The Metalsmith's Gift", DivinationValue.Worthless },
			{ "The Surgeon", DivinationValue.Worthless },
			{ "Lantador's Lost Love", DivinationValue.Worthless },
			{ "The Carrion Crow", DivinationValue.Worthless },
			{ "The Lover", DivinationValue.Worthless },
			{ "The Rabid Rhoa", DivinationValue.Worthless },
			{ "The Warden", DivinationValue.Worthless },
			{ "The Gambler", DivinationValue.Worthless },
			{ "Turn the Other Cheek", DivinationValue.Worthless },
			{ "Thunderous Skies", DivinationValue.Worthless },

			// 0.2c+
			{ "Destined to Crumble", DivinationValue.NearlyWorthless },
			{ "The Lord in Black", DivinationValue.NearlyWorthless },
			{ "Rain of Chaos", DivinationValue.NearlyWorthless },
			{ "Her Mask", DivinationValue.NearlyWorthless },
			{ "Loyalty", DivinationValue.NearlyWorthless },
			{ "The Gemcutter", DivinationValue.NearlyWorthless },
			{ "The Scholar", DivinationValue.NearlyWorthless},
			{ "The Survivalist", DivinationValue.NearlyWorthless},
			{ "Three Voices", DivinationValue.NearlyWorthless},
			{ "Cartographer's Delight", DivinationValue.NearlyWorthless },
			{ "The Puzzle", DivinationValue.NearlyWorthless },
			{ "The Hermit", DivinationValue.NearlyWorthless },
			{ "Boon of Justice", DivinationValue.NearlyWorthless },
			{ "The Mountain", DivinationValue.NearlyWorthless },
			{ "Shard of Fate", DivinationValue.NearlyWorthless },
			{ "The Doppelganger", DivinationValue.NearlyWorthless },

			// 0.4c+
			{ "The Catalyst", DivinationValue.ChaosLess1 },
			{ "Boundless Realms", DivinationValue.ChaosLess1 },
			{ "Coveted Possession", DivinationValue.ChaosLess1 },
			{ "Emperor's Luck", DivinationValue.ChaosLess1 },
			{ "Three Faces in the Dark", DivinationValue.ChaosLess1 },
			{ "The Master Artisan", DivinationValue.ChaosLess1 },

			// 1.1c+
			{ "No Traces", DivinationValue.Chaos1to10 },
			{ "The Fool", DivinationValue.Chaos1to10 },
			{ "The Heroic Shot", DivinationValue.Chaos1to10 },
			{ "The Inventor", DivinationValue.Chaos1to10 },
			{ "The Wrath", DivinationValue.Chaos1to10 },
			{ "Lucky Connections", DivinationValue.Chaos1to10 },
			{ "The Innocent", DivinationValue.Chaos1to10 },
			{ "Vinia's Token", DivinationValue.Chaos1to10 },
			{ "The Cartographer", DivinationValue.Chaos1to10 },
			{ "Chaotic Disposition", DivinationValue.Chaos1to10 },
			{ "Demigod's Wager", DivinationValue.Chaos1to10 },

			// 10c+
			{ "Wealth and Power", DivinationValue.Chaos10 },
			{ "Alluring Bounty", DivinationValue.Chaos10 },
			{ "The Dragon's Heart",DivinationValue.Chaos10 },
			{ "House of Mirrors",DivinationValue.Chaos10 },
			{ "The Doctor", DivinationValue.Chaos10 },
			{ "The Demon", DivinationValue.Chaos10 },
			{ "The Fiend", DivinationValue.Chaos10 },
			{ "The Immortal", DivinationValue.Chaos10 },
			{ "The Nurse", DivinationValue.Chaos10 },
			{ "The Iron Bard", DivinationValue.Chaos10 },
			{ "Seven Years Bad Luck", DivinationValue.Chaos10 },
			{ "The Saint's Treasure", DivinationValue.Chaos10 },
			{ "The Eye of Terror", DivinationValue.Chaos10 },
		};


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
				if (!DivinationCardsValueMap.TryGetValue(divCard, out DivinationValue expectedVal)) {
					expectedVal = data.ExpectedFilterValue;
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
					throw new InvalidOperationException(divCard + " is more valuable than expected"); // card is somehow valuable?
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

			if (list1to10cConflict.Count > 0)
				sb.AppendLine("Show  # Divination Cards - 1c+ (Conflicts)").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(list1to10cConflict)).AppendLine(styleDiv1c).AppendLine();
			if (listLess1cConflict.Count > 0) {
				string showHide = type == FilterType.VERY_STRICT ? "Hide" : "Show";
				sb.AppendLine(showHide + "  # Divination Cards - <1c (Conflicts)").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(listLess1cConflict)).AppendLine(styleDivLess1c).AppendLine();
			}
			if (listNearlyWorthlessConflict.Count > 0) {
				string showHide = (type == FilterType.LEVELING || type == FilterType.MAPPING) ? "Show" : "Hide";
				sb.AppendLine(showHide + "  # Divination Cards - Nearly Worthless (Conflicts)").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(listNearlyWorthlessConflict)).AppendLine(styleDivNearlyWorthless).AppendLine();
			}
			if (listWorthlessConflict.Count > 0) {
				string showHide = type == FilterType.LEVELING ? "Show" : "Hide";
				sb.AppendLine(showHide + "  # Divination Cards - Worthless (Conflicts)").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(listWorthlessConflict)).AppendLine(styleDivWorthless).AppendLine();
			}
			if (list10c.Count > 0)
				sb.AppendLine("Show  # Divination Cards - 10c+").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(list10c)).AppendLine(styleDiv10c).AppendLine();
			if (list1to10c.Count > 0)
				sb.AppendLine("Show  # Divination Cards - 1c+").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(list1to10c)).AppendLine(styleDiv1c).AppendLine();
			if (listLess1c.Count > 0) {
				string showHide = type == FilterType.VERY_STRICT ? "Hide" : "Show";
				sb.AppendLine(showHide + "  # Divination Cards - <1c").AppendLine("\tClass Divination").Append("\tBaseType ").AppendLine(ItemList(listLess1c)).AppendLine(styleDivLess1c).AppendLine();
			}
			if (listNearlyWorthless.Count > 0) {
				string showHide = (type == FilterType.LEVELING || type == FilterType.MAPPING) ? "Show" : "Hide";
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
