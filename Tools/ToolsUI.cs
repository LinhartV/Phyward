
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
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
    public static UIItem scrollPanel;
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
    /// List of all UIItems
    /// </summary>
    [JsonIgnore]
    public static List<UIItem> UIItems = new List<UIItem>();
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
    public static void AnimateSettingSlotable(float time, GameObject finalPos, GameObject formerPrefab, Action onAnimationEnd, bool fromInventory)
    {
        AnimateSettingSlotable(time, ToolsUI.wrapPanel.Go.transform.InverseTransformPoint(finalPos.transform.position), formerPrefab, onAnimationEnd, fromInventory);
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
        scrollPanel = new UIItem(GameObject.FindGameObjectWithTag("ScrollPanel"));
        scrollPanel.Go.GetComponent<CanvasGroup>().alpha = 0;
        scrollPanel.AddTransition(new Transparentable(0.5f, 1, null, false, () =>
        {
            GCon.game.gameActionHandler.AddAction(new ItemAction((ActionHandler item, object[] parameters) => { scrollPanel.ReturnTransition("show"); }, ToolsMath.SecondsToFrames(2), ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning));
        }, () =>
        {
            GCon.PopPausedType();
            scrollPanel.Go.SetActive(false);
        }), "show");
        scrollPanel.Go.SetActive(false);
        descriptionPanel.Go.SetActive(false);
        baseInventory.panel.Go.SetActive(false);
        ToolsUI.ActiveInventory = playerInventory;
    }

    public static void TriggerScrollPanel(PreUnit unit)
    {
        scrollPanel.Go.SetActive(true);
        ToolsUI.scrollPanel.Go.transform.position = Vector3.zero;
        ToolsUI.scrollPanel.Go.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = unit.Prefab.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        ToolsUI.scrollPanel.Go.transform.GetChild(2).gameObject.GetComponent<Image>().preserveAspect = true;
        ToolsUI.scrollPanel.Go.transform.GetChild(4).gameObject.SetActive(true);
        ToolsUI.scrollPanel.Go.transform.GetChild(3).localPosition = new Vector3(117.47f, 117.8f, 0);
        ToolsUI.scrollPanel.Go.transform.GetChild(1).localPosition = new Vector3(-232.82f, -0.17f, 0);
        foreach (Transform child in ToolsUI.scrollPanel.Go.transform.GetChild(3))
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in ToolsUI.scrollPanel.Go.transform.GetChild(5))
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < unit.originalUnitNumeratorList.Count; i++)
        {
            AddUnit(unit.originalUnitNumeratorList[i].Prefab, 3, i);
        }
        if (unit.originalUnitNumeratorList.Count == 0)
        {
            GameObject text = GameObject.Instantiate(new GameObject("text"));
            TMPro.TextMeshProUGUI textObj = text.AddComponent<TMPro.TextMeshProUGUI>();
            text.transform.SetParent(ToolsUI.scrollPanel.Go.transform.GetChild(3));
            text.transform.localScale = Vector3.one;
            text.transform.localPosition = Vector3.zero;
            textObj.text = "1";
            textObj.alignment = TMPro.TextAlignmentOptions.Center;
            textObj.fontSize = 150;
        }
        for (int i = 0; i < unit.originalUnitDenominatorList.Count; i++)
        {
            AddUnit(unit.originalUnitDenominatorList[i].Prefab, 5, i);
        }
        if (unit.originalUnitDenominatorList.Count == 0)
        {
            ToolsUI.scrollPanel.Go.transform.GetChild(1).localPosition = new Vector3(-174.99f, -0.17f, 0);
            ToolsUI.scrollPanel.Go.transform.GetChild(4).gameObject.SetActive(false);
            ToolsUI.scrollPanel.Go.transform.GetChild(3).localPosition = new Vector3(0f, -16f);
        }
        ToolsUI.scrollPanel.StartTransition("show", true);
    }

    private static void AddUnit(GameObject prefab, int childIndex, int i)
    {
        if (i != 0)
        {
            GameObject text = GameObject.Instantiate(new GameObject("text"));
            TMPro.TextMeshProUGUI textObj = text.AddComponent<TMPro.TextMeshProUGUI>();
            text.transform.SetParent(ToolsUI.scrollPanel.Go.transform.GetChild(childIndex));
            text.transform.localScale = Vector3.one;
            text.transform.localPosition = new Vector3(i * 150 - 75, 0, 0); ;
            textObj.text = "*";
            textObj.alignment = TMPro.TextAlignmentOptions.Center;
            textObj.fontSize = 150;
        }
        GameObject displayUnit = GameObject.Instantiate(prefab);
        displayUnit.transform.SetParent(ToolsUI.scrollPanel.Go.transform.GetChild(childIndex));
        displayUnit.transform.GetChild(0).gameObject.AddComponent<Image>().sprite = displayUnit.GetComponentInChildren<SpriteRenderer>().sprite;
        displayUnit.transform.GetChild(0).gameObject.GetComponent<Image>().preserveAspect = true;
        Component.Destroy(displayUnit.GetComponentInChildren<SpriteRenderer>());
        var rect = displayUnit.AddComponent<RectTransform>();
        displayUnit.transform.localPosition = new Vector3(i * 150, 0, 0);
        displayUnit.transform.localScale = new Vector3(150, 150, 150);
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
