using System;
using System.Collections;
using System.Data;

namespace PoE_Price_Lister
{
	public class UniqueValue : IComparable<UniqueValue>
	{
		protected UniqueValue(UniqueValueEnum value, int tier, string toString, float highValue, float lowValue)
		{
			Value = value;
			Tier = tier;
			this.toString = toString;
			HighValue = highValue;
			LowValue = lowValue;
		}

		public static readonly UniqueValue Worthless = new UniqueValue(UniqueValueEnum.Unknown, 0, "Worthless", 2.0f, -1.0f);
		public static readonly UniqueValue Limited = new UniqueValue(UniqueValueEnum.Limited, 1, "Limited", 3.5f, 0.0f);
		public static readonly UniqueValue Chaos3to5 = new UniqueValue(UniqueValueEnum.Chaos3to5, 2, "3-5c", 5.5f, 2.6f);
		public static readonly UniqueValue Chaos5to15 = new UniqueValue(UniqueValueEnum.Chaos5to15, 3, "5-15c", 16.0f, 4.5f);
		public static readonly UniqueValue Chaos15 = new UniqueValue(UniqueValueEnum.Chaos15, 4, "15c+", float.MaxValue, 12.0f);

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
					return Worthless;
				case 1:
					return Limited;
				case 2:
					return Chaos3to5;
				case 3:
					return Chaos5to15;
				case 4:
					return Chaos15;
				default:
					return tier < 0 ? Worthless : Chaos15;
			}
		}

		public static UniqueValue ValueOf(float val)
		{
			if (val <= 2.9f)
				return Worthless;
			else if (val < 5.2f)
				return Chaos3to5;
			else if (val < 14.5f)
				return Chaos5to15;
			return Chaos15;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is UniqueValue other) {
				return other.Value == Value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public int CompareTo(UniqueValue other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
