using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ToolsGame;

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

    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {

        ToolsGame.SetupGame();
        GameObjects.SetPrefabs(empty, solidMap);
        GlobalControl.StartGame("Try");
        player = GlobalControl.game.Player.Prefab;
        player.SetActive(true);
        //player = GlobalControl.game.CurLevel?.InsertAtEmptyPosition(player, solidMap);


        BuildLevel(GlobalControl.game.CurLevel, null);
        var camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (player != null)
        {
            camera.GetComponent<CameraFollow>().target = player.transform;
        }
    }

    void OnApplicationQuit()
    {
        ToolsSystem.SaveGame(GlobalControl.game);
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
            foreach (var obj in prevLevel.GameObjects.Values)
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
        foreach (var obj in level.GameObjects.Values)
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
