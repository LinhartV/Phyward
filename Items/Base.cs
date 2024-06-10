using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Class for managing exits from a particular level.
/// </summary>
[Serializable]
public class Base : Item, IInteractable
{
    public Base() { }

    public Base(Vector2 pos, Tilemap map = null) : base(pos, GameObjects.baseHouse, false, map)
    {
        this.Prefab.transform.position = new Vector3(pos.x, pos.y, 1);
    }

    public void Interact()
    {
        ToolsUI.baseInventory.OpenInventory();

    }
}

