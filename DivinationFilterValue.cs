using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoE_Price_Lister
{
    public enum DivinationValueEnum
    {
        Error, Chaos10, Chaos1to10, ChaosLess1, NearlyWorthless, Worthless
    }

    public class DivinationFilterValue
    {
        DivinationValueEnum val;

        public DivinationFilterValue() { }

        public DivinationFilterValue(DivinationValueEnum val)
        {
            this.val = val;
        }

        public DivinationValueEnum Value
        {
            get { return val; }
            set { val = value; }
        }

        public override string ToString()
        {
            string output;
            switch (val)
            {
                case DivinationValueEnum.Error:
                    output = "Error";
                    break;
                case DivinationValueEnum.Chaos10:
                    output = "10c+";
                    break;
                case DivinationValueEnum.Chaos1to10:
                    output = "1c+";
                    break;
                case DivinationValueEnum.ChaosLess1:
                    output = "<1c";
                    break;
                case DivinationValueEnum.NearlyWorthless:
                    output = "Nearly Worthless";
                    break;
                case DivinationValueEnum.Worthless:
                    output = "Worthless";
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
                    case DivinationValueEnum.Error:
                        output = -1.0f;
                        break;
                    case DivinationValueEnum.Chaos10:
                        output = float.MaxValue;
                        break;
                    case DivinationValueEnum.Chaos1to10:
                        output = 13.0f;
                        break;
                    case DivinationValueEnum.ChaosLess1:
                        output = 1.5f;
                        break;
                    case DivinationValueEnum.NearlyWorthless:
                        output = 0.8f;
                        break;
                    case DivinationValueEnum.Worthless:
                        output = 0.6f;
                        break;
                    default:
                        output = -1.0f;
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
                    case DivinationValueEnum.Error:
                        output = -1.0f;
                        break;
                    case DivinationValueEnum.Chaos10:
                        output = 7.5f;
                        break;
                    case DivinationValueEnum.Chaos1to10:
                        output = 0.8f;
                        break;
                    case DivinationValueEnum.ChaosLess1:
                        output = 0.5f;
                        break;
                    case DivinationValueEnum.NearlyWorthless:
                        output = 0.15f;
                        break;
                    case DivinationValueEnum.Worthless:
                        output = 0.0f;
                        break;
                    default:
                        output = -1.0f;
                        break;
                }
                return output;
            }
        }

        public int ValueTier
        {
            get
            {
                int output;
                switch (val)
                {
                    case DivinationValueEnum.Error:
                        output = 100;
                        break;
                    case DivinationValueEnum.Worthless:
                        output = 0;
                        break;
                    case DivinationValueEnum.NearlyWorthless:
                        output = 1;
                        break;
                    case DivinationValueEnum.ChaosLess1:
                        output = 2;
                        break;
                    case DivinationValueEnum.Chaos1to10:
                        output = 3;
                        break;
                    case DivinationValueEnum.Chaos10:
                        output = 4;
                        break;
                    default:
                        output = 100;
                        break;
                }
                return output;
            }
        }

        public static DivinationFilterValue FromValueTier(int tier)
        {
            DivinationValueEnum output;
            switch (tier)
            {
                case 0:
                    output = DivinationValueEnum.Worthless;
                    break;
                case 1:
                    output = DivinationValueEnum.NearlyWorthless;
                    break;
                case 2:
                    output = DivinationValueEnum.ChaosLess1;
                    break;
                case 3:
                    output = DivinationValueEnum.Chaos1to10;
                    break;
                case 4:
                    output = DivinationValueEnum.Chaos10;
                    break;
                default:
                    output = DivinationValueEnum.Error;
                    break;
            }
            return new DivinationFilterValue(output);
        }

        public static DivinationFilterValue ValueOf(float val)
        {
            DivinationValueEnum output;
            if (val < 0.4f)
                output = DivinationValueEnum.Worthless;
            else if (val < 0.65f)
                output = DivinationValueEnum.NearlyWorthless;
            else if (val < 1.01f)
                output = DivinationValueEnum.ChaosLess1;
            else if (val < 9.0f)
                output = DivinationValueEnum.Chaos1to10;
            else
                output = DivinationValueEnum.Chaos10;
            return new DivinationFilterValue(output);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            DivinationFilterValue other = (DivinationFilterValue)obj;
            return other.val == val;
        }

        public override int GetHashCode()
        {
            return val.GetHashCode();
        }
    }
}
