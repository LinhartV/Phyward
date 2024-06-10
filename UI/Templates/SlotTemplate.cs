using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class SlotTemplate : UIItem
{
    /// <summary>
    /// Whether there's something player can drag out (if it's draggable in this very moment)
    /// </summary>
    public bool Dragable { get; set; } = false;
    /// <summary>
    /// Whether player can drop something here
    /// </summary>
    public bool Dropable { get; private set; } = false;
    /// <summary>
    /// If this slot can be draggable even with something in it
    /// </summary>
    private bool generallyDraggable;
    private bool isBeingDragged = false;
    private bool hasSlotableDisplayed;
    private bool deleteSlotableInDraggedSlotWhenDropped;
    private UIItem draggedObjectUI;
    public Slotable SlotableRef { get; private set; }
    public Func<SlotTemplate, bool> dropableCondition = null;
    protected Action<SlotTemplate> onDrop;
    protected Action<SlotTemplate> onRemovedSlotable;
    private bool hasTextAttached = false;
    /// <summary>
    /// text for count
    /// </summary>
    private GameObject text;

    public int Count { get; private set; }
    /// <summary>
    /// When dragging, whether to drag all or just one from stack
    /// </summary>
    public bool DragAll { get; set; } = true;
    public bool ShowStackTextForCountLessThenTwo { get; set; } = false;
    /// <summary>
    /// Whether this slot is stackable
    /// </summary>
    public bool Stackable { get; set; } = true;
    

    public SlotTemplate()
    {
    }

    /// <param name="hoverable">Whether to start onHover animation</param>
    /// <param name="slotWidth"></param>
    /// <param name="hoverable"></param>
    /// <param name="dragable">If this slot can be draggable even with something in it</param>
    /// <param name="parent">Who should become parent of this slot</param>
    /// <param name="pos">Position</param>
    /// <param name="dropableCondition">Under which condition this slot can be considered dropable. Reference is set to this slot. Refer ToolsUI.draggedSlot for dragged slot. Leave blank for always false</param>
    /// <param name="onRemovedSlotable">What should happen when slotable is removed from this slot. Reference to this slot.</param>
    /// <param name="onDrop">What should happen when something is dropped here - reference is set to slot where it's being dropped</param>
    public SlotTemplate(float slotWidth, bool hoverable, bool dragable, bool deleteSlotableInDraggedSlotWhenDropped, GameObject parent, Vector3 pos, Func<SlotTemplate, bool> dropableCondition = null, Action<SlotTemplate> onDrop = null, Action<SlotTemplate> onRemovedSlotable = null) : base()
    {
        var slot = UnityEngine.Object.Instantiate(GameObjects.slot);
        slot.transform.SetParent(parent.transform, false);
        slot.transform.localPosition = pos;
        var rect = slot.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        slot.transform.GetChild(0).gameObject.GetComponent<RectTransform>().transform.localScale = new Vector3(slotWidth, slotWidth, 1);
        var box = slot.GetComponent<BoxCollider2D>();
        box.size = new Vector2(slotWidth, slotWidth);
        slot.transform.SetAsFirstSibling();
        Constructor(slot, hoverable, dropableCondition, dragable, onDrop, deleteSlotableInDraggedSlotWhenDropped, onRemovedSlotable);
    }

    /// <param name="slot">Reference to a slot placed in the screen</param>
    /// <param name="hoverable">Whether to start onHover animation</param>
    /// <param name="dragable">If this slot can be draggable even with something in it</param>
    /// <param name="dropableCondition">Under which condition this slot can be considered dropable. Reference is set to this slot. Refer ToolsUI.draggedSlot for dragged slot. Leave blank for always false</param>
    /// <param name="onRemovedSlotable">What should happen when slotable is removed from this slot. Reference to this slot.</param>
    /// <param name="onDrop">What should happen when something is dropped here - reference is set to slot where it's being dropped. Reference to this slot.</param>
    public SlotTemplate(GameObject slot, bool hoverable, bool dragable, bool deleteSlotableInDraggedSlotWhenDropped, Func<SlotTemplate, bool> dropableCondition = null, Action<SlotTemplate> onDrop = null, Action<SlotTemplate> onRemovedSlotable = null)
    {
        Constructor(slot, hoverable, dropableCondition, dragable, onDrop, deleteSlotableInDraggedSlotWhenDropped, onRemovedSlotable);
    }
    protected virtual void Constructor(GameObject slot, bool hoverable, Func<SlotTemplate, bool> dropableCondition, bool dragable, Action<SlotTemplate> onDrop, bool deleteSlotableInDraggedSlotWhenDropped, Action<SlotTemplate> onRemovedSlotable)
    {
        this.onRemovedSlotable = onRemovedSlotable;
        this.deleteSlotableInDraggedSlotWhenDropped = deleteSlotableInDraggedSlotWhenDropped;
        var rect = slot.GetComponent<RectTransform>();
        SetupUIItem(slot, slot.transform.GetChild(0).gameObject);
        if (hoverable)
        {
            AddTransition(new ColorChangable(0.1f, new Color(0, 0, 0, -50), null, true), "hover");
        }
        ToolsUI.allSlots.Add(this);

        if (dropableCondition != null)
        {
            this.dropableCondition = dropableCondition;
        }
        generallyDraggable = dragable;
        this.onDrop = onDrop;
    }
    public void CheckForDropability()
    {
        if (dropableCondition != null && this.dropableCondition(this))
        {
            Dropable = true;
            this.AddAnimation(new ColorChangable(0.5f, new Color(-100, -255, 0), ToolsUI.easeIn, true), null, "Dropable");
        }
        else
        {
            Dropable = false;
        }
    }
    public void ResetDropable()
    {
        Dropable = false;
        this.EndAnimation("Dropable", true);
    }
    public override void InnerDispose()
    {
        ToolsUI.allSlots.Remove(this);
        base.InnerDispose();
    }
    public override void OnMouseEnterDefault()
    {
        base.OnMouseEnterDefault();
        if (ToolsUI.DraggedSlot == null)
        {
            StartTransition("hover");
        }
        if (ToolsUI.DraggedSlot != null && this.Dropable == true)
        {
            StartTransition("hover");
            ToolsUI.hoveredWhileDragging = this;
        }
        if (SlotableRef != null && ToolsUI.DraggedSlot == null && GCon.Paused == true)
        {
            this.AddAction(new ItemAction("ShowDescription", 60, ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning));
        }
        //ToolsUI.SetCursor(ToolsUI.selectCursor);
    }
    public override void OnMouseExitDefault()
    {
        this.DeleteAction("ShowDescription");
        if (SlotableRef != null)
        {
            ToolsUI.descriptionPanel.Go.SetActive(false);
        }
        base.OnMouseExitDefault();
        ReturnTransition("hover");

        if (ToolsUI.DraggedSlot != null)
        {
            if (ToolsUI.hoveredWhileDragging == this)
            {
                ToolsUI.hoveredWhileDragging = null;
            }
        }
        //ToolsUI.SetCursor(ToolsUI.normalCursor);
    }
    public override void OnMouseDownDefault()
    {
        if (Dragable && ToolsUI.DraggedSlot == null && Count > 0)
        {
            StartDragging();
        }
        base.OnMouseDownDefault();
    }
    public override void OnMouseUpDefault()
    {
        base.OnMouseDownDefault();
    }
    public override void OnLevelLeave()
    {
        base.OnLevelLeave();
        IsInLevel = false;
    }
    public void StartDragging()
    {
        this.Go.transform.SetAsLastSibling();
        ToolsUI.SetCursor(ToolsUI.holdCursor);
        var draggedObject = GameObject.Instantiate(Go.transform.GetChild(1).gameObject);
        draggedObjectUI = new UIItem(draggedObject);
        draggedObjectUI.Go.transform.SetParent(this.Go.transform);
        draggedObjectUI.Go.transform.localScale = Go.transform.GetChild(1).gameObject.transform.localScale;
        draggedObjectUI.Go.transform.GetChild(0).localScale = Vector3.one;
        draggedObjectUI.Go.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        draggedObjectUI.IsInLevel = true;
        draggedObjectUI.AddAction(new ItemAction("followCursor", 1));
        isBeingDragged = true;
        ToolsUI.DraggedSlot = this;

        ToolsUI.CheckForDropability();
        if (DragAll)
        {
            DisposeSlotable();
        }
        else
        {
            this.Count--;
            if (Count == 0)
            {
                if (!ShowStackTextForCountLessThenTwo)
                {
                    DisposeSlotable();
                }
                this.Dragable = false;
            }
            TMPro.TextMeshProUGUI component;
            if (draggedObjectUI.Go.transform.childCount > 1 && draggedObjectUI.Go.transform.GetChild(1).TryGetComponent<TMPro.TextMeshProUGUI>(out component))
            {
                GameObject.Destroy(draggedObjectUI.Go.transform.GetChild(1).gameObject);
            }
            UpdateCount();
        }
    }
    public virtual void StopDragging()
    {
        ToolsUI.SetCursor(ToolsUI.normalCursor);
        if (ToolsUI.hoveredWhileDragging != null)
        {
            this.Dragable = true;
            ToolsUI.hoveredWhileDragging.AddSlotable(this.SlotableRef, true);
            if (ToolsUI.hoveredWhileDragging.onDrop != null)
            {
                ToolsUI.hoveredWhileDragging.onDrop(ToolsUI.hoveredWhileDragging);
            }
            OnSuccessfulDrag();
            ToolsUI.DraggedSlot = null;
        }
        else
        {
            ToolsUI.draggedSlotBeingReturnedFromUnsuccessfulDrag = true;
            ToolsUI.AnimateSettingSlotable(0.3f, ToolsUI.wrapPanel.Go.transform.InverseTransformPoint(this.Go.transform.position), draggedObjectUI.Go, () =>
            {
                this.Dragable = true;
                if (!DragAll)
                {
                    Count++;
                }
                if (!hasSlotableDisplayed)
                {
                    InstantiateSlotable(SlotableRef);
                }
                UpdateCount();
                ToolsUI.DraggedSlot = null;
            }, true);
        }
        if (isBeingDragged)
        {
            draggedObjectUI.DeleteAction("followCursor");
            draggedObjectUI.Dispose();
        }
        isBeingDragged = false;
    }

    protected virtual void OnSuccessfulDrag()
    {
        if (ToolsUI.hoveredWhileDragging.deleteSlotableInDraggedSlotWhenDropped)
        {
            if (!DragAll)
            {
                Count++;
            }
            RemoveSlotable(DragAll);
        }
        else
        {
            AddSlotable(this.SlotableRef);
        }
    }

    public virtual void RemoveSlotable(bool removeAll = true)
    {
        if (removeAll)
        {
            if (!ShowStackTextForCountLessThenTwo)
            {
                if (onRemovedSlotable != null)
                {
                    onRemovedSlotable(this);
                }
                this.SlotableRef = null;
                DisposeSlotable();
            }
            this.Dragable = false;
        }
        else
        {
            this.Count--;
            if (Count == 0)
            {
                if (!ShowStackTextForCountLessThenTwo)
                {
                    if (onRemovedSlotable != null)
                    {
                        onRemovedSlotable(this);
                    }
                    this.SlotableRef = null;
                    DisposeSlotable();

                }
                this.Dragable = false;
            }
        }
        UpdateCount();
    }

    public void AddSlotable(Slotable slotable, bool stack = false, int count = -1, bool setCountRelatively = true)
    {
        Dragable = generallyDraggable;
        int slotableCount = 0;
        if (ToolsUI.draggedSlot == null || ToolsUI.draggedSlot.DragAll)
        {
            slotableCount = slotable.Count;
        }
        else
        {
            slotableCount = 1;
        }
        if (SlotableRef == null)
        {
            InstantiateSlotable(slotable, count == -1 ? slotableCount : count);
        }
        else if (stack && SlotableRef.Name == slotable.Name && SlotableRef.Stackable && Stackable)
        {
            if (count == -1)
            {
                if (setCountRelatively)
                    Count += slotableCount;
                else
                    Count = slotableCount;
            }
            else
            {
                if (setCountRelatively)
                    Count += count;
                else
                    Count = count;
            }
        }
        else //if (slotable != this.SlotableRef) dunno why I wrote this here
        {
            GameObject.Destroy(this.Go.transform.GetChild(1).gameObject);
            InstantiateSlotable(slotable);
        }
        UpdateCount();
    }
    private void UpdateCount()
    {
        if (Count > 1 || ShowStackTextForCountLessThenTwo)
        {
            if (!hasTextAttached)
            {
                text = UnityEngine.Object.Instantiate(GameObjects.counter);
                text.transform.SetParent(this.Go.transform.GetChild(1).transform, false);
                text.GetComponent<RectTransform>().pivot = new Vector2(0f, 1);
                text.transform.localScale = new Vector3(1 / this.Go.transform.GetChild(1).localScale.x, 1 / this.Go.transform.GetChild(1).localScale.y);
                text.transform.localPosition = new Vector3(-0.38f, 0);
                hasTextAttached = true;
            }
            text.GetComponent<TMPro.TextMeshProUGUI>().text = Count.ToString();
        }
        else
        {
            if (hasTextAttached)
            {
                UnityEngine.Object.Destroy(text);
                hasTextAttached = false;
            }
        }
    }
    protected void InstantiateSlotable(Slotable slotable, int count = -1)
    {
        hasSlotableDisplayed = true;
        var prefab = UnityEngine.Object.Instantiate(slotable.Prefab);
        prefab.layer = LayerMask.NameToLayer("UI");
        var image = prefab.transform.GetChild(0).AddComponent<Image>();
        image.raycastTarget = false;
        image.sprite = prefab.GetComponentInChildren<SpriteRenderer>().sprite;
        Component.Destroy(prefab.GetComponentInChildren<SpriteRenderer>());

        //prefab.transform.localScale = Vector3.one;
        SlotableRef = slotable;
        if (count == -1)
        {
            Count = slotable.Count;
        }
        else
        {
            Count = count;
            SlotableRef.Count = Count;
        }
        if (Stackable == false)
        {
            Count = 1;
            SlotableRef.Count = 1;
        }
        prefab.transform.SetParent(this.Go.transform, false);

        var rect = prefab.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        prefab.transform.localPosition = new Vector3(0, 0);
        var width = (Constants.SLOT_WIDTH - Constants.SLOT_OFFSET) * this.Go.transform.GetChild(0).localScale.x / Constants.SLOT_WIDTH;
        prefab.transform.localScale = new Vector3(width, width, 1);
        Component.Destroy(prefab.GetComponent<BoxCollider2D>());
        prefab.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
        prefab.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
        hasTextAttached = false;
        UpdateCount();
    }
    protected void DisposeSlotable()
    {
        if (this.Go.transform.GetChild(1).gameObject.activeInHierarchy)
        {
            GameObject.Destroy(this.Go.transform.GetChild(1).gameObject);
        }
        hasSlotableDisplayed = false;
        Dragable = false;
    }
}
