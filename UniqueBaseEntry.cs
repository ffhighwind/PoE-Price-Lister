using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoE_Price_Lister
{
    public class UniqueBaseEntry
    {
        string baseType;
        UniqueFilterValue filterValue = new UniqueFilterValue();
        UniqueFilterValue expectedValue = new UniqueFilterValue();
        List<UniqueData> items = new List<UniqueData>();

        public UniqueBaseEntry() { }

        public void Add(UniqueCsvData item)
        {
            foreach(var i in items)
            {
                if(i.Name == item.Name)
                {
                    i.Load(item);
                    return;
                }
            }
            items.Add(new UniqueData(item));
        }

        public void Add(JsonData item)
        {
            foreach(var i in items)
            {
                if (i.Name == item.Name)
                {
                    if(i.Count == 0 || i.Links >= item.Links)
                        i.Load(item);
                    return;
                }
            }
            MessageBox.Show("JSON: The CSV file is missing: " + item.BaseType + " " + item.Name, "Error", MessageBoxButtons.OK);
            items.Add(new UniqueData(item));
        }

        public void CalculateExpectedValue()
        {
            if (items.Count() == 0)
            {
                expectedValue = filterValue;
                return;
            }

            float minValueBoss = float.MaxValue;
            float maxValueBoss = float.MinValue;
            float minValueLeague = float.MaxValue;
            float maxValueLeague = float.MinValue;
            float minValueCraftedFated = float.MaxValue;
            float maxValueCraftedFated = float.MinValue;
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            bool isLeagueOnly = true;
            bool hasLeague = false;
            bool hasCoreLeague = false;
            bool hasLabyrinth = false;
            bool isBossOnly = true;
            bool hasBoss = false;
            bool isCraftedOnly = true;
            bool isUnobtainable = true;
            int minExpectedTier = 1;
            UniqueValueEnum minExpected = UniqueValueEnum.Unknown;

            // Determine Expected Value
            foreach (UniqueData uniqData in items)
            {
                if (uniqData.Unobtainable)
                    continue;
                isUnobtainable = false;
                if (uniqData.IsCrafted || uniqData.IsFated || uniqData.IsPurchased)
                {
                    if (!uniqData.IsLowConfidence)
                    {
                        if (minValueCraftedFated > uniqData.ChaosValue)
                            minValueCraftedFated = uniqData.ChaosValue;
                        if (maxValueCraftedFated < uniqData.ChaosValue)
                            maxValueCraftedFated = uniqData.ChaosValue;
                    }
                    continue;
                }
                isCraftedOnly = false;

                if(uniqData.IsCoreDrop)
                {
                    isLeagueOnly = false;
                    hasCoreLeague = hasCoreLeague || uniqData.League.Length != 0;
                    if(uniqData.IsLimitedDrop)
                    {
                        if (uniqData.IsLabyrinthDrop)
                            hasLabyrinth = true;
                        else
                            hasBoss = true;
                        if (!uniqData.IsLowConfidence)
                        {
                            if (minValueBoss > uniqData.ChaosValue)
                                minValueBoss = uniqData.ChaosValue;
                            if (maxValueBoss < uniqData.ChaosValue)
                                maxValueBoss = uniqData.ChaosValue;
                        }
                    }
                    else
                    {
                        isBossOnly = false;
                        if (!uniqData.IsLowConfidence)
                        {
                            if (minValue > uniqData.ChaosValue)
                                minValue = uniqData.ChaosValue;
                            if (maxValue < uniqData.ChaosValue)
                                maxValue = uniqData.ChaosValue;
                        }
                    }
                }
                else
                {
                    hasLeague = true;
                    if (!uniqData.IsLowConfidence)
                    {
                        if (minValueLeague > uniqData.ChaosValue)
                            minValueLeague = uniqData.ChaosValue;
                        if (maxValueLeague < uniqData.ChaosValue)
                            maxValueLeague = uniqData.ChaosValue;
                    }
                }
            }

            if (isUnobtainable)
            {
                expectedValue.Value = UniqueValueEnum.Chaos10; // unique exists in permanent leagues only
                return;
            }
            else if (isCraftedOnly)
            {
                minValue = minValueCraftedFated;
                maxValue = maxValueCraftedFated;
                minExpected = UniqueValueEnum.ChaosLess1Crafted;
            }
            else if (isLeagueOnly)
            {
                minValue = minValueLeague;
                maxValue = maxValueLeague;
                minExpected = UniqueValueEnum.ChaosLess1League;
            }
            else if (isBossOnly)
            {
                minValue = minValueBoss;
                maxValue = maxValueBoss;
                if(hasBoss)
                    minExpected = UniqueValueEnum.ChaosLess1Boss;
                else
                    minExpected = UniqueValueEnum.ChaosLess1Labyrinth;
            }
            else if (hasLeague)
            {
                if (minValueLeague > 4.5f)
                    minExpected = UniqueValueEnum.ChaosLess1Shared;
                else
                    minExpected = UniqueValueEnum.ChaosLess1;
            }
            else if(hasBoss)
            {
                minExpected = UniqueValueEnum.ChaosLess1Boss;
            }
            else if (hasCoreLeague)
            {
                minExpected = UniqueValueEnum.ChaosLess1League;
            }
            else if(hasLabyrinth)
                minExpected = UniqueValueEnum.ChaosLess1;
            else
                minExpectedTier = 0;

            // Set Expected Value
            if (minValue > maxValue) //no confident value
                expectedValue = filterValue;
            else //confident value
            {
                expectedValue = UniqueFilterValue.ValueOf(minValue);
                if (expectedValue.ValueTier <= 1)
                {
                    if (maxValue > 50.0f)
                        expectedValue.Value = UniqueValueEnum.Chaos2to10;
                    else if (maxValue > 9.0f || (minValue > 0.95f && maxValue > 1.8f))
                        expectedValue.Value = UniqueValueEnum.Chaos1to2;
                    else if(maxValue > 4.5f || minValue > 0.95f)
                        expectedValue.Value = UniqueValueEnum.ChaosLess1;
                }
                else if (expectedValue.Value == UniqueValueEnum.Chaos1to2 && minValue > 1.9f && maxValue > 4.9f)
                    expectedValue.Value = UniqueValueEnum.Chaos2to10;
            }
            if (expectedValue.ValueTier <= minExpectedTier)
                expectedValue.Value = minExpected;
        }

        public UniqueFilterValue ExpectedFilterValue
        {
            get { return expectedValue; }
        }

        public int SeverityLevel
        {
            get
            {
                if (filterValue == expectedValue)
                    return 0;
                int expectTier = expectedValue.ValueTier;
                int severity = Math.Abs(filterValue.ValueTier - expectTier);

                if (severity != 0 && expectTier >= 4)
                    severity += 1;
                return severity;
            }
        }

        public string BaseType
        {
            get { return baseType; }
            set { baseType = value; }
        }

        public string QuotedBaseType
        {
            get
            {
                if (baseType.Contains(' '))
                    return "\"" + baseType + "\"";
                return baseType;
            }
        }

        public UniqueFilterValue FilterValue
        {
            get { return filterValue; }
            set { filterValue = value; }
        }

        public List<UniqueData> Items
        {
            get { return items; }
            set { this.items = value; }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            UniqueBaseEntry other = (UniqueBaseEntry)obj;
            return other.baseType == baseType;
        }

        public override int GetHashCode()
        {
            return baseType.GetHashCode();
        }
    }
}
