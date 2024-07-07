using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
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
    [SerializeField]
    public GameObject scroll;
    [SerializeField]
    public GameObject area;
    [SerializeField]
    public GameObject volume;
    [SerializeField]
    public GameObject inertia;
    [SerializeField]
    public GameObject craftingScroll;
    [SerializeField]
    public GameObject medkit;
    [SerializeField]
    public GameObject basicArmor;
    [SerializeField]
    public GameObject block;
    [SerializeField]
    public GameObject blackShot;
    [SerializeField]
    public GameObject blowgunShot;
    [SerializeField]
    public GameObject boss1;
    [SerializeField]
    public GameObject shootingEnemy;
    [SerializeField]
    public GameObject pebble;
    [SerializeField]
    public GameObject justBlock;
    [SerializeField]
    public GameObject bandage;
    [SerializeField]
    public GameObject boss2;
    [SerializeField]
    public GameObject swarmEnemy;
    [SerializeField]
    public GameObject density;
    [SerializeField]
    public GameObject acceleration;
    [SerializeField]
    public GameObject force;
    [SerializeField]
    public GameObject leatherArmor;
    [SerializeField]
    public GameObject fastReload;
    [SerializeField]
    public GameObject speedUp;
    [SerializeField]
    GameObject finalBoss;
    [SerializeField]
    GameObject acceleratedHealing;
    [SerializeField]
    GameObject portal;

    [SerializeField]
    AudioClip clip1;
    [SerializeField]
    AudioClip clip2;
    [SerializeField]
    AudioClip clip3;
    [SerializeField]
    AudioSource audioSource;
    private List<AudioClip> clips;
    private List<AudioClip> remainingClips;

    private bool frameEnded;

    // Start is called before the first frame update
    void Start()
    {

        ToolsUI.holdCursor = holdCursor; ToolsUI.selectCursor = selectCursor; ToolsUI.normalCursor = normalCursor; ToolsUI.aimCursor = aimCursor;
        GameObjects.SetPrefabs(portal, finalBoss, acceleratedHealing, fastReload, speedUp, leatherArmor, force, acceleration, density, swarmEnemy, boss2, bandage, justBlock, pebble, blackShot, blowgunShot, boss1, shootingEnemy, block, basicArmor, medkit, craftingScroll, inertia, volume, area, scroll, burningRock, crumblingRock, craftable, baseHouse, sling, blowgun, slingshot, counter, unitAnimation, slot, speed, frequency, mass, length, redSmallEnemy, healthBarStandard, purpleEnemy, time, redSmallShot, fireSwarmShot, exit, empty, player, blueShot, solidMap, wallTile);
        ToolsUI.SetCursor(aimCursor);
        ToolsGame.SetupGame();
        ToolsSystem.StartGame("Game");
        var camera = GameObject.FindGameObjectWithTag("MainCamera");
        camera.GetComponent<CameraFollow>().target = GCon.game.Player.Prefab.transform;
        GCon.GameStarted = true;
        GCon.game.Player.Prefab.SetActive(true);
        OnResize();
        frameEnded = true;
        clips = new List<AudioClip>() { clip1, clip2, clip3 };
        remainingClips = new List<AudioClip>(clips);
        audioSource = GameObject.Instantiate(audioSource);
    }

    private void Update()
    {
        KeyPress();
        KeyRelease();
        AudioHandling();
        //OnResize();
    }
    private void FixedUpdate()
    {
        if (frameEnded)
        {
            frameEnded = false;
            if (GCon.GameStarted)
            {
                List<ActionHandler> temp = new List<ActionHandler>(GCon.gameSystems[GCon.GetPausedType()].itemStep);
                foreach (ActionHandler item in temp)
                {
                    item.ExecuteActions(GCon.gameSystems[GCon.GetPausedType()].NowDifference);
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
                ToolsUI.TransitionTransitables(Time.deltaTime);
                GCon.game.Now++;
                GCon.gameSystems[GCon.GetPausedType()].NowDifference++;
            }
            frameEnded = true;
        }
        else
            Debug.Log("Frame execution out of order caught");

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

    private void AudioHandling()
    {
        if (!audioSource.isPlaying)
        {
            if (remainingClips.Count == 0)
            {
                remainingClips = new List<AudioClip>(clips);
            }
            int i = ToolsGame.Rng(0, remainingClips.Count);
            audioSource.clip = remainingClips[i];
            remainingClips.RemoveAt(i);
            audioSource.Play();
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

}
