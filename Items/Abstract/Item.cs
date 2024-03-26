using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ToolsSystem;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

public abstract class Item
{
    public Item()
    {
    }
    public Item((float, float) pos, Collider2D collider = null, string spritePath = "", Tilemap map = null)
    {
        if (map == null)
        {
            map = GameObjects.solidLayer;
        }
        InsertAtPosition(map, pos, false);
        this.spritePath = spritePath;
        SetupItem();
    }
    [JsonProperty]
    public int Id { get; set; }
    //Don't use this X, Y and other GameObject parameters for the game itself - use it only for saving and loading the game
    [JsonProperty]
    public float X { get; set; }
    [JsonProperty]
    public float Y { get; set; }
    [JsonProperty]
    public float Rotation { get; set; }
    [JsonProperty]
    private string spritePath;
    [JsonProperty]
    private string tilemapName;
    [JsonIgnore]
    public GameObject Prefab { get; protected set; }
    /// <summary>
    /// Whether to delete this Item when player leave the room
    /// </summary>
    public bool DeleteOnLeave { get; set; } = false;

    private GameObject InsertAtPosition(Tilemap map, (float, float) pos, bool loadAssign = false)
    {
        var obj = UnityEngine.Object.Instantiate(GameObjects.empty);
        obj.SetActive(false);
        X = pos.Item2;
        Y = pos.Item1;
        if (loadAssign)
            obj.transform.position = new Vector3(X, Y);
        else
            obj.transform.position = map.WorldToCell(new Vector3(X, Y));
        this.Prefab = obj;
        this.Id = this.Prefab.GetInstanceID();
        this.tilemapName = map.name;
        return obj;
    }
    /// <summary>
    /// Called on loading of the game
    /// </summary>
    public void AssignPrefab()
    {
        InsertAtPosition(GameObjects.GetTilemapByName(tilemapName), (Y, X), true);
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
        X = Prefab.transform.position.x;
        Y = Prefab.transform.position.y;
    }
    protected virtual void SetupItem()
    {
        Rigidbody2D rb = Prefab.AddComponent<Rigidbody2D>();
        rb.simulated = true;
        Prefab.
    }

    public virtual void Dispose()
    {
        GlobalControl.game.CurLevel.GameObjects.Remove(this.Id);
        UnityEngine.Object.Destroy(this.Prefab);
    }

    public virtual void OnCollisionEnter(Collider2D collider)
    { 
    }
    public virtual void OnCollisionLeave(Collider2D collider)
    {
    }
    public virtual void Update()
    {
    }
    public virtual void OnLevelEnter()
    {
    }
}

