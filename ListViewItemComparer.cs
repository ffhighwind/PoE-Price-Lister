using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoE_Price_Lister
{
    public class ListViewItemComparer : IComparer
    {
        private int column;
        private bool isNumeric = false;
        private bool ascending;

        public int Column {
            get { return column; }
            set { column = value; }
        }

        public bool Ascending {
            get { return ascending; }
            set { ascending = value; }
        }

        public bool IsNumeric {
            get { return isNumeric; }
            set { isNumeric = value; }
        }

        public ListViewItemComparer(int columnIndex)
        {
            column = columnIndex;
        }

        public int Compare(object x, object y)
        {
            ListViewItem itemX = x as ListViewItem;
            ListViewItem itemY = y as ListViewItem;

            int output;

            if (itemX == null && itemY == null)
                output = 0;
            else if (itemX == null)
                output = -1;
            else if (itemY == null)
                output = 1;
            else if (itemX == itemY)
                output = 0;
            else if (isNumeric) {
                decimal itemXVal, itemYVal;

                if (!Decimal.TryParse(itemX.SubItems[column].Text, out itemXVal)) {
                    itemXVal = 0;
                }
                if (!Decimal.TryParse(itemY.SubItems[column].Text, out itemYVal)) {
                    itemYVal = 0;
                }

                output = Decimal.Compare(itemXVal, itemYVal);
            }
            else {
                string itemXText = itemX.SubItems[column].Text;
                string itemYText = itemY.SubItems[column].Text;

                output = String.Compare(itemXText, itemYText);
            }

            if (ascending)
                output = -output;
            return output;
        }
    }
}
