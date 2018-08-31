namespace PoE_Price_Lister
{
    public enum UniqueValue
    {
        Unknown, Chaos10, Chaos2to10, Chaos1to2, ChaosLess1, Error, ChaosLess1League, ChaosLess1Shared, ChaosLess1Boss, ChaosLess1Crafted, ChaosLess1Labyrinth
    }

    public class UniqueFilterValue
    {
        public UniqueFilterValue() { }

        public UniqueFilterValue(UniqueValue val)
        {
            Value = val;
        }

        public UniqueValue Value { get; set; }

        public int ValueTier {
            get {
                int output;
                switch (Value) {
                    case UniqueValue.Unknown:
                        output = 0;
                        break;
                    case UniqueValue.ChaosLess1:
                    case UniqueValue.ChaosLess1League:
                    case UniqueValue.ChaosLess1Shared:
                    case UniqueValue.ChaosLess1Boss:
                    case UniqueValue.ChaosLess1Crafted:
                    case UniqueValue.ChaosLess1Labyrinth:
                        output = 1;
                        break;
                    case UniqueValue.Chaos1to2:
                        output = 2;
                        break;
                    case UniqueValue.Chaos2to10:
                        output = 3;
                        break;
                    case UniqueValue.Chaos10:
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
            switch (Value) {
                case UniqueValue.Unknown:
                    output = "Unknown";
                    break;
                case UniqueValue.ChaosLess1:
                    output = "<1c";
                    break;
                case UniqueValue.ChaosLess1Shared:
                    output = "<1c Shared";
                    break;
                case UniqueValue.ChaosLess1League:
                    output = "<1c League";
                    break;
                case UniqueValue.ChaosLess1Boss:
                    output = "<1c Boss";
                    break;
                case UniqueValue.ChaosLess1Crafted:
                    output = "<1c Crafted";
                    break;
                case UniqueValue.ChaosLess1Labyrinth:
                    output = "<1c Labyrinth";
                    break;
                case UniqueValue.Chaos1to2:
                    output = "1-2c";
                    break;
                case UniqueValue.Chaos2to10:
                    output = "2-10c";
                    break;
                case UniqueValue.Chaos10:
                    output = "10c+";
                    break;
                default:
                    output = "Error";
                    break;
            }
            return output;
        }

        public float HighValue {
            get {
                float output;
                switch (Value) {
                    case UniqueValue.Unknown:
                        output = 0.95f;
                        break;
                    case UniqueValue.ChaosLess1:
                    case UniqueValue.ChaosLess1Shared:
                    case UniqueValue.ChaosLess1League:
                    case UniqueValue.ChaosLess1Boss:
                    case UniqueValue.ChaosLess1Crafted:
                    case UniqueValue.ChaosLess1Labyrinth:
                        output = 1.5f;
                        break;
                    case UniqueValue.Chaos1to2:
                        output = 3.0f;
                        break;
                    case UniqueValue.Chaos2to10:
                        output = 13.0f;
                        break;
                    case UniqueValue.Chaos10:
                        output = float.MaxValue;
                        break;
                    default:
                        output = 0.0f;
                        break;
                }
                return output;
            }
        }

        public float LowValue {
            get {
                float output;
                switch (Value) {
                    case UniqueValue.Unknown:
                        output = -1.0f;
                        break;
                    case UniqueValue.ChaosLess1:
                    case UniqueValue.ChaosLess1Shared:
                    case UniqueValue.ChaosLess1League:
                    case UniqueValue.ChaosLess1Boss:
                    case UniqueValue.ChaosLess1Crafted:
                    case UniqueValue.ChaosLess1Labyrinth:
                        output = 0.0f;
                        break;
                    case UniqueValue.Chaos1to2:
                        output = 0.8f;
                        break;
                    case UniqueValue.Chaos2to10:
                        output = 1.7f;
                        break;
                    case UniqueValue.Chaos10:
                        output = 8.0f;
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
            UniqueValue output;
            switch (tier) {
                case 0:
                    output = UniqueValue.Unknown;
                    break;
                case 1:
                    output = UniqueValue.ChaosLess1;
                    break;
                case 2:
                    output = UniqueValue.Chaos1to2;
                    break;
                case 3:
                    output = UniqueValue.Chaos2to10;
                    break;
                case 4:
                    output = UniqueValue.Chaos10;
                    break;
                default:
                    output = UniqueValue.Error;
                    break;
            }
            return new UniqueFilterValue(output);
        }

        public static UniqueFilterValue ValueOf(float val)
        {
            UniqueValue output;
            if (val < 0.85f)
                output = UniqueValue.Unknown;
            else if (val < 1.1f)
                output = UniqueValue.ChaosLess1;
            else if (val < 2.5f)
                output = UniqueValue.Chaos1to2;
            else if (val < 9.0f)
                output = UniqueValue.Chaos2to10;
            else
                output = UniqueValue.Chaos10;
            return new UniqueFilterValue(output);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            UniqueFilterValue other = (UniqueFilterValue) obj;
            return other.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
