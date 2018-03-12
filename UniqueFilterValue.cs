using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoE_Price_Lister
{
    public enum UniqueValueEnum
    {
        Unknown, Chaos10, Chaos2to10, Chaos1to2, ChaosLess1, Error, ChaosLess1NoHide
    }

    public class UniqueFilterValue
    {
        UniqueValueEnum val;

        public UniqueFilterValue() { }

        public UniqueFilterValue(UniqueValueEnum val)
        {
            this.val = val;
        }

        public UniqueValueEnum Value
        {
            get { return val; }
            set { val = value; }
        }

        public int ValueTier
        {
            get
            {
                int output;
                switch (val)
                {
                    case UniqueValueEnum.Unknown:
                        output = 0;
                        break;
                    case UniqueValueEnum.ChaosLess1:
                    case UniqueValueEnum.ChaosLess1NoHide:
                        output = 1;
                        break;
                    case UniqueValueEnum.Chaos1to2:
                        output = 2;
                        break;
                    case UniqueValueEnum.Chaos2to10:
                        output = 3;
                        break;
                    case UniqueValueEnum.Chaos10:
                        output = 4;
                        break;
                    default:
                        output = 100;
                        break;
                }
                return output;
            }
        }

        public override string ToString()
        {
            string output;
            switch (val)
            {
                case UniqueValueEnum.Unknown:
                    output = "Unknown";
                    break;
                case UniqueValueEnum.Chaos10:
                    output = "10c+";
                    break;
                case UniqueValueEnum.Chaos2to10:
                    output = "2-10c";
                    break;
                case UniqueValueEnum.Chaos1to2:
                    output = "1-2c";
                    break;
                case UniqueValueEnum.ChaosLess1:
                    output = "<1c";
                    break;
                case UniqueValueEnum.ChaosLess1NoHide:
                    output = "<1c NoHide";
                    break;
                default:
                    output = "Error";
                    break;
            }
            return output;
        }

        public float HighValue
        {
            get
            {
                float output;
                switch (val)
                {
                    case UniqueValueEnum.Unknown:
                        output = 0.95f;
                        break;
                    case UniqueValueEnum.Chaos10:
                        output = float.MaxValue;
                        break;
                    case UniqueValueEnum.Chaos2to10:
                        output = 13.0f;
                        break;
                    case UniqueValueEnum.Chaos1to2:
                        output = 3.0f;
                        break;
                    case UniqueValueEnum.ChaosLess1:
                    case UniqueValueEnum.ChaosLess1NoHide:
                        output = 1.5f;
                        break;
                    default:
                        output = 0.0f;
                        break;
                }
                return output;
            }
        }

        public float LowValue
        {
            get 
            {
                float output;
                switch (val)
                {
                    case UniqueValueEnum.Unknown:
                        output = -1.0f;
                        break;
                    case UniqueValueEnum.Chaos10:
                        output = 8.0f;
                        break;
                    case UniqueValueEnum.Chaos2to10:
                        output = 1.7f;
                        break;
                    case UniqueValueEnum.Chaos1to2:
                        output = 0.8f;
                        break;
                    case UniqueValueEnum.ChaosLess1:
                    case UniqueValueEnum.ChaosLess1NoHide:
                        output = 0.0f;
                        break;
                    default:
                        output = -1.0f;
                        break;
                }
                return output;
            }
        }

        public static UniqueFilterValue FromValueTier(int tier)
        {
            UniqueValueEnum output;
            switch (tier)
            {
                case 0:
                    output = UniqueValueEnum.Unknown;
                    break;
                case 1:
                    output = UniqueValueEnum.ChaosLess1NoHide;
                    break;
                case 2:
                    output = UniqueValueEnum.Chaos1to2;
                    break;
                case 3:
                    output = UniqueValueEnum.Chaos2to10;
                    break;
                case 4:
                    output = UniqueValueEnum.Chaos10;
                    break;
                default:
                    output = UniqueValueEnum.Error;
                    break;
            }
            return new UniqueFilterValue(output);
        }

        public static UniqueFilterValue ValueOf(float val)
        {
            UniqueValueEnum output;
            if (val < 0.85f)
                output = UniqueValueEnum.Unknown;
            else if (val < 1.1f)
                output = UniqueValueEnum.ChaosLess1;
            else if (val < 2.5f)
                output = UniqueValueEnum.Chaos1to2;
            else if (val < 9.0f)
                output = UniqueValueEnum.Chaos2to10;
            else
                output = UniqueValueEnum.Chaos10;
            return new UniqueFilterValue(output);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            UniqueFilterValue other = (UniqueFilterValue)obj;
            return other.val == val;
        }

        public override int GetHashCode()
        {
            return val.GetHashCode();
        }
    }
}
