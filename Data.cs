using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FileHelpers;
using Newtonsoft.Json.Linq;

namespace PoE_Price_Lister
{
    internal class Data
    {
        private Dictionary<string, DivinationData> divination;
        private Dictionary<string, UniqueBaseEntry> uniques;
        private readonly Dictionary<string, DivinationData> divinationHC = new Dictionary<string, DivinationData>();
        private readonly Dictionary<string, UniqueBaseEntry> uniquesHC = new Dictionary<string, UniqueBaseEntry>();
        private readonly Dictionary<string, DivinationData> divinationSC = new Dictionary<string, DivinationData>();
        private readonly Dictionary<string, UniqueBaseEntry> uniquesSC = new Dictionary<string, UniqueBaseEntry>();
        private bool isHardcore = false;
        private List<string> divinationBaseTypes = new List<string>();
        private List<string> csvUniquesBaseTypes = new List<string>();

        private const string csvFile = "poe_uniques.csv";
        private const string league = "Delve";

        private const string filterURL = "https://raw.githubusercontent.com/ffhighwind/PoE-Price-Lister/master/Resources/Filters/S1_Regular_Highwind.filter";
        private const string divinationJsonURL = "http://poe.ninja/api/Data/GetDivinationCardsOverview?league=";
        private const string armorJsonURL = "http://poe.ninja/api/Data/GetUniqueArmourOverview?league=";
        private const string flaskJsonURL = "http://poe.ninja/api/Data/GetUniqueFlaskOverview?league=";
        private const string weaponJsonURL = "http://poe.ninja/api/Data/GetUniqueWeaponOverview?league=";
        private const string accessoryJsonURL = "http://poe.ninja/api/Data/GetUniqueAccessoryOverview?league=";
        private const string baseTypeRegexStr = @"""[A-Za-z'\-, ]+""|[A-Za-z'\-]+";

        private const string uniquesSectionStart = "# Section: Uniques";
        private const string uniquesSectionEnd = "####";
        private const string divSectionStart = "# Section: Divination Cards";
        private const string divSectionEnd = "####";

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

Show  # Uniques - <1c - ilvl <67
    Rarity = Unique
    ItemLevel < 67
    SetFontSize 40
    SetTextColor 255 128 64 # Unique
    SetBackgroundColor 50 25 12 # Unique
    SetBorderColor 180 90 45 # Unique (<1c)
    PlayAlertSound 4 200 # Mid Value";

        private const string loreweaveStr =
@"# Loreweave (60x rings)
Show  # Uniques - 1-2c
    Rarity = Unique
    Class Rings
    SetFontSize 40
    SetTextColor 255 128 64 # Unique
    SetBackgroundColor 50 25 12 # Unique
    SetBorderColor 255 255 0 # Unique (1-2c)
    PlayAlertSound 4 200 # Mid Value";

        private const string style10c =
@"	SetFontSize 45
    SetTextColor 255 128 64 # Unique (10c+)
    SetBackgroundColor 255 255 255 255 # Unique (10c+)
    SetBorderColor 255 128 64 # Unique (10c+)
    PlayAlertSound 1 200 # High Value";

        private const string style2to10c =
@"	SetFontSize 45
    SetTextColor 255 128 64 # Unique
    SetBackgroundColor 50 25 12 # Unique
    SetBorderColor 255 255 255 # Unique (2-10c)
    PlayAlertSound 1 200 # High Value";

        private const string style1c =
@"	SetFontSize 40
    SetTextColor 255 128 64 # Unique
    SetBackgroundColor 50 25 12 # Unique
    SetBorderColor 255 255 0 # Unique (1-2c)
    PlayAlertSound 4 200 # Mid Value";

        private const string styleLess1c =
@"	SetFontSize 40
    SetTextColor 255 128 64 # Unique
    SetBackgroundColor 50 25 12 # Unique
    SetBorderColor 180 90 45 # Unique (<1c)
    PlayAlertSound 4 200 # Mid Value";

        private const string uniqueNewOrWorthless =
@"Show  # Uniques - New or Worthless
    Rarity = Unique
    SetFontSize 36
    SetTextColor 255 128 64 # Unique
    SetBackgroundColor 50 25 12 # Unique
    SetBorderColor 180 90 45 # Unique (<1c)
    PlayAlertSound 4 200 # Mid Value";

        private const string styleDiv10c =
@"	SetFontSize 45
    SetTextColor 255 0 175 # Divination Card (10c+)
    SetBackgroundColor 255 255 255 255 # Divination Card (10c+)
    SetBorderColor 255 0 175 # Divination Card (10c+)
    PlayAlertSound 1 200 # High Value";

        private const string styleDiv1c =
@"	SetFontSize 45
    SetTextColor 255 255 255 # Divination Card (1c+)
    SetBackgroundColor 255 0 175 255 # Divination Card (1c+)
    SetBorderColor 255 255 255 # Divination Card (1c+)
    PlayAlertSound 5 200 # Divination Card (1c+)";

        private const string styleDivLess1c =
@"	SetFontSize 40
    SetTextColor 0 0 0 # Divination Card (<1c)
    SetBackgroundColor 255 0 175 230 # Divination Card (<1c)
    SetBorderColor 150 30 100 # Divination Card (<1c)
    PlayAlertSound 5 100 # Divination Card (Low)";

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
    PlayAlertSound 5 200 # Divination Card (1c+)";

        private const string headerDiv =
@"##########################################
############ DIVINATION CARDS ############
##########################################
# Section: Divination Cards

# Ordered most expensive first to prevent future name conflicts!
# Prices attained from poe.ninja.
# Future values will fluctuate based on league challenges and the meta.";

        private readonly List<List<string>> conflicts = new List<List<string>>();

        public Data()
        {
            divination = divinationSC;
            uniques = uniquesSC;
        }

        private void SetLeague(bool hardcore)
        {
            isHardcore = hardcore;
            if (hardcore) {
                divination = divinationHC;
                uniques = uniquesHC;
            }
            else {
                divination = divinationSC;
                uniques = uniquesSC;
            }
        }

        public IEnumerable<string> GetUniques()
        {
            return csvUniquesBaseTypes;
        }

        public IEnumerable<string> GetDivinationCards()
        {
            return divinationBaseTypes;
        }

        public UniqueBaseEntry GetUniqueEntrySC(string baseType)
        {
            return uniquesSC[baseType];
        }

        public UniqueBaseEntry GetUniqueEntryHC(string baseType)
        {
            return uniquesHC[baseType];
        }

        public DivinationData GetDivinationEntrySC(string name)
        {
            return divinationSC[name];
        }

        public DivinationData GetDivinationEntryHC(string name)
        {
            return divinationHC[name];
        }

        public void Load(string filename)
        {
            try {
                string[] lines = System.IO.File.ReadAllLines(filename);
                foreach (string baseTy in csvUniquesBaseTypes) {
                    uniquesSC[baseTy].FilterValue.Value = UniqueValue.Unknown;
                    uniquesHC[baseTy].FilterValue.Value = UniqueValue.Unknown;
                }
                GetFilterData(lines);
                SetLeague(true);
                GetFilterData(lines);
                SetLeague(false);
                foreach (string baseTy in csvUniquesBaseTypes) {
                    uniquesSC[baseTy].CalculateExpectedValue();
                    uniquesHC[baseTy].CalculateExpectedValue();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Data.Load", MessageBoxButtons.OK);
            }
        }

        public void GetData()
        {
            for (int i = 0; i < 2; i++) //hardcore and not hardcore
            {
                GetCSVData(!isHardcore);
                GetJsonData();

                try {
                    string filterString = ReadWebPage(filterURL, "");
                    if (filterString.Length == 0) {
                        MessageBox.Show("Failed to read the web URL: " + filterURL, "Error", MessageBoxButtons.OK);
                    }
                    else {
                        string[] lines = filterString.Split('\n');
                        GetFilterData(lines);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Data.GetData", MessageBoxButtons.OK);
                    Application.Exit();
                }
                SetLeague(true);
            }
            SetLeague(false);

            List<string> uniqBasesToRemove = new List<string>();

            foreach (string uniq in csvUniquesBaseTypes) {
                if (uniq.EndsWith(" Piece") || uniq.EndsWith(" Talisman")) {
                    uniqBasesToRemove.Add(uniq);
                }
            }
            foreach (string uniq in uniqBasesToRemove) {
                uniques.Remove(uniq);
                csvUniquesBaseTypes.Remove(uniq);
            }

            divinationBaseTypes = divinationBaseTypes.Distinct().ToList();
            divinationBaseTypes.Sort();

            csvUniquesBaseTypes = csvUniquesBaseTypes.Distinct().ToList();
            csvUniquesBaseTypes.Sort();

            foreach (string v in csvUniquesBaseTypes) {
                uniquesSC[v].CalculateExpectedValue();
                uniquesHC[v].CalculateExpectedValue();
            }
            GetDivinationCardConflicts();
        }

        private void GetJsonData()
        {
            string leagueStr = isHardcore ? "Hardcore " + league : league;
            FillJsonData(divinationJsonURL + leagueStr, DivinationJsonHandler);
            FillJsonData(armorJsonURL + leagueStr, UniqueJsonHandler);
            FillJsonData(weaponJsonURL + leagueStr, UniqueJsonHandler);
            FillJsonData(flaskJsonURL + leagueStr, UniqueJsonHandler);
            FillJsonData(accessoryJsonURL + leagueStr, UniqueJsonHandler);
        }

        private void GetCSVData(bool addBaseTypes)
        {
            try {
                FileHelperEngine<UniqueCsvData> engine = new FileHelperEngine<UniqueCsvData>(Encoding.UTF7);
                UniqueCsvData[] records = engine.ReadFile(csvFile);
                foreach (UniqueCsvData data in records) {
                    if (!uniques.TryGetValue(data.BaseType, out UniqueBaseEntry entry)) {
                        entry = new UniqueBaseEntry {
                            BaseType = data.BaseType
                        };
                        uniques.Add(data.BaseType, entry);
                        if (addBaseTypes)
                            csvUniquesBaseTypes.Add(data.BaseType);
                    }
                    entry.Add(data);
                }
                List<UniqueData> resortList = new List<UniqueData>();
                foreach (string baseTy in csvUniquesBaseTypes) {
                    List<UniqueData> items = uniques[baseTy].Items;
                    items.Sort((lhs, rhs) => { return lhs.ChaosValue < rhs.ChaosValue ? -1 : 1; });
                    foreach (UniqueData item in items) {
                        if (item.League.Length > 0)
                            resortList.Add(item);
                    }
                    foreach (UniqueData item in resortList) {
                        items.Remove(item);
                        items.Add(item);
                    }
                    resortList.Clear();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Data.GetCSVData", MessageBoxButtons.OK);
                Application.Exit();
            }
        }

        private bool GetLines(string[] lines, ref int startIndex, out int endIndex, string startLine, string endLine)
        {
            int index = startIndex;
            for (; index < lines.Length; index++) {
                if (lines[index].StartsWith(startLine)) {
                    startIndex = index;
                    for (; index < lines.Length; index++) {
                        if (lines[index].StartsWith(endLine)) {
                            endIndex = index;
                            return true;
                        }
                    }
                }
            }
            endIndex = index;
            return false;
        }

        private void GetFilterData(string[] lines)
        {
            int startIndex = 0;

            if (GetLines(lines, ref startIndex, out int endIndex, uniquesSectionStart, uniquesSectionEnd))
                GetFilterUniqueData(new ArraySegment<string>(lines, startIndex, endIndex - startIndex));

            startIndex = endIndex;
            if (GetLines(lines, ref startIndex, out endIndex, divSectionStart, divSectionEnd))
                GetFilterDivinationData(new ArraySegment<string>(lines, startIndex, endIndex - startIndex));
        }

        private void GetFilterDivinationData(IEnumerable<string> lines)
        {
            DivinationValueEnum value;
            while (lines.Count() > 1) {
                lines = lines.SkipWhile(aline => !aline.StartsWith("Show ") && !aline.StartsWith("Hide "));
                if (!lines.Any())
                    return;
                string line = lines.ElementAt(0);

                if (!line.Contains("# Divination Cards - "))
                    continue;

                if (line.Contains("10c+"))
                    value = DivinationValueEnum.Chaos10;
                else if (line.Contains("1c+"))
                    value = DivinationValueEnum.Chaos1to10;
                else if (line.Contains("<1c") || line.Contains("< 1c"))
                    value = DivinationValueEnum.ChaosLess1;
                else if (line.Contains("Nearly Worthless"))
                    value = DivinationValueEnum.NearlyWorthless;
                else if (line.Contains("Worthless"))
                    value = DivinationValueEnum.Worthless;
                else {
                    if (!line.Contains("New (Error)"))
                        MessageBox.Show("Unexpected Divination input: " + line, "Error", MessageBoxButtons.OK);
                    lines = lines.Skip(1);
                    continue;
                }
                lines = lines.Skip(1).SkipWhile(aline => !aline.TrimStart().StartsWith("BaseType ") && !aline.StartsWith("Show ") && !aline.StartsWith("Hide "));
                line = lines.ElementAt(0).TrimStart();
                if (line.StartsWith("BaseType "))
                    FillFilterDivinationData(GetBaseTypes(line.Substring(9)), value);
            }
        }

        private void GetFilterUniqueData(IEnumerable<string> lines)
        {
            UniqueValue value;
            while (lines.Any()) {
                lines = lines.SkipWhile(aline => !aline.StartsWith("Show ") && !aline.StartsWith("Hide "));
                if (!lines.Any())
                    return;
                string line = lines.ElementAt(0);

                if (!line.Contains("# Uniques -"))
                    continue;

                if (line.Contains("10c+"))
                    value = UniqueValue.Chaos10;
                else if (line.Contains("2-10c"))
                    value = UniqueValue.Chaos2to10;
                else if (line.Contains("1-2c"))
                    value = UniqueValue.Chaos1to2;
                else if (line.Contains("<1c") || line.Contains("< 1c")) {
                    if (line.Contains("<67")) {
                        lines = lines.Skip(1);
                        continue;
                    }
                    else if (line.Contains("Boss"))
                        value = UniqueValue.ChaosLess1Boss;
                    else if (line.Contains("League"))
                        value = UniqueValue.ChaosLess1League;
                    else if (line.Contains("Shared"))
                        value = UniqueValue.ChaosLess1Shared;
                    else if (line.Contains("Crafted"))
                        value = UniqueValue.ChaosLess1Crafted;
                    else if (line.Contains("Labyrinth"))
                        value = UniqueValue.ChaosLess1Labyrinth;
                    else //Nearly Worthless
                        value = UniqueValue.ChaosLess1;
                }
                else {
                    if (!line.Contains("New or Worthless"))
                        MessageBox.Show("Unexpected Unique input: " + line, "Error", MessageBoxButtons.OK);
                    lines = lines.Skip(1);
                    continue;
                }
                lines = lines.Skip(1).SkipWhile(aline => !aline.TrimStart().StartsWith("BaseType ") && !aline.StartsWith("Show ") && !aline.StartsWith("Hide "));
                line = lines.ElementAt(0).TrimStart();
                if (line.StartsWith("BaseType "))
                    FillFilterUniqueData(GetBaseTypes(line.Substring(9)), value);
            }
        }

        private void FillJsonData(string url, Action<JsonData> handler)
        {
            try {
                string jsonURLString = ReadWebPage(url, "application/json");
                if (jsonURLString.Length == 0) {
                    MessageBox.Show("Failed to read the web URL: " + url, "Error", MessageBoxButtons.OK);
                }
                else {
                    JObject jsonString = JObject.Parse(jsonURLString);
                    JToken results = jsonString["lines"];
                    foreach (JToken result in results) {
                        JsonData jdata = result.ToObject<JsonData>();
                        handler(jdata);
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Data.FillJsonData", MessageBoxButtons.OK);
            }
        }

        private void UniqueJsonHandler(JsonData jdata)
        {
            string baseTy = jdata.BaseType;
            if (!uniques.TryGetValue(baseTy, out UniqueBaseEntry data)) {
                data = new UniqueBaseEntry();
                uniques.Add(baseTy, data);
                MessageBox.Show("JSON: The CSV file is missing BaseType: " + baseTy, "Error", MessageBoxButtons.OK);
            }

            data.Add(jdata);
        }

        private void DivinationJsonHandler(JsonData jdata)
        {
            string name = jdata.Name;
            if (!divination.TryGetValue(name, out DivinationData data)) {
                data = new DivinationData();
                divination.Add(name, data);
                divinationBaseTypes.Add(name);
            }
            data.Load(jdata);
        }

        private List<string> GetBaseTypes(string line)
        {
            MatchCollection collection = Regex.Matches(line, baseTypeRegexStr);
            List<string> output = new List<string>();

            foreach (Match m in collection) {
                string baseTy = m.Value;
                if (baseTy.Length == 0)
                    continue;
                if (baseTy[0] == '"')
                    baseTy = baseTy.Substring(1, baseTy.Length - 2);
                output.Add(baseTy);
            }
            return output;
        }

        private void FillFilterUniqueData(List<string> baseTypes, UniqueValue value)
        {

            foreach (string baseTy in baseTypes) {
                if (!uniques.TryGetValue(baseTy, out UniqueBaseEntry data)) {
                    if (baseTy == "Maelstr") {
                        if (!uniques.TryGetValue("Maelström Staff", out data)) {
                            data = new UniqueBaseEntry();
                            uniques.Add("Maelström Staff", data);
                        }
                    }
                    else {
                        data = new UniqueBaseEntry();
                        uniques.Add(baseTy, data);
                        MessageBox.Show("Filter: unknown basetype: " + baseTy, "Error", MessageBoxButtons.OK);
                    }
                }
                data.FilterValue.Value = value;
            }
        }

        private void FillFilterDivinationData(List<string> baseTypes, DivinationValueEnum value)
        {

            foreach (string baseTy in baseTypes) {
                if (!divination.TryGetValue(baseTy, out DivinationData data)) {
                    data = new DivinationData(baseTy);
                    divination.Add(baseTy, data);
                    divinationBaseTypes.Add(baseTy);
                }
                data.FilterValue = new DivinationFilterValue(value);
            }
        }

        private string ReadWebPage(string url, string headerMedia)
        {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(url);
                if (headerMedia.Length > 0)
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(headerMedia));
                HttpResponseMessage response;
                try {
                    response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                        return response.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex) {
                    MessageBox.Show("Error reading webpage " + url + "\n" + ex.Message, "Data.ReadWebPage", MessageBoxButtons.OK);
                    Application.Exit();
                }
                return "";
            }
        }

        private void GetDivinationCardConflicts()
        {
            try {
                List<string> conflictsList = new List<string>();
                for (int i = 0; i < divinationBaseTypes.Count; i++) {
                    string divBaseTy = divinationBaseTypes[i].ToLower();
                    for (int j = i + 1; j < divinationBaseTypes.Count; j++) {
                        string divBaseTy2 = divinationBaseTypes[j].ToLower();
                        if (divBaseTy.Contains(divBaseTy2) || divBaseTy2.Contains(divBaseTy))
                            conflictsList.Add(divinationBaseTypes[j]);
                    }
                    if (conflictsList.Count > 0) {
                        conflictsList.Add(divinationBaseTypes[i]);
                        conflictsList.Sort((left, right) => left.Length - right.Length);
                        conflicts.Add(conflictsList);
                        conflictsList = new List<string>();
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Data.GetDivinationCardConflicts", MessageBoxButtons.OK);
            }
        }

        public void GenerateFilterFile(string path, bool safe)
        {
            try {
                /*
                var fileout = new System.IO.StreamWriter(filterOutputFile, false);
                string[] lines = System.IO.File.ReadAllLines(filterInputFile);
                int startIndex = 0, endIndex = 0;

                if (!GetLines(lines, ref startIndex, out endIndex, uniquesSectionStart, uniquesSectionEnd))
                    throw new Exception("Missing uniques section in filter file.");

                //output the start of the file
                for(int i = 0; i < startIndex; i++)
                    fileout.WriteLine(lines[i]);

                //generate uniques list
                fileout.WriteLine(GenerateUniquesString(new ArraySegment<string>(lines, startIndex, endIndex - startIndex)));

                int prevEndIndex = endIndex;
                startIndex = endIndex;
                if (!GetLines(lines, ref startIndex, out endIndex, divSectionStart, divSectionEnd))
                    throw new Exception("Missing divination cards section in filter file.");

                //output lines between uniques and divination cards
                for (int i = prevEndIndex; i < startIndex; i++)
                    fileout.WriteLine(lines[i]);

                //generate divination list
                fileout.WriteLine(GenerateDivinationString(new ArraySegment<string>(lines, startIndex, endIndex - startIndex)));

                // output rest of the file
                for(int i = endIndex; i < lines.Length; i++)
                    fileout.WriteLine(lines[i]);
                fileout.Close();
                */

                System.IO.StreamWriter fileout = new System.IO.StreamWriter(path, false);
                fileout.WriteLine();
                fileout.WriteLine(GenerateUniquesString(safe));
                fileout.WriteLine(GenerateDivinationString(safe));
                fileout.Close();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Data.GenerateFilterFile", MessageBoxButtons.OK);
            }
        }

        private string GenerateDivinationConflictsString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# Potential Conflicts!!! (They have been separated but may need to be reorganized)");
            foreach (List<string> list in conflicts) {
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

        //IEnumerable<string> lines
        private string GenerateUniquesString(bool safe)
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

            foreach (KeyValuePair<string, UniqueBaseEntry> uniq in uniques) {
                UniqueBaseEntry entry = uniq.Value;
                string baseTy = uniq.Key;
                UniqueFilterValue expectedVal = entry.ExpectedFilterValue;
                UniqueFilterValue filterVal = entry.FilterValue;
                string outputBaseTy = baseTy;
                int index = baseTy.IndexOf('ö');
                if (index > 0)
                    outputBaseTy = baseTy.Substring(0, index);

                if (safe && expectedVal.Value != filterVal.Value) {
                    if (expectedVal.Value == UniqueValue.Unknown) {
                        float maxVal = 0.0f;
                        foreach (UniqueData item in entry.Items) {
                            if (maxVal < item.ChaosValue && !item.IsCrafted && !item.IsFated)
                                maxVal = item.ChaosValue;
                        }
                        if (maxVal > 0.8f)
                            expectedVal.Value = UniqueValue.ChaosLess1;
                    }
                    else if (filterVal.Value == UniqueValue.Unknown && expectedVal.Value == UniqueValue.ChaosLess1)
                        continue;
                    else {
                        int expectTier = expectedVal.ValueTier;
                        if (expectTier < filterVal.ValueTier)
                            expectedVal = UniqueFilterValue.FromValueTier(expectTier + 1);
                    }
                }
                switch (expectedVal.Value) {
                    case UniqueValue.Chaos10:
                        list10c.Add(outputBaseTy);
                        break;
                    case UniqueValue.Chaos2to10:
                        list2to10c.Add(outputBaseTy);
                        break;
                    case UniqueValue.Chaos1to2:
                        list1to2c.Add(outputBaseTy);
                        break;
                    case UniqueValue.ChaosLess1:
                        listLess1c.Add(outputBaseTy);
                        break;
                    case UniqueValue.ChaosLess1League:
                        listLess1cLeague.Add(outputBaseTy);
                        break;
                    case UniqueValue.ChaosLess1Boss:
                        listLess1cBoss.Add(outputBaseTy);
                        break;
                    case UniqueValue.ChaosLess1Shared:
                        listLess1cShared.Add(outputBaseTy);
                        break;
                    case UniqueValue.ChaosLess1Crafted:
                        listLess1cCrafted.Add(outputBaseTy);
                        break;
                    case UniqueValue.ChaosLess1Labyrinth:
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
                sb.AppendLine("Show  # Uniques - <1c - League").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(listLess1cLeague)).AppendLine();
            if (listLess1cBoss.Count > 0)
                sb.AppendLine("Show  # Uniques - <1c - Boss Prophecy").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(listLess1cBoss)).AppendLine();
            if (listLess1cShared.Count > 0)
                sb.AppendLine("Show  # Uniques - <1c - Shared").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(listLess1cShared)).AppendLine();
            if (listLess1cCrafted.Count > 0)
                sb.AppendLine("Show  # Uniques - <1c - Crafted Fated Purchased").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(listLess1cCrafted)).AppendLine();
            if (listLess1cLabyrinth.Count > 0)
                sb.AppendLine("Show  # Uniques - <1c - Labyrinth").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(listLess1cLabyrinth)).AppendLine();
            if (listLess1c.Count > 0)
                sb.AppendLine("Show  # Uniques - <1c - Nearly Worthless").AppendLine("\tRarity = Unique").Append('\t').AppendLine(BaseTypeList(listLess1c)).AppendLine();
            sb.AppendLine(uniqueNewOrWorthless);

            return sb.ToString();
        }

        //IEnumerable<string> lines
        private string GenerateDivinationString(bool safe)
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

            foreach (string baseTy in divinationBaseTypes) {
                DivinationData data = divination[baseTy];
                DivinationFilterValue expectedVal = data.ExpectedFilterValue;
                DivinationFilterValue filterVal = data.FilterValue;
                if (safe && expectedVal.Value != filterVal.Value) {
                    int filterTier = filterVal.ValueTier;
                    int expectedTier = expectedVal.ValueTier;
                    if (filterTier > expectedTier)
                        expectedTier = expectedTier + 1;
                    else if (filterTier < expectedTier && expectedTier < 4)
                        expectedTier = expectedTier - 1;
                    expectedVal = DivinationFilterValue.FromValueTier(expectedTier);
                }
                switch (expectedVal.Value) {
                    case DivinationValueEnum.Chaos10:
                        list10c.Add(baseTy);
                        break;
                    case DivinationValueEnum.Chaos1to10:
                        if (list10c.Exists(str => baseTy.Contains(str)))
                            list1to10cConflict.Add(baseTy);
                        else
                            list1to10c.Add(baseTy);
                        break;
                    case DivinationValueEnum.ChaosLess1:
                        if (list10c.Exists(str => baseTy.Contains(str)) || list1to10c.Exists(str => baseTy.Contains(str)))
                            listLess1cConflict.Add(baseTy);
                        else
                            listLess1c.Add(baseTy);
                        break;
                    case DivinationValueEnum.NearlyWorthless:
                        if (list10c.Exists(str => baseTy.Contains(str)) || list1to10c.Exists(str => baseTy.Contains(str)) || listLess1c.Exists(str => baseTy.Contains(str)))
                            listNearlyWorthlessConflict.Add(baseTy);
                        else
                            listNearlyWorthless.Add(data.Name);
                        break;
                    case DivinationValueEnum.Worthless:
                        if (list10c.Exists(str => baseTy.Contains(str)) || list1to10c.Exists(str => baseTy.Contains(str))
                            || listLess1c.Exists(str => baseTy.Contains(str)) || listNearlyWorthless.Exists(str => baseTy.Contains(str))) {
                            listWorthlessConflict.Add(baseTy);
                        }
                        else
                            listWorthless.Add(baseTy);
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
                sb.AppendLine("Show  # Divination Cards - <1c (Conflicts)").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(listLess1cConflict)).AppendLine();
            if (listNearlyWorthlessConflict.Count > 0)
                sb.AppendLine("Show  # Divination Cards - Nearly Worthless (Conflicts)").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(listNearlyWorthlessConflict)).AppendLine();
            if (listWorthlessConflict.Count > 0)
                sb.AppendLine("Hide  # Divination Cards - Worthless (Conflicts)").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(listWorthlessConflict)).AppendLine(styleDivWorthless).AppendLine();
            if (list10c.Count > 0)
                sb.AppendLine("Show  # Divination Cards - 10c+").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(list10c)).AppendLine(styleDiv10c).AppendLine();
            if (list1to10c.Count > 0)
                sb.AppendLine("Show  # Divination Cards - 1c+").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(list1to10c)).AppendLine(styleDiv1c).AppendLine();
            if (listLess1c.Count > 0)
                sb.AppendLine("Show  # Divination Cards - <1c").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(listLess1c)).AppendLine();
            if (listNearlyWorthless.Count > 0)
                sb.AppendLine("Show  # Divination Cards - Nearly Worthless").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(listNearlyWorthless)).AppendLine();
            if (listWorthless.Count > 0)
                sb.AppendLine("Show  # Divination Cards - Worthless").AppendLine("\tClass Divination").Append('\t').AppendLine(BaseTypeList(listWorthless)).AppendLine(styleDivWorthless).AppendLine();
            sb.AppendLine(divNewOrWorthless).AppendLine();

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
