using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoE_Price_Lister
{
	public class ItemKey
	{
		public ItemKey(string category, string baseType)
		{
			BaseType = baseType;
			Category = category;
		}

		public string BaseType { get; }
		public string Category { get; }
	}
}
