using System;

namespace PoE_Price_Lister
{
    public class UniqueValue : IComparable<UniqueValue>
    {
        private UniqueValue(UniqueValueEnum value, int tier, string toString, float highValue, float lowValue)
        {
            Value = value;
            Tier = tier;
            this.toString = toString;
            HighValue = highValue;
            LowValue = lowValue;
        }

        public static readonly UniqueValue Error = new UniqueValue(UniqueValueEnum.Error, -1, "Error", -1.0f, float.MinValue);
        public static readonly UniqueValue Unknown = new UniqueValue(UniqueValueEnum.Unknown, 0, "Unknown", 0.95f, -1.0f);
        public static readonly UniqueValue ChaosLess1 = new UniqueValue(UniqueValueEnum.ChaosLess1, 1, "<1c", 1.5f, 0.3f);
        public static readonly UniqueValue ChaosLess1Labyrinth = new UniqueValue(UniqueValueEnum.ChaosLess1Labyrinth, 1, "<1c Labyrinth", 1.5f, 0.3f);
        public static readonly UniqueValue ChaosLess1Boss = new UniqueValue(UniqueValueEnum.ChaosLess1Boss, 1, "<1c Boss", 1.5f, 0.3f);
        public static readonly UniqueValue ChaosLess1League = new UniqueValue(UniqueValueEnum.ChaosLess1Crafted, 1, "<1c League", 1.5f, 0.3f);
        public static readonly UniqueValue ChaosLess1Shared = new UniqueValue(UniqueValueEnum.ChaosLess1Shared, 1, "<1c Shared", 1.5f, 0.3f);
        public static readonly UniqueValue ChaosLess1Crafted = new UniqueValue(UniqueValueEnum.ChaosLess1Crafted, 1, "<1c Crafted", 1.5f, 0.3f);
        public static readonly UniqueValue Chaos1to2 = new UniqueValue(UniqueValueEnum.Chaos1to2, 2, "1-2c", 3.0f, 0.8f);
        public static readonly UniqueValue Chaos2to10 = new UniqueValue(UniqueValueEnum.Chaos2to10, 3, "2-10c", 13.0f, 1.7f);
        public static readonly UniqueValue Chaos10 = new UniqueValue(UniqueValueEnum.Chaos10, 4, "10c+", float.MaxValue, 8.0f);

        public UniqueValueEnum Value { get; private set; }

        public int Tier { get; private set; }
        private readonly string toString;

        public override string ToString()
        {
            return toString;
        }

        public float HighValue { get; private set; }

        public float LowValue { get; private set; }

        public static UniqueValue FromTier(int tier)
        {
            switch (tier) {
                case 0:
                    return Unknown;
                case 1:
                    return ChaosLess1;
                case 2:
                    return Chaos1to2;
                case 3:
                    return Chaos2to10;
                case 4:
                    return Chaos10;
                default:
                    if (tier < 0)
                        return Error;
                    return Chaos10;
            }
        }

        public static UniqueValue ValueOf(float val)
        {
            if (val < 0.85f)
                return Unknown;
            else if (val < 1.1f)
                return ChaosLess1;
            else if (val < 2.5f)
                return Chaos1to2;
            else if (val < 9.0f)
                return Chaos2to10;
            return Chaos10;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return ((UniqueValue) obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        int IComparable<UniqueValue>.CompareTo(UniqueValue other)
        {
            return Tier.CompareTo(other.Tier);
        }
    }
}
