using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileHelpers;
using Newtonsoft.Json;

namespace PoE_Price_Lister
{
    [DelimitedRecord(",")]
    [IgnoreFirst(1)]
    public class UniqueCsvData
    {
        string baseType;
        [FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.AllowForRead)]
        string name;
        string league;
        [FieldConverter(typeof(UniqueUsageConverter))]
        [FieldNullValue(UniqueUsage.None)]
        UniqueUsage usage;
        [FieldConverter(typeof(BoolConverter))]
        [FieldNullValue(false)]
        bool unobtainable;
        string source;

        public string BaseType
        {
            get { return baseType; }
            set { baseType = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string League
        {
            get { return league; }
            set { league = value; }
        }

        public bool Unobtainable
        {
            get { return unobtainable; }
            set { unobtainable = value; }
        }

        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        public UniqueUsage Usage
        {
            get { return usage; }
            set { usage = value; }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented).ToString();
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            UniqueCsvData other = (UniqueCsvData)obj;
            return other.BaseType == this.baseType && other.Name == name;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode() * baseType.GetHashCode();
        }
    }

    public enum UniqueUsage
    {
        None, Prophecy, Recipe, Upgradable, Piece
    }

    public class BoolConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            return from != null;
        }

        public override string FieldToString(object fieldValue)
        {
            return ((bool)fieldValue).ToString();
        }
    }

    public class UniqueUsageConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            UniqueUsage output;
            if (from.Equals("Prophecy"))
                output = UniqueUsage.Prophecy;
            else if (from.Equals("Recipe"))
                output = UniqueUsage.Recipe;
            else if (from.Equals("Upgradable"))
                output = UniqueUsage.Upgradable;
            else if (from.Equals("Piece"))
                output = UniqueUsage.Piece;
            else
                output = UniqueUsage.None;
            return output;
        }

        public override string FieldToString(object fieldValue)
        {
            return ((UniqueUsage)fieldValue).ToString();
        }
    }
}
