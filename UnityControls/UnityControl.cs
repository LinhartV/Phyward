using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ToolsGame;
using static UnityEditor.Progress;

/// <summary>
/// Just what it says...
/// </summary>
public class UnityControl : MonoBehaviour
{
    [SerializeField]
    Tilemap solidMap;
    [SerializeField]
    Tilemap backgroundMap;
    [SerializeField]
    Tilemap backgroundDecorationsMap;
    [SerializeField]
    TileBase wallTile;
    [SerializeField]
    private GameObject empty;
    [SerializeField]
    private GameObject blueShot;
    [SerializeField]
    private GameObject redSmallShot;
    [SerializeField]
    private GameObject fireSwarmShot;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    public GameObject exit;
    [SerializeField]
    public GameObject time;
    [SerializeField]
    public GameObject speed;
    [SerializeField]
    public GameObject length;
    [SerializeField]
    public GameObject frequency;
    [SerializeField]
    public GameObject mass;
    [SerializeField]
    public GameObject purpleEnemy;
    [SerializeField]
    public GameObject healthBarStandard;
    [SerializeField]
    public GameObject redSmallEnemy;
    [SerializeField]
    public GameObject slot;
    [SerializeField]
    public GameObject unitAnimation;
    [SerializeField]
    public Texture2D normalCursor;
    [SerializeField]
    public Texture2D selectCursor;
    [SerializeField]
    public Texture2D holdCursor;
    [SerializeField]
    public Texture2D aimCursor;
    [SerializeField]
    public GameObject counter;
    [SerializeField]
    public GameObject slingshot;
    [SerializeField]
    public GameObject blowgun;
    [SerializeField]
    public GameObject sling;
    [SerializeField]
    public GameObject baseHouse;
    [SerializeField]
    public GameObject craftable;
    [SerializeField]
    public GameObject crumblingRock;
    [SerializeField]
    public GameObject burningRock;

    // Start is called before the first frame update
    void Start()
    {
        ToolsUI.holdCursor = holdCursor; ToolsUI.selectCursor = selectCursor; ToolsUI.normalCursor = normalCursor; ToolsUI.aimCursor = aimCursor;
        GameObjects.SetPrefabs(burningRock, crumblingRock, craftable, baseHouse, sling, blowgun, slingshot, counter, unitAnimation, slot, speed, frequency, mass, length, redSmallEnemy, healthBarStandard, purpleEnemy, time, redSmallShot, fireSwarmShot, exit, empty, player, blueShot, solidMap);
        ToolsUI.SetCursor(aimCursor);
        ToolsGame.SetupGame();
        ToolsSystem.StartGame("Try");
        BuildLevel(GCon.game.CurLevel, null, GCon.game.Player.Prefab.transform.position);
        var camera = GameObject.FindGameObjectWithTag("MainCamera");
        camera.GetComponent<CameraFollow>().target = GCon.game.Player.Prefab.transform;
        GCon.GameStarted = true;
        GCon.game.Player.Prefab.SetActive(true);
        OnResize();
    }

    private void Update()
    {
        KeyPress();
        KeyRelease();
        //OnResize();
    }
    private void FixedUpdate()
    {
        if (GCon.GameStarted)
        {
            if (!GCon.Paused)
            {

                Dictionary<int, ActionHandler> temp = new Dictionary<int, ActionHandler>(GCon.game.ItemsStep);
                foreach (ActionHandler item in temp.Values)
                {
                    item.ExecuteActions(GCon.game.Now);
                }
                List<ActionHandler> tempDestroyed = new List<ActionHandler>(GCon.game.ItemsToBeDestroyed);
                foreach (var item in tempDestroyed)
                {
                    item.InnerDispose();
                }
                GCon.game.ItemsToBeDestroyed.Clear();
                List<Item> tempInactive = new List<Item>(GCon.game.ItemsToBeSetInactive);
                foreach (var item in tempInactive)
                {
                    if (item.IsTriggered == false)
                    {
                        item.Prefab.SetActive(false);
                        GCon.game.ItemsToBeSetInactive.Remove(item);
                    }
                }
                GCon.game.Now++;
            }

            List<ActionHandler> tempDestroyedUI = new List<ActionHandler>(ToolsUI.UIItemsToBeDestroyed);
            foreach (var item in tempDestroyedUI)
            {
                item.InnerDispose();
            }
            ToolsUI.UIItemsToBeDestroyed.Clear();
            List<ActionHandler> tempUI = new List<ActionHandler>(ToolsUI.UIItemsStep);
            foreach (ActionHandler item in tempUI)
            {
                item.ExecuteActions(ToolsUI.nowUI);
            }
            ToolsUI.TransitionTransitables(Time.deltaTime);
            ToolsUI.nowUI++;
            //Debug.Log(GCon.game.Now);
        }

    }
    float prevWidth = 0;
    float prevHeight = 0;
    private void OnResize()
    {
        if (prevWidth != Screen.width || prevHeight != Screen.height)
        {
            ToolsUI.OnResize();
        }
    }

    void OnApplicationQuit()
    {
        ToolsSystem.SaveGame(GCon.game);
    }

    private void KeyPress()
    {
        foreach (var key in KeyController.registeredKeys)
        {
            try
            {
                if (Input.GetKey(key.Key))
                {
                    KeyController.GetRegisteredKey(key.Key)?.KeyDown();
                    KeyController.SetPressedStateToOtherSameKeys(key.Key, true);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
    private void KeyRelease()
    {
        foreach (var key in KeyController.registeredKeys)
        {
            if (Input.GetKeyUp(key.Key))
            {
                KeyController.GetRegisteredKey(key.Key)?.KeyUp();
                KeyController.SetPressedStateToOtherSameKeys(key.Key, false);
            }
        }
    }

    public void BuildLevel(Level level, Level prevLevel, Vector2 playerPos)
    {
        if (level == null)
        {
            return;
        }
        solidMap.ClearAllTiles();
        MazeTile[,] maze = level.Maze;

        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                switch (maze[i, j])
                {
                    case MazeTile.empty:

                        break;
                    case MazeTile.block:
                        InsertTile(wallTile, solidMap, j, i);
                        break;
                    default:
                        break;
                }
            }
        }
        if (prevLevel != null)
        {
            var temp = new List<Item>(prevLevel.Items.Values);
            foreach (var obj in temp)
            {
                obj.OnLevelLeave();
                if (!obj.IsInLevel)
                {
                    GCon.game.ItemsToBeSetInactive.Add(obj);
                }
            }
        }
        foreach (var obj in level.Items.Values)
        {
            obj.Prefab.SetActive(true);
            obj.OnLevelEnter();
        }
        GCon.game.Player.Prefab.transform.position = new Vector3(playerPos.x, playerPos.y, GCon.game.Player.Prefab.transform.position.z);
    }
    private void InsertTile(TileBase tile, Tilemap map, int x, int y)
    {
        var tilePosition = solidMap.WorldToCell(new Vector3(x, y));
        map.SetTile(tilePosition, tile);
    }

}
