using System;

namespace PoE_Price_Lister
{
	public class DivinationValue : IComparable<DivinationValue>
	{
		protected DivinationValue(DivinationValueEnum value, string toString, float highValue, float lowValue)
		{
			Value = value;
			this.toString = toString;
			HighValue = highValue;
			LowValue = lowValue;
		}

		public static readonly DivinationValue Error = new DivinationValue(DivinationValueEnum.Error, "Error", 0.0f, float.MinValue);
		public static readonly DivinationValue Chaos10 = new DivinationValue(DivinationValueEnum.Chaos10, "10c+", float.MaxValue, 6.0f);
		public static readonly DivinationValue Chaos1to10 = new DivinationValue(DivinationValueEnum.Chaos1to10, "1c+", 13.0f, 0.9f);
		public static readonly DivinationValue ChaosLess1 = new DivinationValue(DivinationValueEnum.ChaosLess1, "<1c", 1.5f, 0.9f);
		public static readonly DivinationValue NearlyWorthless = new DivinationValue(DivinationValueEnum.NearlyWorthless, "Nearly Worthless", 0.9f, 0.2f);
		public static readonly DivinationValue Worthless = new DivinationValue(DivinationValueEnum.Worthless, "Worthless", 0.66f, -1.0f);

		public DivinationValueEnum Value { get; private set; }

		private readonly string toString;

		public override string ToString()
		{
			return toString;
		}

		public float HighValue { get; private set; }

		public float LowValue { get; private set; }

		public int Tier => (int) Value;

		public static DivinationValue FromTier(int tier)
		{
			switch (tier) {
				case (int) DivinationValueEnum.Worthless: // 0
					return Worthless;
				case (int) DivinationValueEnum.NearlyWorthless: // 1
					return NearlyWorthless;
				case (int) DivinationValueEnum.ChaosLess1: // 2
					return ChaosLess1;
				case (int) DivinationValueEnum.Chaos1to10: // 3
					return Chaos1to10;
				case (int) DivinationValueEnum.Chaos10: // 4
					return Chaos10;
				default:
					if (tier < 0)
						return Error; // -1
					return Chaos10;
			}
		}

		public static DivinationValue ValueOf(float val)
		{
			if (val < 0.4f)
				return Worthless;
			else if (val < 0.65f)
				return NearlyWorthless;
			else if (val < 1.01f)
				return ChaosLess1;
			else if (val < 9.0f)
				return Chaos1to10;
			return Chaos10;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is DivinationValue other)
				return other.Value == Value;
			return false;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public int CompareTo(DivinationValue other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
