using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoE_Price_Lister
{
	public class ItemKey : IEquatable<ItemKey>
	{
		public ItemKey(string category, string baseType)
		{
			BaseType = baseType;
			Category = category;
		}

		public string BaseType { get; }
		public string Category { get; }

		public override bool Equals(object obj)
		{
			return Equals(obj as ItemKey);
		}

		public bool Equals(ItemKey other)
		{
			return other != null &&
				BaseType == other.BaseType &&
				Category == other.Category;
		}

		public override int GetHashCode()
		{
			int hashCode = -817170655;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BaseType);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Category);
			return hashCode;
		}

		public override string ToString()
		{
			return "[" + Category + " " + BaseType + "]";
		}
	}
}
