using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Physic unit - used for crafting
/// </summary>
[Serializable]
public class PreUnit : Slotable
{
    public PreUnit()
    {
    }
    public PreUnit(string name, string description, string unit)
    {
        Name = name;
        Description = description;
        Unit = unit;
        unitNumeratorList.Add(this);
    }
    public string Name{get; private set; }
    public string Description { get; private set; }
    public string Unit { get; private set; }
    public List<PreUnit> unitNumeratorList = new List<PreUnit>();
    public List<PreUnit> unitDenominatorList = new List<PreUnit>();

    public PreUnit(string name, string description, string unit, List<PreUnit> numerator, List<PreUnit> denominator)
    {
        DecomposeUnit(numerator, denominator);
        Name = name;
        Description = description;
        Unit = unit;
    }

    private void DecomposeUnit(List<PreUnit> numerator, List<PreUnit> denominator)
    {
        for (int i = 0; i < numerator.Count; i++)
        {
            unitNumeratorList.AddRange(numerator[i].unitNumeratorList);
            unitDenominatorList.AddRange(numerator[i].unitDenominatorList);
        }
        for (int i = 0; i < denominator.Count; i++)
        {
            unitNumeratorList.AddRange(denominator[i].unitDenominatorList);
            unitDenominatorList.AddRange(denominator[i].unitNumeratorList);
        }
        Units.FractionReduction(ref unitNumeratorList, ref unitDenominatorList);

    }
    
}

