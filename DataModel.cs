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
using System.Windows.Forms;
using FileHelpers;
using Newtonsoft.Json.Linq;

namespace PoE_Price_Lister
{
	public class DataModel
	{
		// All URLs for poe.ninja/api are here:
		// https://github.com/5k-mirrors/misc-poe-tools/blob/master/doc/poe-ninja-api.md

		// Alternate API (POE watch)
		// https://api.poe.watch/get?league=Metamorph&category=armour
		// https://api.poe.watch/compact?league=Metamorph&category=armour

		////private const string csvFile = "poe_uniques.csv";
		private const string league = "Delirium";

		private const string repoURL = @"https://raw.githubusercontent.com/ffhighwind/PoE-Price-Lister/master/";
		public const string FiltersUrl = repoURL + @"Resources/Filters/";
		private const string filterFile = @"S1_Regular_Highwind.filter";
		private const string uniquesCsvFile = "poe_uniques.csv";
		private const string helmEnchantCsvFile = "poe_helm_enchants.csv";
		//private const string jsonURL = @"http://poe.ninja/api/Data/Get{0}Overview?league={1}";
		private const string jsonURL = @"https://poe.ninja/api/data/itemoverview?league={1}&type={0}";
		//ALTERNATE API? https://poe.watch/prices?category=enchantment&league=Metamorph

		//{0} = "UniqueAccessory", "UniqueFlask", "UniqueArmour", "UniqueWeapon", "UniqueJewel", "UniqueMap"
		// "DivinationCard", "HelmetEnchant", "Fragment", "Prophecy", "Essence", "SkillGem", "Beast"
		// "Fossil", "Resonator", "Scarab", "Incubator", "Oil", "Prophecy", "BaseType"
		// currencyoverview? "Fragment", "Currency"
		private static readonly Regex quotedListRegex = new Regex(@"""[^""\r\n]+""|[^ \r\n]+", RegexOptions.Compiled);
		private static readonly Regex versionRegex = new Regex(@".+(\d+)[.](\d+)[.](\d+) [^\r\n]+", RegexOptions.Compiled);

		public int VersionMajor { get; private set; }
		public int VersionMinor { get; private set; }
		public int VersionRelease { get; private set; }
		public string Version { get; private set; }

		public LeagueData HC { get; private set; } = new LeagueData(true);
		public LeagueData SC { get; private set; } = new LeagueData(false);

		public IReadOnlyList<string> Uniques { get; private set; }
		public IReadOnlyList<string> DivinationCards { get; private set; }
		public IReadOnlyList<string> Enchantments { get; private set; }

		private const int MaxErrors = 5;
		private int ErrorCount = 0;

		private List<IReadOnlyList<string>> conflicts { get; } = new List<IReadOnlyList<string>>();
		public IReadOnlyList<IReadOnlyList<string>> DivinationCardNameConflicts => conflicts;

		public void Load()
		{
			ErrorCount = 0;
			LoadUniquesCsv();
			LoadEnchantsCsv();
			GetJsonData(HC);
			GetJsonData(SC);
			string filterString = Util.ReadWebPage(FiltersUrl + filterFile);
			if (filterString.Length == 0) {
				MessageBox.Show("Failed to read the web URL: " + FiltersUrl + filterFile, "Error", MessageBoxButtons.OK);
				Environment.Exit(1);
			}
			Load(filterString.Split('\n'));

			Match m = versionRegex.Match(filterString);
			VersionMajor = int.Parse(m.Groups[1].Value);
			VersionMinor = int.Parse(m.Groups[2].Value);
			VersionRelease = int.Parse(m.Groups[3].Value);
			Version = VersionMajor.ToString() + "." + VersionMinor + "." + VersionRelease;
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
			ErrorCount = 0;
			//HC.ClearFilterValues();
			//SC.ClearFilterValues();
			GetFilterData(lines);

			DivinationCards = SC.DivinationCards.Keys.OrderBy(x => x).ToList();
			GetDivinationCardConflicts();

			List<string> uniquesToRemove = SC.Uniques.Keys.Where(uniq => uniq.EndsWith(" Piece") || uniq.EndsWith(" Talisman")).ToList();
			foreach (string uniq in uniquesToRemove) {
				SC.Uniques.Remove(uniq);
				HC.Uniques.Remove(uniq);
			}
			List<Enchantment> enchantsToRemove = SC.Enchantments.Values.Where(x => x.Source == EnchantmentSource.BlightOils).ToList();
			foreach (Enchantment ench in enchantsToRemove) {
				SC.Enchantments.Remove(ench.Name);
				SC.EnchantmentsDescriptions.Remove(ench.Description);
				HC.Enchantments.Remove(ench.Name);
				HC.EnchantmentsDescriptions.Remove(ench.Description);
			}
			Uniques = SC.Uniques.Keys.OrderBy(x => x).ToList();
			Enchantments = SC.Enchantments.Keys.OrderBy(x => x).ToList();
		}

		private void GetJsonData(LeagueData data)
		{
			string leagueStr = data.IsHardcore ? "Hardcore " + league : league;
			FillJsonData(string.Format(jsonURL, "DivinationCard", leagueStr), data, DivinationJsonHandler);
			FillJsonData(string.Format(jsonURL, "UniqueArmour", leagueStr), data, UniqueJsonHandler);
			FillJsonData(string.Format(jsonURL, "UniqueFlask", leagueStr), data, UniqueJsonHandler);
			FillJsonData(string.Format(jsonURL, "UniqueWeapon", leagueStr), data, UniqueJsonHandler);
			FillJsonData(string.Format(jsonURL, "UniqueAccessory", leagueStr), data, UniqueJsonHandler);
			FillJsonData(string.Format(jsonURL, "HelmetEnchant", leagueStr), data, EnchantJsonHandler);
		}

		private void FillJsonData(string url, LeagueData data, Action<JsonData, LeagueData> handler)
		{
			string jsonURLString = Util.ReadWebPage(url, "application/json");
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

		private void EnchantJsonHandler(JsonData jdata, LeagueData data)
		{
			string description = jdata.Name.Trim();
			if (!data.EnchantmentsDescriptions.TryGetValue(description, out Enchantment ench)) {
				ench = new Enchantment(description);
				data.EnchantmentsDescriptions.Add(description, ench);
				MessageBox.Show("JSON: The CSV file is missing Enchantment: " + description, "Error", MessageBoxButtons.OK);
				ErrorCount++;
			}
			ench.Load(jdata);
			if (ErrorCount > MaxErrors) {
				Environment.Exit(1);
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

		private void LoadUniquesCsv()
		{
			FileHelperEngine<UniqueBaseTypeCsv> engine = new FileHelperEngine<UniqueBaseTypeCsv>(Encoding.UTF8);
			string csvText = new FileInfo(uniquesCsvFile).Exists ?
				File.ReadAllText("poe_uniques.csv", Encoding.UTF8)
				: Util.ReadWebPage(repoURL + "poe_uniques.csv", "", Encoding.UTF8);
			UniqueBaseTypeCsv[] records = engine.ReadString(csvText);
			foreach (UniqueBaseTypeCsv csvdata in records) {
				if (!SC.Uniques.ContainsKey(csvdata.BaseType)) {
					SC.Uniques[csvdata.BaseType] = new UniqueBaseType(csvdata.BaseType);
					HC.Uniques[csvdata.BaseType] = new UniqueBaseType(csvdata.BaseType);
				}
				SC.Uniques[csvdata.BaseType].Load(csvdata);
				HC.Uniques[csvdata.BaseType].Load(csvdata);
			}
			foreach (string baseType in SC.Uniques.Keys) {
				Sort(SC, baseType);
				Sort(HC, baseType);
			}
		}

		private void LoadEnchantsCsv()
		{
			FileHelperEngine<EnchantCsv> engine = new FileHelperEngine<EnchantCsv>(Encoding.UTF8);
			string csvText = new FileInfo(helmEnchantCsvFile).Exists ?
				File.ReadAllText(helmEnchantCsvFile, Encoding.UTF8)
				: Util.ReadWebPage(repoURL + helmEnchantCsvFile, "", Encoding.UTF8);
			EnchantCsv[] records = engine.ReadString(csvText);
			foreach (EnchantCsv csvdata in records) {
				if (!SC.Enchantments.ContainsKey(csvdata.Description)) {
					Enchantment scData = new Enchantment(csvdata.Name);
					SC.Enchantments.Add(csvdata.Name, scData);
					SC.EnchantmentsDescriptions.Add(csvdata.Description, scData);
					Enchantment hcData = new Enchantment(csvdata.Name);
					HC.Enchantments.Add(csvdata.Name, hcData);
					HC.EnchantmentsDescriptions.Add(csvdata.Description, hcData);
				}
				SC.Enchantments[csvdata.Name].Load(csvdata);
				HC.Enchantments[csvdata.Name].Load(csvdata);
			}
		}

		private void Sort(LeagueData data, string baseType)
		{
			data.Uniques[baseType].Sort();
		}

		private void GetDivinationCardConflicts()
		{
			List<string> conflictsList = new List<string>();
			conflicts.Clear();
			for (int i = 0; i < DivinationCards.Count; i++) {
				string divBaseTy = DivinationCards[i].ToLower();
				for (int j = i + 1; j < DivinationCards.Count; j++) {
					if (DivinationCards[i].IndexOf(DivinationCards[j], StringComparison.OrdinalIgnoreCase) >= 0
							|| DivinationCards[j].IndexOf(DivinationCards[i], StringComparison.OrdinalIgnoreCase) >= 0)
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
			SC.ClearFilterValues();
			HC.ClearFilterValues();
			int startIndex = 0;
			int endIndex;
			if (GetLines(lines, ref startIndex, out endIndex, "# Section: Enchantments", "######"))
				GetFilterEnchantsData(new ArraySegment<string>(lines, startIndex, endIndex - startIndex));
			startIndex = 0;
			if (GetLines(lines, ref startIndex, out endIndex, "# Section: Divination Cards", "######"))
				GetFilterDivinationData(new ArraySegment<string>(lines, startIndex, endIndex - startIndex));
			startIndex = 0;
			if (GetLines(lines, ref startIndex, out endIndex, "# Section: Uniques", "######"))
				GetFilterUniqueData(new ArraySegment<string>(lines, startIndex, endIndex - startIndex));
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
				else if (line.Contains("10c+"))
					value = DivinationValue.Chaos10;
				else if (line.Contains("2-10c"))
					value = DivinationValue.Chaos2to10;
				else if (line.Contains("<2c") || line.Contains("< 2c"))
					value = DivinationValue.ChaosLess2;
				else if (line.Contains("Nearly Worthless"))
					value = DivinationValue.NearlyWorthless;
				else if (line.Contains("Worthless"))
					value = DivinationValue.Worthless;
				else {
					lines = lines.Skip(1);
					continue;
				}
				lines = lines.Skip(1).SkipWhile(aline => !aline.TrimStart().StartsWith("BaseType ") && !aline.StartsWith("Show ") && !aline.StartsWith("Hide "));
				line = lines.First().Trim();
				if (line.StartsWith("BaseType ")) {
					IEnumerable<string> baseTypes = SplitQuotedList(line).Skip(1);
					FillFilterDivinationData(SC, baseTypes, value);
					FillFilterDivinationData(HC, baseTypes, value);
				}
			}
		}

		private void GetFilterEnchantsData(IEnumerable<string> lines)
		{
			EnchantmentValue value = null;
			while (lines.Any()) {
				lines = lines.SkipWhile(aline => !aline.StartsWith("Show ") && !aline.StartsWith("Hide "));
				if (!lines.Any())
					return;
				string line = lines.First();
				if (!line.Contains("# Enchantments -") || line.Contains("Unique")) {
					lines = lines.Skip(1);
					continue;
				}
				if (line.Contains("20c+"))
					value = EnchantmentValue.Chaos20;
				else if (line.Contains("10c+"))
					value = EnchantmentValue.Chaos10;
				else {
					//if (!line.Contains("Other"))
					//	MessageBox.Show("Unexpected Enchant input: " + line, "Error", MessageBoxButtons.OK);
					lines = lines.Skip(1);
					continue;
				}
				lines = lines.Skip(1).SkipWhile(aline => !aline.TrimStart().StartsWith("HasEnchantment ") && !aline.StartsWith("Show ") && !aline.StartsWith("Hide "));
				line = lines.First().TrimStart();
				if (line.StartsWith("HasEnchantment ")) {
					IEnumerable<string> enchantTypes = SplitQuotedList(line).Skip(1);
					FillFilterEnchantData(SC, enchantTypes, value);
					FillFilterEnchantData(HC, enchantTypes, value);
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

				if (!line.Contains("# Uniques -")) { 
					lines = lines.Skip(1);
					continue;
				}

				if (line.Contains("15c+"))
					value = UniqueValue.Chaos15;
				else if (line.Contains("5-15c"))
					value = UniqueValue.Chaos5to15;
				else if (line.Contains("3-5c"))
					value = UniqueValue.Chaos3to5;
				else if (line.Contains("Limited"))
					value = UniqueValue.Limited;
				else {
					//(line.Contains("<67")
					lines = lines.Skip(1);
					continue;
				}
				lines = lines.Skip(1).SkipWhile(aline => !aline.TrimStart().StartsWith("BaseType ") && !aline.StartsWith("Show ") && !aline.StartsWith("Hide "));
				line = lines.First().TrimStart();
				if (line.StartsWith("BaseType ")) {
					IEnumerable<string> baseTypes = SplitQuotedList(line).Skip(1);
					FillFilterUniqueData(SC, baseTypes, value);
					FillFilterUniqueData(HC, baseTypes, value);
				}
			}
		}

		private List<string> SplitQuotedList(string line)
		{
			MatchCollection collection = quotedListRegex.Matches(line);
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

		private void FillFilterUniqueData(LeagueData data, IEnumerable<string> baseTypes, UniqueValue value)
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

		private void FillFilterEnchantData(LeagueData data, IEnumerable<string> enchantTypes, EnchantmentValue value)
		{
			foreach (string enchType in enchantTypes) {
				if (!data.Enchantments.TryGetValue(enchType, out Enchantment ench)) {
					ench = new Enchantment(enchType);
					data.Enchantments.Add(enchType, ench);
				}
				ench.FilterValue = value;
			}
		}

		private void FillFilterDivinationData(LeagueData data, IEnumerable<string> baseTypes, DivinationValue value)
		{
			foreach (string baseTy in baseTypes) {
				if (!data.DivinationCards.TryGetValue(baseTy, out DivinationCard divCard)) {
					divCard = new DivinationCard(baseTy);
					data.DivinationCards.Add(baseTy, divCard);
				}
				divCard.FilterValue = value;
			}
		}
	}
}
