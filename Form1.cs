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
            data.GetData();
            FillUniqueListView();
            FillDivinationListView();
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
        }

        void FillUniqueListView()
        {
            listViewUniques.BeginUpdate();
            listViewUniques.Items.Clear();
            IEnumerable<string> uniquesList = data.GetUniques();
            foreach (string baseType in uniquesList)
            {
                var uniqData = data.GetUniqueEntry(baseType);
                string values = "";
                foreach (UniqueData udata in uniqData.Items)
                {
                    if (udata.Links > 4)
                        values += "(" + udata.Links + "L)";
                    string value = udata.Count > 0 ? udata.ChaosValue.ToString() : "?";
                    values += udata.Name + ": " + value + ", ";
                }
                var expect = uniqData.ExpectedFilterValue;
                string severity = uniqData.SeverityLevel.ToString();
                string filterVal = uniqData.FilterValue.ToString();
                string expectVal = uniqData.ExpectedFilterValue.ToString();
                string listedVals = values.Substring(0, values.Length - 2);
                listViewUniques.Items.Add(new ListViewItem(new string[] { baseType, severity, filterVal, expectVal, listedVals }));
            }
            listViewUniques.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            columnUniqBaseType.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            columnUniqFilter.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            columnUniqExpect.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            columnUniqSeverity.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            columnUniqItems.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewUniques.EndUpdate();
        }

        void FillDivinationListView()
        {
            listViewDiv.BeginUpdate();
            listViewDiv.Items.Clear();
            IEnumerable<string> divinationList = data.GetDivinationCards();
            foreach (string div in divinationList)
            {
                var divData = data.GetDivinationEntry(div);
                var expect = divData.ExpectedFilterValue;
                string severity = divData.SeverityLevel.ToString();
                string filterVal = divData.FilterValue.ToString();
                string expectVal = expect.ToString();
                string listedVal = divData.ChaosValue < 0.0f ? "?" : divData.ChaosValue.ToString();
                listViewDiv.Items.Add(new ListViewItem(new string[] { div, severity, filterVal, expectVal, listedVal }));
            }
            listViewDiv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            columnDivName.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            columnDivFilter.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            columnDivExpect.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            columnDivValue.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            columnDivSeverity.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewDiv.EndUpdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listViewUniques.Show();
            listViewUniques.BringToFront();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listViewDiv.Show();
            listViewDiv.BringToFront();
        }

        private void listViewUniques_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewItemComparer sorter = listViewUniques.ListViewItemSorter as ListViewItemComparer;

            if (sorter == null)
            {
                sorter = new ListViewItemComparer(e.Column);
                listViewUniques.ListViewItemSorter = sorter;
            }
            else
                sorter.Column = e.Column;

            sorter.Ascending = listViewUniques.Columns[e.Column].Text == "Severity";

            listViewUniques.Sort();
        }

        private void listViewDiv_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewItemComparer sorter = listViewDiv.ListViewItemSorter as ListViewItemComparer;

            if (sorter == null)
            {
                sorter = new ListViewItemComparer(e.Column);
                listViewDiv.ListViewItemSorter = sorter;
            }
            else
                sorter.Column = e.Column;

            sorter.Ascending = listViewDiv.Columns[e.Column].Text == "Severity";

            listViewDiv.Sort();
        }

        private void buttonGenFilter_Click(object sender, EventArgs e)
        {
            bool safe = checkBox1.Checked;
            string filename = safe ? "highwind_filter_safe.txt" : "highwind_filter.txt";
            data.GenerateFilterFile(filename, safe);
        }

        private void listView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
            {
                var listView = (ListView)sender;
                StringBuilder sb = new StringBuilder();
                if (listView.SelectedItems.Count == 1)
                    sb.Append(listView.SelectedItems[0].Text);
                else
                {
                    foreach (ListViewItem item in listView.SelectedItems)
                    {
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
            if(DialogResult.OK == openFileDialog1.ShowDialog())
            {
                data.Load(openFileDialog1.FileName);

                FillDivinationListView();
                FillUniqueListView();
            }
        }
    }
}
