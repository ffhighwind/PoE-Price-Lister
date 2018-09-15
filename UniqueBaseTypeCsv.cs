
using FileHelpers;
using Newtonsoft.Json;

namespace PoE_Price_Lister
{
    [DelimitedRecord(",")]
    [IgnoreFirst(1)]
    public class UniqueBaseTypeCsv
    {
        public string BaseType { get; set; }

        [FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.AllowForRead)]
        public string Name { get; set; }

        public string League { get; set; }

        [FieldConverter(typeof(UniqueUsageConverter))]
        [FieldNullValue(UniqueUsage.None)]
        public UniqueUsage Usage { get; set; }

        [FieldConverter(typeof(BoolConverter))]
        [FieldNullValue(false)]
        public bool Unobtainable { get; set; }

        public string Source { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented).ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            UniqueBaseTypeCsv other = (UniqueBaseTypeCsv) obj;
            return other.BaseType == BaseType && other.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() * BaseType.GetHashCode();
        }
    }

    internal class BoolConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            return from != null;
        }

        public override string FieldToString(object fieldValue)
        {
            return ((bool) fieldValue).ToString();
        }
    }

    internal class UniqueUsageConverter : ConverterBase
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
            return ((UniqueUsage) fieldValue).ToString();
        }
    }
}
