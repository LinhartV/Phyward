using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Physic unit - used for crafting - slotable
/// </summary>
[Serializable]
public class PreUnit : Slotable
{
    public PreUnit()
    {
    }
    public PreUnit(string name, string description, string unit, GameObject prefab) : base(name, unit, description, ToolsUI.FilterType.units, prefab, true)
    {
        unitNumeratorList.Add(this);
    }
    public List<PreUnit> unitNumeratorList = new List<PreUnit>();
    public List<PreUnit> unitDenominatorList = new List<PreUnit>();
    public List<PreUnit> originalUnitNumeratorList = new List<PreUnit>();
    public List<PreUnit> originalUnitDenominatorList = new List<PreUnit>();

    public PreUnit(string name, string description, string unit, List<PreUnit> numerator, List<PreUnit> denominator, GameObject prefab) : base(name, unit, description, ToolsUI.FilterType.units, prefab, true)
    {
        originalUnitNumeratorList = new List<PreUnit>(numerator);
        originalUnitDenominatorList = new List<PreUnit>(denominator);
        DecomposeUnit(numerator, denominator);
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

    public override ICollectableRef DeepClone()
    {
        return new PreUnit(this.Name, this.Description, this.Subheading, this.unitNumeratorList, this.unitDenominatorList, GameObjects.GetPrefabByName(PrefabName));
    }
}

