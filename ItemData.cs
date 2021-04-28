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
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace PoE_Price_Lister
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public sealed class ItemData : IEquatable<ItemData>
    {
		public ItemData() { }

		//[JsonProperty(PropertyName = "id")]
		//public int Id { get; set; }

		[JsonProperty(PropertyName = "name")]
		[JsonConverter(typeof(TrimConverter))]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "baseType")]
		public string BaseType { get; set; } // null?

		//[JsonProperty(PropertyName = "icon")]
		//public string Icon { get; set; }

		/*
        [JsonProperty(PropertyName = "mapTier")]
        public int MapTier { get; set; } // 0

        [JsonProperty(PropertyName = "levelRequired")]
        public int LevelRequired { get; set; } // 0

        [JsonProperty(PropertyName = "stackSize")]
        public int StackSize { get; set; }

        [JsonProperty(PropertyName = "variant")]
        public string Variant { get; set; } // null

        [JsonProperty(PropertyName = "prophecyText")]
        public string ProphecyText { get; set; } // null?

        // JsonConverter?
        [JsonProperty(PropertyName = "sparkline")]
        public IDictionary<string, Object> Sparkline { get; set; }

        [JsonProperty(PropertyName = "implicitModifiers")]
        public IList<IDictionary<string, string>> ImplicitModifiers { get; set; }

        [JsonProperty(PropertyName = "explicitModifiers")]
        public IList<IDictionary<string, string>> ExplicitModifiers { get; set; }

        [JsonProperty(PropertyName = "flavourText")]
        public string FlavourText { get; set; }

        [JsonProperty(PropertyName = "corrupted")]
        public bool Corrupted { get; set; } // false

        [JsonProperty(PropertyName = "gemLevel")]
        public int GemLevel { get; set; } // 0

        [JsonProperty(PropertyName = "gemQuality")]
        public int GemQuality { get; set; } // 0
        */

		[JsonProperty(PropertyName = "links")]
		public int Links { get; set; }

		//[JsonProperty(PropertyName = "itemType")]
		//public string ItemType { get; set; }

		[JsonProperty(PropertyName = "chaosValue")]
		public float ChaosValue { get; set; }

		//[JsonProperty(PropertyName = "exaltedValue")]
		//public float ExaltedValue { get; set; }

		[JsonProperty(PropertyName = "count")]
		public int Count { get; set; } // too low means low accuracy

        public override bool Equals(object obj)
        {
            return Equals(obj as ItemData);
        }

        public bool Equals(ItemData other)
        {
            return other != null &&
                   Name == other.Name &&
                   BaseType == other.BaseType;
        }

        public override int GetHashCode()
        {
            int hashCode = -1412448124;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BaseType);
            return hashCode;
        }

        public override string ToString()
		{
            StringBuilder sb = new StringBuilder();
            if(Links > 4) {
                sb.Append('(').Append(Links).Append("L)");
            }
            sb.Append(Name ?? "").Append(' ');
            if (Count > 5)
                sb.Append(ChaosValue).Append("c ");
            sb.Append(BaseType ?? "");
            return sb.ToString();

			//return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		internal class TrimConverter : JsonConverter
		{
			public override bool CanRead => true;
			public override bool CanWrite => false;

			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(string);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				return ((string) reader.Value)?.Trim();
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}
		}
	}
}

/*
{ lines:[
{
     "id":636,
     "name":"House of Mirrors",
     "icon":"http://web.poecdn.com/image/Art/2DItems/Divination/InventoryIcon.png?scale=1&scaleIndex=0&w=1&h=1",
     "mapTier":0,
     "levelRequired":0,
     "baseType":null,
     "stackSize":9,
     "variant":null,
     "prophecyText":null,
     "artFilename":"HouseOfMirrors",
     "links":0,
     "itemClass":6,
     "sparkline":{
        "data":[
           null,
           null,
           null,
           null,
           0.0,
           25.64,
           17.33
        ],
        "totalChange":17.33
     },
     "implicitModifiers":[

     ],
     "explicitModifiers":[
        {
           "text":"<currencyitem>{Mirror of Kalandra}",
           "optional":false
        }
     ],
     "flavourText":"What do you see in the mirror?",
     "corrupted":false,
     "gemLevel":0,
     "gemQuality":0,
     "itemType":"Unknown",
     "chaosValue":970.00,
     "exaltedValue":20.00,
     "count":6
}
*/
