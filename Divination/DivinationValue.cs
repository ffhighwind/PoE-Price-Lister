#region License
// Copyright © 2018 Wesley Hamilton
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at https://github.com/ffhighwind/PoE-Price-Lister
#endregion

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
		public static readonly DivinationValue Chaos10 = new DivinationValue(DivinationValueEnum.Chaos10, "10c+", float.MaxValue, 8.5f);
		public static readonly DivinationValue Chaos2to10 = new DivinationValue(DivinationValueEnum.Chaos2to10, "2-10c", 13.0f, 1.7f);
		public static readonly DivinationValue ChaosLess2 = new DivinationValue(DivinationValueEnum.ChaosLess2, "<2c", 2.5f, 0.9f);
		public static readonly DivinationValue NearlyWorthless = new DivinationValue(DivinationValueEnum.NearlyWorthless, "Nearly Worthless", 0.995f, 0.0f);
		public static readonly DivinationValue Worthless = new DivinationValue(DivinationValueEnum.Worthless, "Worthless", 0.85f, -1.0f);

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
				case (int) DivinationValueEnum.ChaosLess2: // 2
					return ChaosLess2;
				case (int) DivinationValueEnum.Chaos2to10: // 3
					return Chaos2to10;
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
			//if (val < 0.4f)
			//	return Worthless;
			if (val <= 0.80f)
				return NearlyWorthless;
			else if (val < 2.1f)
				return ChaosLess2;
			else if (val < 9.5f)
				return Chaos2to10;
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
