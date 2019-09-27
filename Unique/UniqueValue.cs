using System;

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

		//public static readonly UniqueValue Error = new UniqueValue(UniqueValueEnum.Error, -1, "Error", 0.0f, float.MinValue);
		public static readonly UniqueValue Unknown = new UniqueValue(UniqueValueEnum.Unknown, 0, "Unknown", 2.0f, -1.0f);
		//public static readonly UniqueValue Labyrinth = new UniqueValue(UniqueValueEnum.Labyrinth, 1, "Labyrinth", 2.5f, 0.0f);
		//public static readonly UniqueValue Boss = new UniqueValue(UniqueValueEnum.Boss, 1, "Boss", 2.5f, 0.0f);
		//public static readonly UniqueValue League = new UniqueValue(UniqueValueEnum.League, 1, "League", 2.5f, 0.0f);
		public static readonly UniqueValue Shared = new UniqueValue(UniqueValueEnum.Shared, 1, "Shared", 2.5f, 0.0f);
		//public static readonly UniqueValue Crafted = new UniqueValue(UniqueValueEnum.Crafted, 1, "Crafted", 2.5f, 0.0f);
		public static readonly UniqueValue Chaos2to3 = new UniqueValue(UniqueValueEnum.Chaos2to3, 2, "2-3c", 3.0f, 1.6f);
		public static readonly UniqueValue Chaos3to10 = new UniqueValue(UniqueValueEnum.Chaos3to10, 3, "3-10c", 13.0f, 2.7f);
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
					return Shared;
				case 2:
					return Chaos2to3;
				case 3:
					return Chaos3to10;
				case 4:
					return Chaos10;
				default:
					if (tier < 0)
						return Unknown;
					return Chaos10;
			}
		}

		public static UniqueValue ValueOf(float val)
		{
			if (val <= 1.7f)
				return Unknown;
			//else if (val < 1.1f)
			//	return ChaosLess1;
			else if (val < 3.2f)
				return Chaos2to3;
			else if (val < 9.0f)
				return Chaos3to10;
			return Chaos10;
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

		int IComparable<UniqueValue>.CompareTo(UniqueValue other)
		{
			return Tier.CompareTo(other.Tier);
		}
	}
}
