using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace PoE_Price_Lister
{
    public partial class Form1 : Form
    {
        Data data;

        public Form1()
        {
            InitializeComponent();
            data = new Data();
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + "\\Resources";
        }

        void FillUniqueListView(ListView lv, Func<string, UniqueBaseEntry> getEntry)
        {
            lv.BeginUpdate();
            lv.Items.Clear();
            IEnumerable<string> uniquesList = data.GetUniques();
            foreach (string baseType in uniquesList) {
                var uniqData = getEntry(baseType);
                string values = "";
                foreach (UniqueData udata in uniqData.Items) {
                    if (udata.Links > 4)
                        values += "(" + udata.Links + "L)";
                    string value = udata.Count > 0 ? udata.ChaosValue.ToString() : "?";
                    values += udata.Name + ": " + value + ", ";
                }
                var expect = uniqData.ExpectedFilterValue;
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

        void FillDivinationListView(ListView lv, Func<string, DivinationData> getEntry)
        {
            lv.BeginUpdate();
            lv.Items.Clear();
            IEnumerable<string> divinationList = data.GetDivinationCards();
            foreach (string div in divinationList) {
                var divData = getEntry(div);
                var expect = divData.ExpectedFilterValue;
                string severity = divData.SeverityLevel.ToString();
                string filterVal = divData.FilterValue.ToString();
                string expectVal = expect.Value == divData.FilterValue.Value ? "" : expect.ToString();
                string listedVal = divData.ChaosValue < 0.0f ? "?" : divData.ChaosValue.ToString();
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

        private void listViewUniques_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListView lv = (ListView)sender;
            ListViewItemComparer sorter = lv.ListViewItemSorter as ListViewItemComparer;

            if (sorter == null) {
                sorter = new ListViewItemComparer(e.Column);
                lv.ListViewItemSorter = sorter;
            }
            else
                sorter.Column = e.Column;

            sorter.Ascending = lv.Columns[e.Column].Text == "Severity";

            lv.Sort();
        }

        private void listViewDiv_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListView lv = (ListView)sender;
            ListViewItemComparer sorter = lv.ListViewItemSorter as ListViewItemComparer;

            if (sorter == null) {
                sorter = new ListViewItemComparer(e.Column);
                lv.ListViewItemSorter = sorter;
            }
            else
                sorter.Column = e.Column;

            sorter.Ascending = lv.Columns[e.Column].Text == "Severity";

            lv.Sort();
        }

        private void buttonGenFilter_Click(object sender, EventArgs e)
        {
            bool safe = checkBox1.Checked;
            string filename = safe ? "highwind_filter_safe.txt" : "highwind_filter.txt";
            data.GenerateFilterFile(filename, safe);
        }

        private void listView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C) {
                var listView = (ListView)sender;
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
                data.Load(openFileDialog1.FileName);

                FillDivinationListView(listViewDivination, data.GetDivinationEntrySC);
                FillUniqueListView(listViewUniques, data.GetUniqueEntrySC);
                FillDivinationListView(listViewDivinationHC, data.GetDivinationEntryHC);
                FillUniqueListView(listViewUniquesHC, data.GetUniqueEntryHC);
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            data.GetData();
            FillUniqueListView(listViewUniques, data.GetUniqueEntrySC);
            FillDivinationListView(listViewDivination, data.GetDivinationEntrySC);
            FillUniqueListView(listViewUniquesHC, data.GetUniqueEntryHC);
            FillDivinationListView(listViewDivinationHC, data.GetDivinationEntryHC);
        }
    }
}
