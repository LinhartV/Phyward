using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Everything I can have in player slot
/// </summary>
[Serializable]
public abstract class Slotable
{
    [JsonIgnore]
    public Sprite Img { get; private set; }
    [JsonProperty]
    private string spriteName;
    public Slotable() { }

    public Slotable(string spriteName)
    {
        this.spriteName = spriteName;
        Img = Resources.Load(spriteName) as Sprite;
    }
    public virtual void SetupSlotable()
    {
        Img = Resources.Load(spriteName) as Sprite;
    }
}

