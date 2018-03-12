using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PoE_Price_Lister
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class JsonData
    {
        JsonData() { }

        //[JsonProperty(PropertyName = "id")]
        //public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "baseType")]
        public string BaseType { get; set; } // null?

        /*
        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        [JsonProperty(PropertyName = "mapTier")]
        public int MapTier { get; set; } // 0

        [JsonProperty(PropertyName = "levelRequired")]
        public int LevelRequired { get; set; } // 0

        [JsonProperty(PropertyName = "baseType")]
        public string BaseType { get; set; } // null?

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

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
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
