
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEditor.Progress;

public static class ToolsUI
{
    public static PlayerInventory playerInventory;
    public static WrapPanel wrapPanel;
    public static BaseInventory baseInventory;
    public static UnitCraftInventory unitCraftInventory;
    private static Inventory activeInventory;
    public static Inventory ActiveInventory
    {
        get { return activeInventory; }
        set
        {
            /*if (activeInventory != value)
            {
                   if (openInventories.Count < 2)
                   {
                       if (activeInventory != null)
                       {
                           activeInventory.panel.Go.SetActive(false);
                       }
                   }
            }*/
            if (openInventories.Count < 2)
            {
                if (activeInventory != null)
                {
                    activeInventory.panel.Go.SetActive(false);
                }
            }
            activeInventory = value;
            activeInventory.panel.Go.SetActive(true);
            activeInventory.panel.Go.transform.SetAsLastSibling();
        }
    }
    public enum FilterType { all, weapons, units, bonuses, armor }
    public static Texture2D normalCursor;
    public static Texture2D selectCursor;
    public static Texture2D holdCursor;
    public static Texture2D aimCursor;
    public static AnimationCurve linear = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
    public static AnimationCurve easeIn = new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 2, 2));
    public static AnimationCurve easeOut = new AnimationCurve(new Keyframe(0, 0, 2, 2), new Keyframe(1, 1, 0, 0));
    public static UIItem descriptionPanel;
    /// <summary>
    /// Literally all baseSlots on scene
    /// </summary>
    public static List<SlotTemplate> allSlots = new List<SlotTemplate>();
    /// <summary>
    /// slot which item is being dragged
    /// </summary>
    public static SlotTemplate draggedSlot;
    public static SlotTemplate DraggedSlot
    {
        get { return draggedSlot; }
        set
        {
            draggedSlot = value;
            if (value != null)
            {
                StartDragging();
            }
        }
    }
    public static bool draggedSlotBeingReturnedFromUnsuccessfulDrag;
    public static SlotTemplate hoveredWhileDragging;
    public static FilterType filter = FilterType.all;

    public static long nowUI = 0;

    /// <summary>
    /// List of all items undergoing lerp transition
    /// </summary>
    [JsonIgnore]
    public static List<UIItem> transitables = new List<UIItem>();
    /// <summary>
    /// List of all UIitems for action handling
    /// </summary>
    [JsonIgnore]
    public static List<UIItem> UIItems = new List<UIItem>();
    /// <summary>
    /// List of all UIitems for action handling
    /// </summary>
    [JsonIgnore]
    public static List<ActionHandler> UIItemsStep = new List<ActionHandler>();
    /// <summary>
    /// List of all UIitems for action handling
    /// </summary>
    [JsonIgnore]
    public static List<UIItem> UIItemsToBeDestroyed = new List<UIItem>();
    public static Stack<Inventory> openInventories = new Stack<Inventory>();

    public static void TransitionTransitables(float deltaTime)
    {
        List<UIItem> temp = new List<UIItem>(transitables);
        foreach (var transitable in temp)
        {
            if (transitable.Go != null)
            {
                transitable.ProcedeTransitions(deltaTime);
            }
            else
            {
                transitables.Remove(transitable);
            }
        }
    }

    public static void StartDragging()
    {

    }
    /// <summary>
    /// When player presses key to interact with inventory
    /// </summary>
    /// <param name="closing">Whether it's triggered only for closing</param>
    public static void TriggerInventory(bool closing)
    {
        if (openInventories.Count > 0)
        {
            ActiveInventory.CloseInventory();
        }
        else if (!closing)
        {
            activeInventory.OpenInventory();
        }
    }

    public static void SetCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, new Vector2(16f, 0), CursorMode.Auto);
    }



    public static void DropDraggedObject()
    {
        if (ToolsUI.DraggedSlot != null && draggedSlotBeingReturnedFromUnsuccessfulDrag == false)
        {
            ToolsUI.DraggedSlot.StopDragging();
            hoveredWhileDragging = null;
            ResetDropables();
        }
    }

    public static void CheckForDropability()
    {
        foreach (var item in allSlots)
        {
            item.CheckForDropability();
        }
    }
    public static void ResetDropables()
    {
        foreach (var slot in allSlots)
        {
            slot.ResetDropable();
        }
    }
    public static void AnimateSettingSlotable(float time, Vector2 pos, GameObject formerPrefab, Action onAnimationEnd, bool fromInventory)
    {
        var colAnimGo = UnityEngine.Object.Instantiate(formerPrefab);
        /*colAnimGo.GetComponent<SpriteRenderer>().sprite = formerPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
        colAnimGo.GetComponent<SpriteRenderer>().drawMode = formerPrefab.GetComponentInChildren<SpriteRenderer>().drawMode;*/
        Component.Destroy(colAnimGo.GetComponent<Rigidbody2D>());
        Component.Destroy(colAnimGo.GetComponent<Collider2D>());
        colAnimGo.transform.position = formerPrefab.transform.position;
        colAnimGo.transform.parent = ToolsUI.wrapPanel.Go.transform;
        colAnimGo.transform.localScale = formerPrefab.transform.localScale;
        if (!fromInventory)
        {
            colAnimGo.transform.localScale = new Vector3(formerPrefab.transform.localScale.x / GameObject.FindGameObjectWithTag("Inventory").transform.localScale.x, formerPrefab.transform.localScale.y / GameObject.FindGameObjectWithTag("Inventory").transform.localScale.y);
        }
        var colAnimUI = new UIItem(colAnimGo);
        colAnimUI.AddTransition(new Lerpable(time, pos, ToolsUI.easeOut, false, true, () =>
        {
            if (onAnimationEnd != null)
            {
                onAnimationEnd();
            }
            //ToolsUI.ActiveInventory.UpdateInventory();

            colAnimUI.Dispose();
            draggedSlotBeingReturnedFromUnsuccessfulDrag = false;
        }, null));
        colAnimUI.StartTransition();
    }

    public static void SetupUI()
    {
        wrapPanel = new WrapPanel();
        playerInventory = new PlayerInventory(new UIItem(GameObject.FindGameObjectWithTag("PlayerInventory")));
        baseInventory = new BaseInventory(new UIItem(GameObject.FindGameObjectWithTag("BaseInventory")));
        unitCraftInventory = new UnitCraftInventory(new UIItem(GameObject.FindGameObjectWithTag("UnitCraft")));
        descriptionPanel = new UIItem(GameObject.FindGameObjectWithTag("DescriptionPanel"));
        descriptionPanel.Go.SetActive(false);
        baseInventory.panel.Go.SetActive(false);
        ToolsUI.ActiveInventory = playerInventory;
    }




    public static void OnResize()
    {
        /*var outline = GameObject.FindGameObjectWithTag("Outline").GetComponent<LineRenderer>();
        float width = outline.GetComponent<RectTransform>().rect.width;
        float height = outline.GetComponent<RectTransform>().rect.height;
        outline.SetPositions(new Vector3[] { new Vector3(0, height, 1), new Vector3(0, 0, 1), new Vector3(width, 0, 1), new Vector3(width, height, 1) });*/

        /*playerInventory.Go.GetComponent<RectTransform>().transform.localPosition = new Vector2(0, GCon.inventoryOpen ? inventoryUpY : inventoryDownY);
        var panel = GameObject.FindGameObjectWithTag("InventoryPanel").GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);*/
    }

    public static void LoadUI()
    {
        playerInventory.LoadUI();
        baseInventory.LoadUI();
        wrapPanel.LoadUI();
    }



}
