using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoE_Price_Lister
{
	public partial class MainForm : Form
	{
		private readonly DataModel model = new DataModel();
		private readonly ListViewItemComparer sorter = new ListViewItemComparer(0);
		private static readonly string[] numericCols = new string[] { "Severity", "Value" };

		private readonly DataTable scUniquesTable = new DataTable();
		private readonly DataTable hcUniquesTable = new DataTable();
		private readonly DataTable scDivinationTable = new DataTable();
		private readonly DataTable hcDivinationTable = new DataTable();
		private readonly DataTable scEnchantsTable = new DataTable();
		private readonly DataTable hcEnchantsTable = new DataTable();

		public MainForm()
		{
			InitializeComponent();
			openFileDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\Resources";

			scUniquesTable.Columns.Add("BaseType");
			scUniquesTable.Columns.Add("Severity", typeof(int));
			scUniquesTable.Columns.Add("Filter", typeof(UniqueValue));
			scUniquesTable.Columns.Add("Expected", typeof(UniqueValue));
			scUniquesTable.Columns.Add("Min", typeof(float));
			scUniquesTable.Columns.Add("Max", typeof(float));
			scUniquesTable.Columns.Add("Uniques");

			hcUniquesTable.Columns.Add("BaseType");
			hcUniquesTable.Columns.Add("Severity", typeof(int));
			hcUniquesTable.Columns.Add("Filter", typeof(UniqueValue));
			hcUniquesTable.Columns.Add("Expected", typeof(UniqueValue));
			hcUniquesTable.Columns.Add("Max", typeof(float));
			hcUniquesTable.Columns.Add("Min", typeof(float));
			hcUniquesTable.Columns.Add("Uniques");

			scDivinationTable.Columns.Add("Divination Card");
			scDivinationTable.Columns.Add("Severity", typeof(int));
			scDivinationTable.Columns.Add("Filter", typeof(DivinationValue));
			scDivinationTable.Columns.Add("Expected", typeof(DivinationValue));
			scDivinationTable.Columns.Add("Value", typeof(float));

			hcDivinationTable.Columns.Add("Divination Card");
			hcDivinationTable.Columns.Add("Severity", typeof(int));
			hcDivinationTable.Columns.Add("Filter", typeof(DivinationValue));
			hcDivinationTable.Columns.Add("Expected", typeof(DivinationValue));
			hcDivinationTable.Columns.Add("Value", typeof(float));

			scEnchantsTable.Columns.Add("Gem");
			scEnchantsTable.Columns.Add("Enchantment");
			scEnchantsTable.Columns.Add("Severity", typeof(int));
			scEnchantsTable.Columns.Add("Filter", typeof(EnchantmentValue));
			scEnchantsTable.Columns.Add("Expected", typeof(EnchantmentValue));
			scEnchantsTable.Columns.Add("Value", typeof(float));
			scEnchantsTable.Columns.Add("Name");

			hcEnchantsTable.Columns.Add("Gem");
			hcEnchantsTable.Columns.Add("Enchantment");
			hcEnchantsTable.Columns.Add("Severity", typeof(int));
			hcEnchantsTable.Columns.Add("Filter", typeof(EnchantmentValue));
			hcEnchantsTable.Columns.Add("Expected", typeof(EnchantmentValue));
			hcEnchantsTable.Columns.Add("Value", typeof(float));
			hcEnchantsTable.Columns.Add("Name");

			divinationHcDataGridView.DataSource = hcDivinationTable;
			divinationScDataGridView.DataSource = scDivinationTable;
			uniquesScDataGridView.DataSource = scUniquesTable;
			uniquesHcDataGridView.DataSource = hcUniquesTable;
			enchantsHcDataGridView.DataSource = hcEnchantsTable;
			enchantsScDataGridView.DataSource = scEnchantsTable;

			divinationHcDataGridView.DoubleBuffer();
			divinationScDataGridView.DoubleBuffer();
			uniquesScDataGridView.DoubleBuffer();
			uniquesHcDataGridView.DoubleBuffer();
			enchantsHcDataGridView.DoubleBuffer();
			enchantsScDataGridView.DoubleBuffer();
		}

		private void buttonGenFilter_Click(object sender, EventArgs e)
		{
			LeagueData l1, l2;
			if (hcFriendlyRadioButton.Checked) {
				l1 = model.SC;
				l2 = model.HC;
			}
			else if (scRadioButton.Checked) {
				l1 = model.SC;
				l2 = model.SC;
			}
			else {
				l1 = model.HC;
				l2 = model.HC;
			}
			FilterWriter writer = new FilterWriter(model, l1, l2);
			string[] filterFiles = new string[] {
				"S1_Regular_Highwind.filter",
				"S2_Mapping_Highwind.filter",
				"S3_Semi_Strict_Highwind.filter",
				"S4_Strict_Highwind.filter",
				"S5_Very_Strict_Highwind.filter"
			};
			string[] largeFiles = new string[] {
				"L1_Regular_Highwind.filter",
				"L2_Mapping_Highwind.filter",
				"L3_Semi_Strict_Highwind.filter",
				"L4_Strict_Highwind.filter",
				"L5_Very_Strict_Highwind.filter"
			};
			FilterType[] filterTypes = new FilterType[] {
				FilterType.LEVELING,
				FilterType.MAPPING,
				FilterType.SEMI_STRICT,
				FilterType.STRICT,
				FilterType.VERY_STRICT
			};
			for (int i = 0; i < filterFiles.Length; i++) {
				writer.Create(filterTypes[i], filterFiles[i]);
				string filterData = File.ReadAllText(filterFiles[i]);
				filterData = filterData.Replace("SetFontSize 40", "SetFontSize 45");
				filterData = filterData.Replace("SetFontSize 36", "SetFontSize 40");
				filterData = filterData.Replace("SetFontSize 32", "SetFontSize 36");
				using (StreamWriter lwriter = File.CreateText(largeFiles[i])) {
					lwriter.Write(filterData);
				}
			}
			model.Load(filterFiles[1]);
			LoadDataGridViews();
		}

		/*
		private void listView_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C) {
				ListView listView = (ListView) sender;
				StringBuilder sb = new StringBuilder();
				if (listView.SelectedItems.Count == 1)
					sb.Append(listView.SelectedItems[0].Text);
				else {
					foreach (ListViewItem item in listView.SelectedItems) {
						if (item.Text.Contains(" "))
							sb.Append('\"').Append(item.Text).Append('\"');
						else
							sb.Append(item.Text);
						sb.Append(' ');
					}
				}
				Clipboard.SetText(sb.ToString());
			}
		}
		*/


		/*
		private void FillUniqueListView(ListView lv, LeagueData data)
		{
			lv.BeginUpdate();
			lv.Items.Clear();
			foreach (string baseType in model.Uniques) {
				if (!data.Uniques.TryGetValue(baseType, out UniqueBaseType uniqData))
					continue;
				string values = "";
				foreach (UniqueItem udata in uniqData.OrderedItems) {
					if (udata.Links > 4)
						values += "(" + udata.Links + "L)";
					string value = udata.Count > 0 ? udata.ChaosValue.ToString() : "?";
					values += udata.Name + ": " + value + ", ";
				}
				UniqueValue expect = uniqData.ExpectedFilterValue;
				string severity = uniqData.SeverityLevel.ToString();
				string filterVal = uniqData.FilterValue.ToString();
				string expectVal = expect.Value == uniqData.FilterValue.Value ? "" : expect.ToString();
				string listedVals = values.Substring(0, values.Length - 2);
				lv.Items.Add(new ListViewItem(new string[] { baseType, severity, filterVal, expectVal, listedVals }));
			}
			lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			lv.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			lv.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			lv.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			lv.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			lv.Columns[4].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			lv.EndUpdate();
		}

		private void FillDivinationListView(ListView lv, LeagueData data)
		{
			lv.BeginUpdate();
			lv.Items.Clear();
			foreach (string div in model.DivinationCards) {
				if (!data.DivinationCards.TryGetValue(div, out DivinationCard divCard))
					continue;
				DivinationValue expect = divCard.ExpectedFilterValue;
				string severity = divCard.SeverityLevel.ToString();
				string filterVal = divCard.FilterValue.ToString();
				string expectVal = expect.Value == divCard.FilterValue.Value ? "" : expect.ToString();
				string listedVal = divCard.ChaosValue < 0.0f ? "?" : divCard.ChaosValue.ToString();
				lv.Items.Add(new ListViewItem(new string[] { div, severity, filterVal, expectVal, listedVal }));
			}
			lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			lv.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			lv.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			lv.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			lv.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			lv.Columns[4].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			lv.EndUpdate();
		}

		private void FillEnchantsListView(ListView lv, LeagueData data)
		{
			lv.BeginUpdate();
			lv.Items.Clear();
			foreach (string ench in model.Enchantments) {
				if (!data.Enchantments.TryGetValue(ench, out Enchantment enchantment))
					continue;
				string severity = enchantment.SeverityLevel.ToString();
				lv.Items.Add(new ListViewItem(new string[] { ench, severity, enchantment.Gem, "0", enchantment.Value.ToString() }));
			}
			lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			lv.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			lv.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			lv.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			lv.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			lv.Columns[4].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
			lv.EndUpdate();
		}

		private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			ListView lv = (ListView) sender;

			if (!(lv.ListViewItemSorter is ListViewItemComparer sorter)) {
				sorter = new ListViewItemComparer(e.Column);
				lv.ListViewItemSorter = sorter;
			}
			string text = lv.Columns[e.Column].Text;
			sorter.Ascending = sorter.Column == e.Column ? !sorter.Ascending : text == "Severity";
			sorter.Column = e.Column;
			sorter.IsNumeric = numericCols.Contains(text);

			lv.Sort();
		}
		*/

		private void buttonLoad_Click(object sender, EventArgs e)
		{
			if (DialogResult.OK == openFileDialog.ShowDialog()) {
				model.Load(openFileDialog.FileName);
				LoadDataGridViews();
			}
		}

		private async void Form1_Shown(object sender, EventArgs e)
		{
			buttonGenFilter.Enabled = false;
			buttonLoad.Enabled = false;
			await Task.Run(
			() =>
			{
				model.Load();
			});
			LoadDataGridViews();
			buttonGenFilter.Enabled = true;
			buttonLoad.Enabled = true;
			Text += " (v" + model.Version + ")";

			string errors = model.GetErrorsString();
			if (errors.Length > 0) {
				Invoke(new Action(() => {
					Clipboard.SetText(errors);
				}));
			}
		}

		private void LoadDataGridViews()
		{
			SuspendLayout();
			FillUniques(scUniquesTable, model.SC);
			FillDivination(scDivinationTable, model.SC);
			FillUniques(hcUniquesTable, model.HC);
			FillDivination(hcDivinationTable, model.HC);
			FillEnchants(scEnchantsTable, model.SC);
			FillEnchants(hcEnchantsTable, model.HC);
			SortDgv(uniquesScDataGridView);
			SortDgv(uniquesHcDataGridView);
			SortDgv(divinationScDataGridView);
			SortDgv(divinationHcDataGridView);
			SortDgv(enchantsScDataGridView);
			SortDgv(enchantsHcDataGridView);
			ResumeLayout(true);
		}

		private static void SortDgv(DataGridView dgv)
		{
			dgv.Sort(dgv.Columns["Severity"], System.ComponentModel.ListSortDirection.Descending);
			dgv.FirstDisplayedScrollingRowIndex = 0;
		}

		private void FillUniques(DataTable table, LeagueData data)
		{
			table.Clear();
			foreach (UniqueBaseType unique in data.Uniques.Values) {
				DataRow row = table.NewRow();
				row["BaseType"] = unique.BaseType;
				row["Severity"] = unique.SeverityLevel;
				row["Filter"] = unique.FilterValue;
				row["Expected"] = unique.ExpectedFilterValue; // expect.Value == unique.FilterValue.Value ? "" : expect.ToString();
				row["Min"] = (object) unique.MinValue ?? DBNull.Value;
				row["Max"] = (object) unique.MaxValue ?? DBNull.Value;
				row["Uniques"] = unique.GetString();
				table.Rows.Add(row);
			}
		}

		private void FillDivination(DataTable table, LeagueData data)
		{
			table.Clear();
			foreach (DivinationCard divCard in data.DivinationCards.Values) {
				DataRow row = table.NewRow();
				row["Divination Card"] = divCard.Name;
				row["Severity"] = divCard.SeverityLevel;
				row["Filter"] = divCard.FilterValue;
				row["Expected"] = divCard.ExpectedFilterValue; // expect.Value == divCard.FilterValue.Value ? "" : expect.ToString();
				row["Value"] = divCard.ChaosValue < 0.0f ? DBNull.Value : (object) divCard.ChaosValue;
				table.Rows.Add(row);
			}
		}

		private void FillEnchants(DataTable table, LeagueData data)
		{
			table.Clear();
			foreach (Enchantment ench in data.Enchantments.Values) {
				DataRow row = table.NewRow();
				row["Gem"] = ench.Gem;
				row["Enchantment"] = ench.Description;
				row["Severity"] = ench.SeverityLevel;
				row["Filter"] = ench.FilterValue;
				row["Expected"] = ench.ExpectedFilterValue; // expect.Value == ench.FilterValue.Value ? "" : expect.ToString();
				row["Value"] = ench.ChaosValue < 0.0f ? DBNull.Value : (object) ench.ChaosValue;
				row["Name"] = ench.Name;
				table.Rows.Add(row);
			}
		}

		private void scRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (scRadioButton.Checked) {
				hcRadioButton.Checked = false;
				hcFriendlyRadioButton.Checked = false;
			}
		}

		private void hcRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (hcRadioButton.Checked) {
				scRadioButton.Checked = false;
				hcFriendlyRadioButton.Checked = false;
			}
		}

		private void hcFriendlyRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (hcFriendlyRadioButton.Checked) {
				scRadioButton.Checked = false;
				hcRadioButton.Checked = false;
			}
		}
	}
}
