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

        /*
        public bool IsWorthless
        {
            get
            {
                bool coreLeagueOnly = true;
                float minValue = filterValue.LowValue;
                foreach (UniqueData uniqData in items)
                {
                    string league = uniqData.League;
                    if (uniqData.ChaosValue > 5.0f)
                        return false;
                    bool isCoreLeague = IsCoreLeague(league);
                    coreLeagueOnly = coreLeagueOnly && !isCoreLeague;
                    if (uniqData.ChaosValue > 1.2f)
                        return false;
                }
                return true;
            }
        }
        */

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
            float minValueCore = float.MaxValue;
            float maxValueCore = float.MinValue;
            bool isCoreDrop = false;
            bool isUnobtainable = true;
            bool nohide = false;

            foreach (UniqueData uniqData in items)
            {
                if (uniqData.Unobtainable)
                    continue;
                isUnobtainable = false;
                if (uniqData.IsCrafted || uniqData.IsFated)
                    continue;

                bool isBossDrop = uniqData.IsBossDrop;
                nohide = nohide || isBossDrop || uniqData.League.Length != 0;

                if (uniqData.IsCoreDrop)
                {
                    isCoreDrop = true;
                    if (uniqData.IsLowConfidence)
                        continue;
                    if (isBossDrop)
                    {
                        if (minValueBoss > uniqData.ChaosValue)
                            minValueBoss = uniqData.ChaosValue;
                        if (maxValueBoss < uniqData.ChaosValue)
                            maxValueBoss = uniqData.ChaosValue;
                    }
                    else
                    {
                        if (minValueCore > uniqData.ChaosValue)
                            minValueCore = uniqData.ChaosValue;
                        if (maxValueCore < uniqData.ChaosValue)
                            maxValueCore = uniqData.ChaosValue;
                    }
                }
                else
                {
                    nohide = nohide || uniqData.ChaosValue > 9.0f;
                    if (isCoreDrop || uniqData.IsLowConfidence)
                        continue;
                    if (minValueLeague > uniqData.ChaosValue)
                        minValueLeague = uniqData.ChaosValue;
                    if (maxValueLeague < uniqData.ChaosValue)
                        maxValueLeague = uniqData.ChaosValue;
                }
            }

            if (isUnobtainable)
                expectedValue = new UniqueFilterValue(UniqueValueEnum.Chaos10); // unique exists in permanent leagues only
            else if (isCoreDrop)
            {
                if (minValueCore > maxValueCore) //Boss drop or PoE Ninja doesn't have a confident value
                {
                    if (minValueBoss > maxValueBoss) //not a boss drop
                    {
                        expectedValue = filterValue;
                        if (expectedValue.ValueTier <= 1 && maxValueLeague >= minValueLeague)
                            expectedValue.Value = UniqueValueEnum.ChaosLess1NoHide; // always show potentially expensive league drops
                    }
                    else
                    {
                        expectedValue = UniqueFilterValue.ValueOf(minValueBoss);
                        if (expectedValue.ValueTier <= 1)
                            expectedValue.Value = UniqueValueEnum.ChaosLess1NoHide; // always show core boss drops
                    }
                }
                else //PoE Ninja has a confident value
                {
                    expectedValue = UniqueFilterValue.ValueOf(minValueCore);
                    if (expectedValue.ValueTier <= 1)
                    {
                        if (maxValueCore > 50.0f)
                            expectedValue = new UniqueFilterValue(UniqueValueEnum.Chaos2to10);
                        else if (maxValueCore > 9.0f || (minValueCore > 0.95f && maxValueCore > 1.8f))
                            expectedValue = new UniqueFilterValue(UniqueValueEnum.Chaos1to2);
                        else if (nohide || maxValueCore > 5.0f)
                            expectedValue = new UniqueFilterValue(UniqueValueEnum.ChaosLess1NoHide);
                        //else if (filterValue.Value != UniqueValueEnum.Unknown)
                        //   expectedValue = new UniqueFilterValue(UniqueValueEnum.ChaosLess1);
                    }
                    else if (expectedValue.Value == UniqueValueEnum.Chaos1to2 && minValueCore > 1.9f && maxValueCore > 4.9f)
                        expectedValue = new UniqueFilterValue(UniqueValueEnum.Chaos2to10);
                }
            }
            else // League-specific drop
            {
                if (minValueLeague > maxValueLeague) //PoE Ninja doesn't have a confident value
                {
                    expectedValue = filterValue;
                    if (expectedValue.ValueTier <= 1)
                        expectedValue = new UniqueFilterValue(UniqueValueEnum.ChaosLess1NoHide); // Filter may not have the BaseType. League-specific should not be hidden.
                }
                else //PoE Ninja has a confident value
                {
                    expectedValue = UniqueFilterValue.ValueOf(minValueLeague);
                    if (expectedValue.ValueTier <= 1)
                    {
                        if (maxValueLeague > 50.0f)
                            expectedValue = new UniqueFilterValue(UniqueValueEnum.Chaos2to10);
                        else if (maxValueLeague > 9.0f || (minValueLeague > 0.95f && maxValueLeague > 1.8f))
                            expectedValue = new UniqueFilterValue(UniqueValueEnum.Chaos1to2);
                        else
                            expectedValue = new UniqueFilterValue(UniqueValueEnum.ChaosLess1NoHide);
                    }
                    else if (expectedValue.Value == UniqueValueEnum.Chaos1to2 && minValueLeague > 1.9f && maxValueLeague > 4.9f)
                        expectedValue = new UniqueFilterValue(UniqueValueEnum.Chaos2to10);
                }
            }
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
