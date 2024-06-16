using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handler for collectibles
/// </summary>
public abstract class ICollectableRef
{
    public ICollectableRef() { }
    protected ICollectableRef(GameObject prefab)
    {
        Prefab = prefab;
        PrefabName = prefab.name;
    }
    [JsonIgnore]
    public GameObject Prefab { get; set; }
    public string PrefabName { get; set; }
    public virtual void AssignPrefab()
    {
        Prefab = GameObjects.GetPrefabByName(this.PrefabName);
    }
    public abstract ICollectableRef DeepClone();
}
