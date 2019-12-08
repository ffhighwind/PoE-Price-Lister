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
	public class EnchantmentValue : IComparable<EnchantmentValue>
	{
		protected EnchantmentValue(EnchantmentValueEnum value, string toString, float highValue, float lowValue)
		{
			Value = value;
			HighValue = highValue;
			LowValue = lowValue;
			this.toString = toString;
		}

		public static readonly EnchantmentValue Error = new EnchantmentValue(EnchantmentValueEnum.Error, "Error", 0.0f, float.MinValue);
		public static readonly EnchantmentValue Worthless = new EnchantmentValue(EnchantmentValueEnum.Worthless, "Worthless", 13, -1);
		public static readonly EnchantmentValue Chaos10 = new EnchantmentValue(EnchantmentValueEnum.Chaos10, "10c+", 23, 7);
		public static readonly EnchantmentValue Chaos20 = new EnchantmentValue(EnchantmentValueEnum.Chaos20, "20c+", float.MaxValue, 16);

		public EnchantmentValueEnum Value { get; private set; }

		private readonly string toString;

		public override string ToString()
		{
			return toString;
		}

		public float HighValue { get; private set; }

		public float LowValue { get; private set; }

		public int Tier => (int) Value;

		public static EnchantmentValue FromTier(int tier)
		{
			switch (tier) {
				case (int) EnchantmentValueEnum.Worthless: // 1
					return Worthless;
				case (int) EnchantmentValueEnum.Chaos10: // 2
					return Chaos10;
				case (int) EnchantmentValueEnum.Chaos20: // 3
					return Chaos20;
				default:
					if (tier < 0)
						return Error; // -1
					return Chaos20;
			}
		}

		public static EnchantmentValue ValueOf(float val)
		{
			if (val < 0)
				return Error;
			if (val < 8.0f)
				return Worthless;
			if (val < 19.0f)
				return Chaos10;
			return Chaos20;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is EnchantmentValue other)
				return other.Value == Value;
			return false;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public int CompareTo(EnchantmentValue other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
