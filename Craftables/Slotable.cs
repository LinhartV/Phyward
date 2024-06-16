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
public abstract class Slotable : ICollectableRef, IComparable<Slotable>
{

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
    /// <summary>
    /// Whether there can be more duplicates on this slot
    /// </summary>
    public bool Stackable { get; protected set; }
    /// <summary>
    /// Whether to exchange slot with other Slotable on drop
    /// </summary>
    public bool Exchangable { get; protected set; }

    public Slotable() { }

    public Slotable(string name, string subheading, string description, ToolsUI.FilterType filter, GameObject prefab, bool stackable = false, bool exchangable = false) : base(prefab)
    {
        this.filter = filter;
        Description = description;
        Name = name;
        Subheading = subheading;
        Stackable = stackable;
        Count = 1;
        Exchangable = exchangable;
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
                    return PrefabName.CompareTo(other.PrefabName);
                }
                else
                    return -1;
            }//Two craftableSlots
            else if (this is Craftable tc && other is Craftable oc)
            {
                if (tc.Tier == oc.Tier)
                {
                    return PrefabName.CompareTo(other.PrefabName);
                }
                else
                    return tc.Tier.CompareTo(oc.Tier);
            }
            else
            {
                return PrefabName.CompareTo(other.PrefabName);
            }
        }
        else
        {
            return this.filter.CompareTo(other.filter);
        }
    }
}

