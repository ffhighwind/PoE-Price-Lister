using System;
using System.Collections.Generic;
using System.IO;
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
    public class DataModel
    {

        ////private const string csvFile = "poe_uniques.csv";
        private const string league = "Synthesis";

        private const string uniquesCsvURL = "https://raw.githubusercontent.com/ffhighwind/PoE-Price-Lister/master/poe_uniques.csv";
        private const string filterURL = "https://raw.githubusercontent.com/ffhighwind/PoE-Price-Lister/master/Resources/Filters/S1_Regular_Highwind.filter";
        private const string jsonURL = "http://poe.ninja/api/Data/Get{0}Overview?league={1}";
        //{0} = "UniqueAccessory", "UniqueJewel", "UniqueMap", "UniqueArmour", "UniqueFlask",
        // "UniqueWeapon", "DivinationCards", "Fragment", "Currency", "Prophecy", "Essence", "SkillGem", "HelmEnchant"
        // Resonators/Fossils are not implemented as an API yet

        private static readonly Regex baseTypeRegex = new Regex(@"""[A-Za-zö'\-, ]+""|[A-Za-zö'\-]+", RegexOptions.Compiled);

        public LeagueData HC { get; private set; } = new LeagueData(true);
        public LeagueData SC { get; private set; } = new LeagueData(false);

        public IReadOnlyList<string> Uniques { get; private set; }
        public IReadOnlyList<string> DivinationCards { get; private set; }

        private const string uniquesSectionStart = "# Section: Uniques";
        private const string uniquesSectionEnd = "####";
        private const string divSectionStart = "# Section: Divination Cards";
        private const string divSectionEnd = "####";

        private const int MaxErrors = 5;
        private int ErrorCount = 0;

        private List<IReadOnlyList<string>> conflicts = new List<IReadOnlyList<string>>();
        public IReadOnlyList<IReadOnlyList<string>> DivinationCardNameConflicts => conflicts;

        public void Load()
        {

            try {
                ErrorCount = 0;
                LoadCsv();
                GetJsonData(HC);
                GetJsonData(SC);
                List<string> uniquesToRemove = SC.Uniques.Keys.Where(uniq => uniq.EndsWith(" Piece") || uniq.EndsWith(" Talisman")).ToList();
                foreach (string uniq in uniquesToRemove) {
                    SC.Uniques.Remove(uniq);
                    HC.Uniques.Remove(uniq);
                }
                DivinationCards = SC.DivinationCards.Keys.ToList();
                Uniques = SC.Uniques.Keys.ToList();
                GetDivinationCardConflicts();
                string filterString = ReadWebPage(filterURL);
                if (filterString.Length == 0) {
                    MessageBox.Show("Failed to read the web URL: " + filterURL, "Error", MessageBoxButtons.OK);
                    Environment.Exit(1);
                }
                Load(filterString.Split('\n'));
                ////Load(@"..\..\Resources\Filters\S1_Regular_Highwind.filter");
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                Environment.Exit(1);
            }
        }

        public void Load(string filename)
        {
            using (TextReader reader = Util.TextReader(filename)) {
                List<string> lines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null) {
                    lines.Add(line);
                }
                Load(lines.ToArray());
            }
        }

        public void Load(string[] lines)
        {
            try {
                ErrorCount = 0;
                GetFilterData(lines);
                foreach (UniqueBaseType baseEntry in SC.Uniques.Values) {
                    baseEntry.CalculateExpectedValue();
                }
                foreach (UniqueBaseType baseEntry in HC.Uniques.Values) {
                    baseEntry.CalculateExpectedValue();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Data.Load", MessageBoxButtons.OK);
                Environment.Exit(1);
            }
        }

        private void GetJsonData(LeagueData data)
        {
            string leagueStr = data.IsHardcore ? "Hardcore " + league : league;
            FillJsonData(string.Format(jsonURL, "DivinationCards", leagueStr), data, DivinationJsonHandler);
            FillJsonData(string.Format(jsonURL, "UniqueArmour", leagueStr), data, UniqueJsonHandler);
            FillJsonData(string.Format(jsonURL, "UniqueFlask", leagueStr), data, UniqueJsonHandler);
            FillJsonData(string.Format(jsonURL, "UniqueWeapon", leagueStr), data, UniqueJsonHandler);
            FillJsonData(string.Format(jsonURL, "UniqueAccessory", leagueStr), data, UniqueJsonHandler);
        }

        private void FillJsonData(string url, LeagueData data, Action<JsonData, LeagueData> handler)
        {
            string jsonURLString = ReadWebPage(url, "application/json");
            if (jsonURLString.Length == 0) {
                MessageBox.Show("Failed to read the web URL: " + url, "Error", MessageBoxButtons.OK);
                Environment.Exit(1);
            }
            else {
                JObject jsonString = JObject.Parse(jsonURLString);
                JToken results = jsonString["lines"];
                foreach (JToken result in results) {
                    JsonData jdata = result.ToObject<JsonData>();
                    handler(jdata, data);
                }
            }
        }

        private void UniqueJsonHandler(JsonData jdata, LeagueData data)
        {
            string baseTy = jdata.BaseType;
            if (!data.Uniques.TryGetValue(baseTy, out UniqueBaseType uniq)) {
                uniq = new UniqueBaseType(baseTy);
                data.Uniques.Add(baseTy, uniq);
                if (!data.IsHardcore) {
                    MessageBox.Show("JSON: The CSV file is missing BaseType: " + baseTy, "Error", MessageBoxButtons.OK);
                    ErrorCount++;
                }
            }
            if (!uniq.Add(jdata) && !data.IsHardcore) {
                MessageBox.Show("JSON: The CSV file is missing: " + jdata.BaseType + " " + jdata.Name, "Error", MessageBoxButtons.OK);
                ErrorCount++;
            }
            if (ErrorCount > MaxErrors) {
                Environment.Exit(1);
            }
        }

        private void DivinationJsonHandler(JsonData jdata, LeagueData data)
        {
            string name = jdata.Name;
            if (!data.DivinationCards.TryGetValue(name, out DivinationCard div)) {
                div = new DivinationCard();
                data.DivinationCards.Add(name, div);
            }
            data.DivinationCards[name].Load(jdata);
        }

        private void LoadCsv()
        {
            try {
                FileHelperEngine<UniqueBaseTypeCsv> engine = new FileHelperEngine<UniqueBaseTypeCsv>(Encoding.UTF8);
				string csvText = new FileInfo("poe_uniques.csv").Exists ? 
					File.ReadAllText("poe_uniques.csv", Encoding.UTF8) 
					: ReadWebPage(uniquesCsvURL, "", Encoding.UTF8);
				UniqueBaseTypeCsv[] records = engine.ReadString(csvText);
				HashSet<string> baseTypes = new HashSet<string>();
                foreach (UniqueBaseTypeCsv data in records) {

                    if (!baseTypes.Contains(data.BaseType)) {
                        baseTypes.Add(data.BaseType);
                        SC.Uniques[data.BaseType] = new UniqueBaseType(data.BaseType);
                        HC.Uniques[data.BaseType] = new UniqueBaseType(data.BaseType);
                    }
                    SC.Uniques[data.BaseType].Add(data);
                    HC.Uniques[data.BaseType].Add(data);
                }
                foreach (string baseType in baseTypes) {
                    Sort(SC, baseType);
                    Sort(HC, baseType);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Data.GetCSVData", MessageBoxButtons.OK);
                Environment.Exit(1);
            }
        }

        private void Sort(LeagueData data, string baseType)
        {
            List<UniqueItem> resortList = new List<UniqueItem>();
            List<UniqueItem> items = data.Uniques[baseType].Items;
            foreach (UniqueItem item in items) {
                if (item.League.Length > 0)
                    resortList.Add(item);
            }
            foreach (UniqueItem item in resortList) {
                items.Remove(item);
                items.Add(item);
            }
        }

        private void GetDivinationCardConflicts()
        {
            List<string> conflictsList = new List<string>();
            for (int i = 0; i < DivinationCards.Count; i++) {
                string divBaseTy = DivinationCards[i].ToLower();
                for (int j = i + 1; j < DivinationCards.Count; j++) {
                    string divBaseTy2 = DivinationCards[j].ToLower();
                    if (divBaseTy.Contains(divBaseTy2) || divBaseTy2.Contains(divBaseTy))
                        conflictsList.Add(DivinationCards[j]);
                }
                if (conflictsList.Count > 0) {
                    conflictsList.Add(DivinationCards[i]);
                    conflictsList.Sort((left, right) => left.Length - right.Length);
                    conflicts.Add(conflictsList);
                    conflictsList = new List<string>();
                }
            }
        }

        private bool GetLines(IReadOnlyList<string> lines, ref int startIndex, out int endIndex, string startLine, string endLine)
        {
            int index = startIndex;
            for (; index < lines.Count; index++) {
                if (lines[index].StartsWith(startLine)) {
                    startIndex = index;
                    for (; index < lines.Count; index++) {
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
            foreach (UniqueBaseType uniq in SC.Uniques.Values) {
                uniq.FilterValue = UniqueValue.Unknown;
            }
            foreach (UniqueBaseType uniq in HC.Uniques.Values) {
                uniq.FilterValue = UniqueValue.Unknown;
            }
            if (GetLines(lines, ref startIndex, out int endIndex, uniquesSectionStart, uniquesSectionEnd))
                GetFilterUniqueData(new ArraySegment<string>(lines, startIndex, endIndex - startIndex));

            startIndex = endIndex;
            if (GetLines(lines, ref startIndex, out endIndex, divSectionStart, divSectionEnd))
                GetFilterDivinationData(new ArraySegment<string>(lines, startIndex, endIndex - startIndex));
        }

        private void GetFilterDivinationData(IEnumerable<string> lines)
        {
            DivinationValue value;
            while (lines.Any()) {
                lines = lines.SkipWhile(aline => !aline.StartsWith("Show ") && !aline.StartsWith("Hide "));
                if (!lines.Any())
                    return;
                string line = lines.First();

                if (!line.Contains("# Divination Cards - "))
                    continue;

                if (line.Contains("10c+"))
                    value = DivinationValue.Chaos10;
                else if (line.Contains("1c+"))
                    value = DivinationValue.Chaos1to10;
                else if (line.Contains("<1c") || line.Contains("< 1c"))
                    value = DivinationValue.ChaosLess1;
                else if (line.Contains("Nearly Worthless"))
                    value = DivinationValue.NearlyWorthless;
                else if (line.Contains("Worthless"))
                    value = DivinationValue.Worthless;
                else {
                    if (!line.Contains("New (Error)"))
                        MessageBox.Show("Unexpected Divination input: " + line, "Error", MessageBoxButtons.OK);
                    lines = lines.Skip(1);
                    continue;
                }
                lines = lines.Skip(1).SkipWhile(aline => !aline.TrimStart().StartsWith("BaseType ") && !aline.StartsWith("Show ") && !aline.StartsWith("Hide "));
                line = lines.First().Trim();
                if (line.StartsWith("BaseType ")) {
                    List<string> baseTypes = GetBaseTypes(line.Substring(9));
                    FillFilterDivinationData(SC, baseTypes, value);
                    FillFilterDivinationData(HC, baseTypes, value);
                }
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
                line = lines.First().TrimStart();
                if (line.StartsWith("BaseType ")) {
                    List<string> baseTypes = GetBaseTypes(line.Substring(9));
                    FillFilterUniqueData(SC, baseTypes, value);
                    FillFilterUniqueData(HC, baseTypes, value);
                }
            }
        }

        private List<string> GetBaseTypes(string line)
        {
            MatchCollection collection = baseTypeRegex.Matches(line);
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

        private void FillFilterUniqueData(LeagueData data, List<string> baseTypes, UniqueValue value)
        {
            foreach (string baseTy in baseTypes) {
                if (!data.Uniques.TryGetValue(baseTy, out UniqueBaseType unique)) {
                    if (baseTy == "Maelstr") {
                        if (!data.Uniques.TryGetValue("Maelström Staff", out unique)) {
                            unique = new UniqueBaseType("Maelström Staff");
                            data.Uniques.Add("Maelström Staff", unique);
                        }
                    }
                    else {
                        unique = new UniqueBaseType(baseTy);
                        data.Uniques.Add(baseTy, unique);
                        MessageBox.Show("Filter: unknown basetype: " + unique.BaseType, "Error", MessageBoxButtons.OK);
                        Environment.Exit(1);
                    }
                }
                unique.FilterValue = value;
            }
        }

        private void FillFilterDivinationData(LeagueData data, List<string> baseTypes, DivinationValue value)
        {

            foreach (string baseTy in baseTypes) {
                if (!data.DivinationCards.TryGetValue(baseTy, out DivinationCard divCard)) {
                    divCard = new DivinationCard(baseTy);
                    data.DivinationCards.Add(baseTy, divCard);
                }
                divCard.FilterValue = value;
            }
        }

        private string ReadWebPage(string url, string headerMedia = "", Encoding encoding = null)
        {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(url);
                if (headerMedia.Length > 0)
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(headerMedia));
                HttpResponseMessage response;
                try {
                    response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode) {
                        return encoding == null ? response.Content.ReadAsStringAsync().Result
                            : encoding.GetString(response.Content.ReadAsByteArrayAsync().Result);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show("Error reading webpage " + url + "\n" + ex.Message, "Data.ReadWebPage", MessageBoxButtons.OK);
                    Environment.Exit(1);
                }
                return "";
            }
        }

    }
}
