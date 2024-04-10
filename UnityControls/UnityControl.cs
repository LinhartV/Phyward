using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private GameObject player;
    [SerializeField]
    public GameObject exit;
    // Start is called before the first frame update
    void Start()
    {

        ToolsGame.SetupGame();
        GameObjects.SetPrefabs(exit, empty, player, blueShot, solidMap);
        ToolsSystem.StartGame("Try");
        GCon.game.Player.Prefab.SetActive(true);
        BuildLevel(GCon.game.CurLevel, null);
        var camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (player != null)
        {
            camera.GetComponent<CameraFollow>().target = GCon.game.Player.Prefab.transform;
        }
        GCon.gameStarted = true;
    }
    private void Update()
    {
        KeyPress();
        KeyRelease();
    }
    private void FixedUpdate()
    {
        if (GCon.gameStarted)
        {
            Dictionary<int, ActionHandler> temp = new Dictionary<int, ActionHandler>(GCon.game.ItemsStep);
            foreach (ActionHandler item in temp.Values)
            {
                item.ExecuteActions(GCon.game.Now);
            }
            GCon.game.Now++;
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
            if (Input.GetKey(key.Key))
            {
                key.Value.Peek().KeyDown();
            }
        }
    }
    private void KeyRelease()
    {
        foreach (var key in KeyController.registeredKeys)
        {
            if (Input.GetKeyUp(key.Key))
            {
                key.Value.Peek().KeyUp();
            }
        }
    }

    public void BuildLevel(Level level, Level prevLevel)
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
                if (obj.Prefab.tag != "Player")
                {
                    obj.Prefab.SetActive(false);
                    if (obj.DeleteOnLeave)
                    {
                        obj.Dispose();
                    }
                }
            }
        }
        foreach (var obj in level.Items.Values)
        {
            obj.Prefab.SetActive(true);
            obj.OnLevelEnter();
        }
    }
    private void InsertTile(TileBase tile, Tilemap map, int x, int y)
    {
        var tilePosition = solidMap.WorldToCell(new Vector3(x, y));
        map.SetTile(tilePosition, tile);
    }

}
