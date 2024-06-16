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
    public static GameObject slot;
    public static GameObject unitAnimation;
    public static GameObject text;
    public static GameObject slingshot;
    public static GameObject sling;
    public static GameObject blowgun;
    public static GameObject baseHouse;
    public static GameObject craftable;
    public static GameObject crumblingRock;
    public static GameObject burningRock;
    public static GameObject scroll;
    public static GameObject area;
    public static GameObject volume;
    public static GameObject inertia;
    public static GameObject craftingScroll;
    public static GameObject medkit;


    public static void SetPrefabs(GameObject _medkit, GameObject _craftingScroll, GameObject _inertia, GameObject _volume, GameObject _area, GameObject _scroll, GameObject _burningRock, GameObject _crumblingRock,GameObject _craftable, GameObject _baseHouse, GameObject _sling, GameObject _blowgun, GameObject _slingshot,GameObject _counter, GameObject _unitAnimation, GameObject _slot, GameObject _speed, GameObject _frequency, GameObject _mass, GameObject _length, GameObject _redSmallEnemy, GameObject _healthBarStandard, GameObject _purpleEnemy, GameObject _time, GameObject _redSmallShot, GameObject _fireSwarmShot, GameObject _exit, GameObject _empty, GameObject _player, GameObject _blueShot, Tilemap _solidLayer)
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
        prefabs.Add(_slot.name, _slot);
        prefabs.Add(_time.name, _time);
        prefabs.Add(_unitAnimation.name, _unitAnimation);
        prefabs.Add(_counter.name, _counter);
        prefabs.Add(_slingshot.name, _slingshot);
        prefabs.Add(_blowgun.name, _blowgun);
        prefabs.Add(_sling.name, _sling);
        prefabs.Add(_baseHouse.name, _baseHouse);
        prefabs.Add(_purpleEnemy.name, _purpleEnemy);
        prefabs.Add(_craftable.name, _craftable);
        prefabs.Add(_crumblingRock.name, _crumblingRock);
        prefabs.Add(_burningRock.name, _burningRock);
        prefabs.Add(_scroll.name, _scroll);
        prefabs.Add(_area.name, _area);
        prefabs.Add(_volume.name, _volume);
        prefabs.Add(_inertia.name, _inertia);
        prefabs.Add(_craftingScroll.name, _craftingScroll);
        prefabs.Add(_medkit.name, _medkit);
        medkit = _medkit;
        craftingScroll = _craftingScroll;
        inertia = _inertia;
        volume = _volume;
        area = _area;
        scroll = _scroll;
        burningRock = _burningRock;
        crumblingRock = _crumblingRock;
        craftable = _craftable;
        baseHouse = _baseHouse;
        sling = _sling;
        blowgun = _blowgun;
        slingshot = _slingshot;
        text = _counter;
        unitAnimation = _unitAnimation;
        slot = _slot;
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

