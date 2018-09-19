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
            "#-----#\n" +
            "# <1c #\n" +
            "#-----#\n" +
            "# Orange border\n" +
            "# Usually < 1c or nearly worthless.\n\n" +

            "Show  # Uniques - <1c - ilvl <67\n" +
            "\tRarity = Unique\n" +
            "\tItemLevel < 67\n" +
            "\tSetFontSize 40\n" +
            "\tSetTextColor 255 128 64 # Unique\n" +
            "\tSetBackgroundColor 50 25 12 # Unique\n" +
            "\tSetBorderColor 180 90 45 # Unique (<1c)\n" +
            "\tPlayAlertSound 4 200 # Mid Value\n" +
            "\tMinimapIcon 0 Brown Star\n" +
            "\tPlayEffect Brown";

        private const string loreweaveStr =
            "# Loreweave (60x rings)\n" +
            "Show  # Uniques - 1-2c\n" +
            "\tRarity = Unique\n" +
            "\tClass Rings\n" +
            "\tSetFontSize 40\n" +
            "\tSetTextColor 255 128 64 # Unique\n" +
            "\tSetBackgroundColor 50 25 12 # Unique\n" +
            "\tSetBorderColor 255 255 0 # Unique (1-2c)\n" +
            "\tPlayAlertSound 4 200 # Mid Value\n" +
            "\tMinimapIcon 0 Brown Star\n" +
            "\tPlayEffect Brown";

        private const string style10c =
            "\tSetFontSize 45\n" +
            "\tSetTextColor 255 128 64 # Unique (10c+)\n" +
            "\tSetBackgroundColor 255 255 255 255 # Unique (10c+)\n" +
            "\tSetBorderColor 255 128 64 # Unique (10c+)\n" +
            "\tPlayAlertSound 1 200 # High Value\n" +
            "\tMinimapIcon 0 Red Star\n" +
            "\tPlayEffect Red";

        private const string style2to10c =
            "\tSetFontSize 45\n" +
            "\tSetTextColor 255 128 64 # Unique\n" +
            "\tSetBackgroundColor 50 25 12 # Unique\n" +
            "\tSetBorderColor 255 255 255 # Unique (2-10c)\n" +
            "\tPlayAlertSound 1 200 # High Value\n" +
            "\tMinimapIcon 0 Brown Star\n" +
            "\tPlayEffect Brown";

        private const string style1c =
            "\tSetFontSize 40\n" +
            "\tSetTextColor 255 128 64 # Unique\n" +
            "\tSetBackgroundColor 50 25 12 # Unique\n" +
            "\tSetBorderColor 255 255 0 # Unique (1-2c)\n" +
            "\tPlayAlertSound 4 200 # Mid Value\n" +
            "\tMinimapIcon 0 Brown Star\n" +
            "\tPlayEffect Brown";

        private const string styleLess1c =
            "\tSetFontSize 40\n" +
            "\tSetTextColor 255 128 64 # Unique\n" +
            "\tSetBackgroundColor 50 25 12 # Unique\n" +
            "\tSetBorderColor 180 90 45 # Unique (<1c)\n" +
            "\tPlayAlertSound 4 200 # Mid Value\n" +
            "\tMinimapIcon 0 Brown Star\n" +
            "\tPlayEffect Brown";

        private const string uniqueNewOrWorthless =
            "Show  # Uniques - New or Worthless\n" +
            "\tRarity = Unique\n" +
            "\tSetFontSize 36\n" +
            "\tSetTextColor 255 128 64 # Unique\n" +
            "\tSetBackgroundColor 50 25 12 # Unique\n" +
            "\tSetBorderColor 180 90 45 # Unique (<1c)\n" +
            "\tPlayAlertSound 4 200 # Mid Value\n" +
            "\tMinimapIcon 0 Brown Star\n" +
            "\tPlayEffect Brown";

        private const string styleDiv10c =
            "\tSetFontSize 45\n" +
            "\tSetTextColor 255 0 175 # Divination Card (10c+)\n" +
            "\tSetBackgroundColor 255 255 255 255 # Divination Card (10c+)\n" +
            "\tSetBorderColor 255 0 175 # Divination Card (10c+)\n" +
            "\tPlayAlertSound 1 200 # High Value\n" +
            "\tMinimapIcon 0 Red Triangle\n" +
            "\tPlayEffect Red";

        private const string styleDiv1c =
            "\tSetFontSize 45\n" +
            "\tSetTextColor 255 255 255 # Divination Card (1c+)\n" +
            "\tSetBackgroundColor 255 0 175 255 # Divination Card (1c+)\n" +
            "\tSetBorderColor 255 255 255 # Divination Card (1c+)\n" +
            "\tPlayAlertSound 5 200 # Divination Card (1c+)\n" +
            "\tMinimapIcon 0 Brown Triangle\n" +
            "\tPlayEffect Brown";

        private const string styleDivLess1c =
            "\tSetFontSize 40\n" +
            "\tSetTextColor 0 0 0 # Divination Card (<1c)\n" +
            "\tSetBackgroundColor 255 0 175 230 # Divination Card (<1c)\n" +
            "\tSetBorderColor 150 30 100 # Divination Card (<1c)\n" +
            "\tPlayAlertSound 5 100 # Divination Card (Low)\n" +
            "\tPlayEffect Brown";

        private const string styleDivNearlyWorthless =
            "\tSetFontSize 36\n" +
            "\tSetTextColor 0 0 0 # Divination Card (Nearly Worthless)\n" +
            "\tSetBackgroundColor 255 0 175 170 # Divination Card (Nearly Worthless)\n" +
            "\tSetBorderColor 0 0 0 # Divination Card (Nearly Worthless)\n" +
            "\tPlayAlertSound 5 0 # Divination Card (Nearly Worthless)";

        private const string styleDivWorthless =
            "\tSetFontSize 32\n" +
            "\tSetTextColor 0 0 0 # Divination Card (Worthless)\n" +
            "\tSetBackgroundColor 255 0 175 120 # Divination Card (Worthless)\n" +
            "\tSetBorderColor 255 0 175 50 # Divination Card (Worthless)\n" +
            "\tDisableDropSound";

        private const string divNewOrWorthless =
            "Show  # Divination Cards - New (Error)\n" +
            "\tClass Divination\n" +
            "\tSetFontSize 40\n" +
            "\tSetTextColor 255 255 255 # Divination Card (1c+)\n" +
            "\tSetBackgroundColor 255 0 175 255 # Divination Card (1c+)\n" +
            "\tSetBorderColor 0 255 0 # Error\n" +
            "\tPlayAlertSound 5 200 # Divination Card (1c+)\n" +
            "\tMinimapIcon 0 White Triangle\n" +
            "\tPlayEffect White";

        private const string headerDiv =
@"##########################################
############ DIVINATION CARDS ############
##########################################
# Section: Divination Cards

# Ordered most expensive first to prevent future name conflicts!
# Prices attained from poe.ninja.
# Future values will fluctuate based on league challenges and the meta.";

        private readonly IReadOnlyList<string> Uniques;
        private readonly IReadOnlyList<string> DivinationCards;
        private readonly Dictionary<string, UniqueBaseType> UniquesSC;
        private readonly Dictionary<string, UniqueBaseType> UniquesHC;
        private readonly Dictionary<string, DivinationCard> DivinationSC;
        private readonly Dictionary<string, DivinationCard> DivinationHC;
        private readonly IReadOnlyList<IReadOnlyList<string>> Conflicts;

        private StreamWriter Writer { get; set; }

        private bool Safe { get; set; }
        private bool HCFriendly { get; set; }

        public FilterWriter(DataModel model)
        {
            DivinationCards = model.DivinationCards;
            Uniques = model.Uniques;
            UniquesSC = model.SC.Uniques;
            UniquesHC = model.HC.Uniques;
            DivinationHC = model.HC.DivinationCards;
            DivinationSC = model.SC.DivinationCards;
            Conflicts = model.DivinationCardNameConflicts;
        }

        public void Create(string path, bool safe, bool hcFriendly)
        {
            try {
                Safe = safe;
                HCFriendly = hcFriendly;
                using (Writer = new StreamWriter(path, false, Encoding.UTF8)) {
                    Writer.WriteLine();
                    Writer.WriteLine(GenerateUniquesString());
                    Writer.Write(GenerateDivinationString());
                }
                Writer = null;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "FilterWriter.Create", MessageBoxButtons.OK);
            }
        }

        private string GenerateDivinationConflictsString()
        {
            StringBuilder sb = new StringBuilder();
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
            return sb.ToString();
        }

        private string GenerateUniquesString()
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

            foreach (KeyValuePair<string, UniqueBaseType> uniq in UniquesSC) {
                UniqueBaseType entry = uniq.Value;
                string baseTy = uniq.Key;
                UniqueValue expectedVal = entry.ExpectedFilterValue;
                UniqueValue filterVal = entry.FilterValue;
                string outputBaseTy = baseTy;
                //int index = baseTy.IndexOf('ö');
                UniqueBaseType entryHC = UniquesHC[baseTy];
                UniqueValue expectedValHC = entryHC.ExpectedFilterValue;

                if (entry.SeverityLevel == 0)
                    expectedVal = filterVal;
                else if (Safe && filterVal.Tier > expectedVal.Tier) {
                    if (entry.Items.Count > 1 || filterVal.LowValue < entry.Items[0].ChaosValue) {
                        expectedVal = UniqueValue.FromTier(expectedVal.Tier + 1);
                    }
                }
                //if SC <1c and HC 5c+ then 1-2c
                if (HCFriendly && expectedVal.Tier < 2 && expectedValHC.Tier > 2 && entryHC.Items.Any(i => i.IsCoreDrop && i.ChaosValue > 4.0f)) {
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

            if (list10c.Count > 0) {
                sb.AppendLine(header10c).AppendLine();
                sb.AppendLine("Show  # Uniques - 10c+").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(list10c)).AppendLine(style10c).AppendLine();
            }
            if (list2to10c.Count > 0) {
                sb.AppendLine(header2to10c).AppendLine();
                sb.AppendLine("Show  # Uniques - 2-10c").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(list2to10c)).AppendLine(style2to10c).AppendLine();
            }
            sb.AppendLine(header1c).AppendLine();
            if (list1to2c.Count > 0) {
                sb.AppendLine("Show  # Uniques - 1-2c").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(list1to2c)).AppendLine(style1c).AppendLine();
            }
            sb.AppendLine(loreweaveStr).AppendLine();
            sb.AppendLine(headerLess1c).AppendLine();
            if (listLess1cLeague.Count > 0)
                sb.AppendLine("Show  # Uniques - <1c - League").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(listLess1cLeague)).AppendLine(styleLess1c).AppendLine();
            if (listLess1cBoss.Count > 0)
                sb.AppendLine("Show  # Uniques - <1c - Boss Prophecy").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(listLess1cBoss)).AppendLine(styleLess1c).AppendLine();
            if (listLess1cShared.Count > 0)
                sb.AppendLine("Show  # Uniques - <1c - Shared").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(listLess1cShared)).AppendLine(styleLess1c).AppendLine();
            if (listLess1cCrafted.Count > 0)
                sb.AppendLine("Show  # Uniques - <1c - Crafted Fated Purchased").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(listLess1cCrafted)).AppendLine(styleLess1c).AppendLine();
            if (listLess1cLabyrinth.Count > 0)
                sb.AppendLine("Show  # Uniques - <1c - Labyrinth").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(listLess1cLabyrinth)).AppendLine(styleLess1c).AppendLine();
            if (listLess1c.Count > 0)
                sb.AppendLine("Show  # Uniques - <1c - Nearly Worthless").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(listLess1c)).AppendLine(styleLess1c).AppendLine();
            sb.AppendLine(uniqueNewOrWorthless);

            return sb.ToString();
        }

        //IEnumerable<string> lines
        private string GenerateDivinationString()
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
                DivinationCard data = DivinationSC[divCard];
                DivinationValue expectedVal = data.ExpectedFilterValue;
                DivinationValue filterVal = data.FilterValue;
                DivinationCard dataHC = DivinationHC[divCard];
                DivinationValue expectedValHC = dataHC.ExpectedFilterValue;
                if (data.SeverityLevel == 0)
                    expectedVal = filterVal;
                else if (Safe && filterVal.LowValue < data.ChaosValue) {
                    expectedVal = DivinationValue.FromTier(expectedVal.Tier + 1);
                }
                //if SC <1c and HC 1c+ then +1 tier
                if (HCFriendly && expectedVal.Tier < 2 && dataHC.ChaosValue > 4.0f) {
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

            sb.AppendLine(headerDiv).AppendLine();
            sb.AppendLine(GenerateDivinationConflictsString()).AppendLine();
            if (list1to10cConflict.Count > 0)
                sb.AppendLine("Show  # Divination Cards - 1c+ (Conflicts)").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(list1to10cConflict)).AppendLine(styleDiv1c).AppendLine();
            if (listLess1cConflict.Count > 0)
                sb.AppendLine("Show  # Divination Cards - <1c (Conflicts)").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(listLess1cConflict)).AppendLine(styleDivLess1c).AppendLine();
            if (listNearlyWorthlessConflict.Count > 0)
                sb.AppendLine("Show  # Divination Cards - Nearly Worthless (Conflicts)").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(listNearlyWorthlessConflict)).AppendLine(styleDivNearlyWorthless).AppendLine();
            if (listWorthlessConflict.Count > 0)
                sb.AppendLine("Hide  # Divination Cards - Worthless (Conflicts)").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(listWorthlessConflict)).AppendLine(styleDivWorthless).AppendLine();
            if (list10c.Count > 0)
                sb.AppendLine("Show  # Divination Cards - 10c+").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(list10c)).AppendLine(styleDiv10c).AppendLine();
            if (list1to10c.Count > 0)
                sb.AppendLine("Show  # Divination Cards - 1c+").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(list1to10c)).AppendLine(styleDiv1c).AppendLine();
            if (listLess1c.Count > 0)
                sb.AppendLine("Show  # Divination Cards - <1c").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(listLess1c)).AppendLine(styleDivLess1c).AppendLine();
            if (listNearlyWorthless.Count > 0)
                sb.AppendLine("Show  # Divination Cards - Nearly Worthless").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(listNearlyWorthless)).AppendLine(styleDivNearlyWorthless).AppendLine();
            if (listWorthless.Count > 0)
                sb.AppendLine("Show  # Divination Cards - Worthless").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(listWorthless)).AppendLine(styleDivWorthless).AppendLine();
            sb.AppendLine(divNewOrWorthless);

            return sb.ToString();
        }

        private string BaseTypeList(List<string> baseTypes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BaseType ");
            foreach (string baseTy in baseTypes) {
                string result = baseTy;
                if (baseTy.Contains(' '))
                    result = "\"" + baseTy + "\"";
                sb.Append(result).Append(' ');
            }
            return sb.Remove(sb.Length - 1, 1).ToString();
        }
    }
}
