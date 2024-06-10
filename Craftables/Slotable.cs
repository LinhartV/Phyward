using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

/// <summary>
/// Everything I can have in player slot
/// </summary>
[Serializable]
public abstract class Slotable : IComparable<Slotable>
{
    [JsonIgnore]
    public GameObject Prefab { get; set; }
    [JsonProperty]
    private string prefabName;
    [JsonProperty]
    public ToolsUI.FilterType filter { get; private set; }
    public string Description { get; private set; }
    /// <summary>
    /// Primary key for identification - also displayed name
    /// </summary>
    public string Name { get; private set; }
    public string Subheading { get; private set; }
    /// <summary>
    /// Count of how many occurences of this slotable are here if stackable
    /// </summary>
    private int count;
    public int Count { get => count; set { count = value; } }
    public bool Stackable { get; protected set; }
    public Slotable() { }

    public Slotable(string name, string subheading, string description, ToolsUI.FilterType filter, GameObject prefab, bool stackable = false)
    {
        this.filter = filter;
        Prefab = prefab;
        prefabName = prefab.name;
        Description = description;
        Name = name;
        Subheading = subheading;
        Stackable = stackable;
        Count = 1;
    }
    public virtual void AssignPrefab()
    {
        Prefab = GameObjects.GetPrefabByName(this.prefabName);
    }


    public int CompareTo(Slotable other)
    {
        if (this.filter == other.filter)
        {
            //two units
            if (this is PreUnit tp && other is PreUnit op)
            {
                if (tp.unitNumeratorList.Count + tp.unitDenominatorList.Count > op.unitNumeratorList.Count + op.unitDenominatorList.Count)
                    return 1;
                else if (tp.unitNumeratorList.Count + tp.unitDenominatorList.Count == op.unitNumeratorList.Count + op.unitDenominatorList.Count)
                {
                    return prefabName.CompareTo(other.prefabName);
                }
                else
                    return -1;
            }//Two craftableSlots
            else if (this is Craftable tc && other is Craftable oc)
            {
                if (tc.Tier == oc.Tier)
                {
                    return prefabName.CompareTo(other.prefabName);
                }
                else
                    return tc.Tier.CompareTo(oc.Tier);
            }
            else
            {
                return prefabName.CompareTo(other.prefabName);
            }
        }
        else
        {
            return this.filter.CompareTo(other.filter);
        }
    }
}

