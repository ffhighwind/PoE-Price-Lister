using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoE_Price_Lister
{
    public partial class Form1 : Form
    {
        private DataModel model;
        private ListViewItemComparer sorter = new ListViewItemComparer(0);
        private static readonly string[] numericCols = new string[] { "Severity", "Value" };

        public Form1()
        {
            InitializeComponent();
            model = new DataModel();
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + "\\Resources";
        }

        private void FillUniqueListView(ListView lv, LeagueData data)
        {
            lv.BeginUpdate();
            lv.Items.Clear();
            foreach (string baseType in model.Uniques) {
                if (!data.Uniques.TryGetValue(baseType, out UniqueBaseType uniqData))
                    continue;
                string values = "";
                foreach (UniqueItem udata in uniqData.Items) {
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

        private void buttonGenFilter_Click(object sender, EventArgs e)
        {
            bool safe = checkBox1.Checked;
            bool hcFriendly = checkBox2.Checked;
            string filename = safe ? "highwind_filter_safe.txt" : "highwind_filter.txt";
            FilterWriter writer = new FilterWriter(model);
            writer.Create(filename, safe, hcFriendly);
        }

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

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog()) {
                model.Load(openFileDialog1.FileName);

                LoadListViews();
            }
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            await Task.Run(
            () => {
                model.Load();
            });
            LoadListViews();
        }

        private void LoadListViews()
        {
            FillUniqueListView(listViewUniques, model.SC);
            FillDivinationListView(listViewDivination, model.SC);
            FillUniqueListView(listViewUniquesHC, model.HC);
            FillDivinationListView(listViewDivinationHC, model.HC);
        }
    }
}
