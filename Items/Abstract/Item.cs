﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ToolsSystem;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

public abstract class Item : ActionHandler
{
    public Item()
    {
    }

    public Item((float, float) pos, GameObject prefab, Tilemap map = null)
    {
        if (map == null)
        {
            map = GameObjects.solidLayer;
        }
        this.Prefab = prefab;
        this.prefabName = Prefab.name;
        InsertAtPosition(map, pos, false);
        SetupItem();
    }
    //Don't use this X, Y and other GameObject parameters for the game itself - use it only for saving and loading the game
    [JsonProperty]
    private float x;
    [JsonProperty]
    private float y;
    [JsonProperty]
    public float Rotation { get; set; }
    [JsonProperty]
    private string tilemapName;
    [JsonProperty]
    private string prefabName;
    [JsonIgnore]
    public GameObject Prefab { get; private set; }

    /// <summary>
    /// Whether to delete this Item when player leave the room
    /// </summary>
    public bool DeleteOnLeave { get; set; } = false;

    private GameObject InsertAtPosition(Tilemap map, (float, float) pos, bool loadAssign = false)
    {
        Prefab = UnityEngine.Object.Instantiate(Prefab);
        Prefab.SetActive(false);
        x = pos.Item2;
        y = pos.Item1;
        if (loadAssign)
            Prefab.transform.position = new Vector3(x, y);
        else
            Prefab.transform.position = map.WorldToCell(new Vector3(x, y));
        this.Id = this.Prefab.GetInstanceID();
        this.tilemapName = map.name;
        return Prefab;
    }
    /// <summary>
    /// Called on loading of the game
    /// </summary>
    public void AssignPrefab()
    {
        Prefab = GameObjects.GetPrefabByName(this.prefabName);
        InsertAtPosition(GameObjects.GetTilemapByName(tilemapName), (y, x), true);
        SetupItem();
        //Prefab.GetComponentInChildren<SpriteRenderer>().transform.up = Vector2.up;
        //Prefab.transform.rotation. =  
    }
    /// <summary>
    /// Whole time I use (preferably) only Prefab properties, which are saved here
    /// </summary>
    public virtual void SaveItem()
    {
        //Rotation = Prefab.transform.rotation;
        x = Prefab.transform.position.x;
        y = Prefab.transform.position.y;
    }
    /// <summary>
    /// This method is called in constructor and on load of the game.
    /// </summary>
    protected virtual void SetupItem()
    {
        GCon.game.Items.Add(Id, this);
        Rigidbody2D rb = Prefab.AddComponent<Rigidbody2D>();
        rb.simulated = true;
        rb.gravityScale = 0;
        ItemScript script = Prefab.AddComponent<ItemScript>();
        script.item = this;
    }

    public virtual void Dispose()
    {
        GCon.game.Items.Remove(this.Id);
        GCon.game.CurLevel.Items.Remove(this.Id);
        if (GCon.game.ItemsStep.ContainsKey(Id))
        {
            GCon.game.ItemsStep.Remove(Id);
        }

        UnityEngine.Object.Destroy(this.Prefab);
    }

    public virtual void OnCollisionEnter(Item collider)
    {
    }
    public virtual void OnCollisionLeave(Item collider)
    {
    }
    public virtual void OnLevelEnter()
    {
    }
}

