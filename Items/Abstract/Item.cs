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

    public Item((float, float) pos, GameObject prefab, bool isSolid = false, Tilemap map = null) : base(true)
    {
        this.prefabName = prefab.name;
        this.IsSolid = isSolid;
        Prefab = UnityEngine.Object.Instantiate(prefab);
        InsertAtPosition(pos, GCon.gameStarted, false, map);
        GCon.game.Items.Add(Id, this);
        SetupItem();
    }
    public Item(GameObject prefab, bool isSolid = false) : base(true)
    {
        this.prefabName = prefab.name;
        this.IsSolid = isSolid;
        GCon.game.Items.Add(Id, this);
        Prefab = UnityEngine.Object.Instantiate(prefab);
        Prefab.SetActive(false);
        SetupItem();
    }


    // Angle where the character is "looking" (for picture, shooting and stuff)
    [JsonProperty]
    private float angle;
    public float Angle
    {
        get => angle;
        set
        {
            angle = (float)(value % (Math.PI * 2));
        }
    }
    //Don't use this X, Y and other GameObject parameters for the game itself - use it only for saving and loading the game
    [JsonProperty]
    private float x;
    [JsonProperty]
    private float y;
    [JsonProperty]
    private string tilemapName;
    [JsonProperty]
    private string prefabName;
    [JsonProperty]
    public bool IsSolid { get; set; }
    [JsonIgnore]
    public GameObject Prefab { get; private set; }
    [JsonIgnore]
    protected Rigidbody2D rb;

    /// <summary>
    /// Whether to delete this Item when player leave the room
    /// </summary>
    public bool DeleteOnLeave { get; set; } = false;

    public GameObject InsertAtPosition((float, float) pos, Tilemap map = null)
    {
        return InsertAtPosition(pos, this.Prefab.activeInHierarchy, false, map);
    }
    public GameObject InsertAtPosition((float, float) pos, bool setActive, bool loadAssign = false, Tilemap map = null)
    {
        if (map == null)
        {
            map = GameObjects.solidLayer;
        }
        Prefab.SetActive(setActive);
        x = pos.Item2;
        y = pos.Item1;
        if (loadAssign || GCon.gameStarted)
            Prefab.transform.position = new Vector3(x, y, Prefab.transform.position.z);
        else
        {
            var z = Prefab.transform.position.z;
            Prefab.transform.position = map.WorldToCell(new Vector3(x, y, Prefab.transform.position.z));
            Prefab.transform.position = new Vector3(Prefab.transform.position.x, Prefab.transform.position.y, z);
        }
        this.tilemapName = map.name;
        return Prefab;
    }
    public GameObject InsertAtPosition(Vector2 pos, bool setActive, bool loadAssign = false, Tilemap map = null)
    {
        return InsertAtPosition((pos.y, pos.x), setActive, loadAssign, map);
    }
    /// <summary>
    /// Called on loading of the game
    /// </summary>
    public void AssignPrefab()
    {
        Prefab = UnityEngine.Object.Instantiate(GameObjects.GetPrefabByName(this.prefabName));
        InsertAtPosition((y, x), false, true, GameObjects.GetTilemapByName(tilemapName));
        SetupItem();
        //Prefab.GetComponentInChildren<SpriteRenderer>().transform.up = Vector2.up;
        //Prefab.transform.rotation. =  
    }
    public override void SaveItem()
    {
        base.SaveItem();
        x = Prefab.transform.position.x;
        y = Prefab.transform.position.y;
    }
    /// <summary>
    /// This method is called in constructor and on load of the game.
    /// </summary>
    protected virtual void SetupItem()
    {
        rb = Prefab.AddComponent<Rigidbody2D>();
        rb.simulated = true;
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        Prefab.GetComponent<Collider2D>().isTrigger = !IsSolid;
        ItemScript script = Prefab.AddComponent<ItemScript>();
        script.item = this;
    }

    public override void Dispose()
    {
        base.Dispose();
        GCon.game.Items.Remove(this.Id);
        GCon.game.CurLevel.Items.Remove(this.Id);
        UnityEngine.Object.Destroy(this.Prefab);
    }

    public virtual void OnCollisionEnter(Item collider)
    {
        if (!GCon.gameStarted)
            return;
    }
    public virtual void OnCollisionLeave(Item collider)
    {
        if (!GCon.gameStarted)
            return;
    }
    public override void OnLevelLeave()
    {
        base.OnLevelLeave();
        if (DeleteOnLeave)
        {
            Dispose();
        }
    }
}

