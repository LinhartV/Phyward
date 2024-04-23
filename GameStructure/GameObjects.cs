using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class GameObjects
{
    private static Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();
    private static Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    public static Tilemap solidLayer;
    public static GameObject empty;
    public static GameObject player;
    public static GameObject exit;
    public static GameObject blueShot;
    public static GameObject redSmallShot;
    public static GameObject fireSwarmShot;
    public static GameObject time;
    public static GameObject speed;
    public static GameObject frequency;
    public static GameObject mass;
    public static GameObject length;
    public static GameObject purpleEnemy;
    public static GameObject healthBarStandard;
    public static GameObject redSmallEnemy; 


    public static void SetPrefabs(GameObject _speed, GameObject _frequency, GameObject _mass, GameObject _length, GameObject _redSmallEnemy, GameObject _healthBarStandard, GameObject _purpleEnemy, GameObject _time, GameObject _redSmallShot, GameObject _fireSwarmShot, GameObject _exit, GameObject _empty, GameObject _player, GameObject _blueShot, Tilemap _solidLayer)
    {
        tilemaps.Add(_solidLayer.name, _solidLayer);
        prefabs.Add(_empty.name, _empty);
        prefabs.Add(_exit.name, _exit);
        prefabs.Add(_player.name, _player);
        prefabs.Add(_blueShot.name, _blueShot);
        prefabs.Add(_redSmallShot.name, _redSmallShot);
        prefabs.Add(_fireSwarmShot.name, _fireSwarmShot);
        prefabs.Add(_healthBarStandard.name, _healthBarStandard);
        prefabs.Add(_redSmallEnemy.name, _redSmallEnemy);
        prefabs.Add(_speed.name, _speed);
        prefabs.Add(_frequency.name, _frequency);
        prefabs.Add(_mass.name, _mass);
        prefabs.Add(_length.name, _length);
        healthBarStandard = _healthBarStandard;
        redSmallShot = _redSmallShot;
        fireSwarmShot = _fireSwarmShot;
        empty = _empty;
        exit = _exit;
        player = _player;
        blueShot = _blueShot;
        solidLayer = _solidLayer;
        time = _time;
        purpleEnemy = _purpleEnemy;
        redSmallEnemy = _redSmallEnemy;
        speed = _speed;
        frequency = _frequency;
        mass = _mass;
        length = _length;
    }
    public static GameObject GetPrefabByName(string name)
    {
        return prefabs[name];
    }
    public static Tilemap GetTilemapByName(string name)
    {
        return tilemaps[name];
    }

}

