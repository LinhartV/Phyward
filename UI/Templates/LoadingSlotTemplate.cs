using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LoadingSlotTemplate : SlotTemplate
{
    public float Time { get; set; }
    public bool SlotReady { get; private set; }
    private float currentTime;
    private RectTransform rect;
    public LoadingSlotTemplate(GameObject slot, bool hoverable, bool dragable, bool deleteSlotableInDraggedSlotWhenDropped, Func<SlotTemplate, bool> dropableCondition = null, Action<SlotTemplate> onDrop = null, Action<SlotTemplate> onRemovedSlotable = null, bool addByReference = false, bool exchangable = false, ToolsSystem.PauseType pauseType = ToolsSystem.PauseType.Inventory, params ToolsSystem.PauseType[] pauseTypes) : base(slot, hoverable, dragable, deleteSlotableInDraggedSlotWhenDropped, dropableCondition, onDrop, onRemovedSlotable, addByReference, exchangable, pauseType, pauseTypes)
    {
        Constructor();
    }

    public LoadingSlotTemplate(float slotWidth, bool hoverable, bool dragable, bool deleteSlotableInDraggedSlotWhenDropped, GameObject parent, Vector3 pos, Func<SlotTemplate, bool> dropableCondition = null, Action<SlotTemplate> onDrop = null, Action<SlotTemplate> onRemovedSlotable = null, bool addByReference = false, bool exchangable = false, ToolsSystem.PauseType pauseType = ToolsSystem.PauseType.Inventory, params ToolsSystem.PauseType[] pauseTypes) : base(slotWidth, hoverable, dragable, deleteSlotableInDraggedSlotWhenDropped, parent, pos, dropableCondition, onDrop, onRemovedSlotable, addByReference, exchangable, pauseType, pauseTypes)
    {
        Constructor();
    }

    private void Constructor()
    {
        SlotReady = true;
        var gradUI = GameObject.Instantiate(new GameObject("Gradient"));
        var grad = gradUI.AddComponent<Gilzoide.GradientRect.GradientRect>();
        gradUI.transform.SetParent(Go.transform);
        grad.Gradient = new Gradient();
        grad.Gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(new Color(0, 0, 0, 1), 0), new GradientColorKey(new Color(0.2358491f, 0.08121218f, 0.1194027f), 1) }, new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });

        rect = gradUI.GetComponent<RectTransform>();
        rect.transform.localPosition = new Vector3(Go.transform.GetChild(0).localScale.x / 20, 0);
        rect.pivot = new Vector2(0, 0.5f);
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(0, 0.5f);
        rect.transform.localScale = new Vector3(0.9f * Go.transform.GetChild(0).localScale.x, 0.9f * Go.transform.GetChild(0).localScale.y);
        rect.sizeDelta = new Vector2(0, 1);
    }

    public void StartCountDown()
    {
        currentTime = 0;
        AddAction(new ItemAction("slotCountdown", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning, new List<ToolsSystem.PauseType>() { ToolsSystem.PauseType.Inventory }, this), 0);
        SlotReady = false;
    }


    public void SetLoadingValue(float value, bool relative = true)
    {
        if (relative)
        {
            currentTime += value;
        }
        else
        {
            currentTime = value;
        }
        if (currentTime >= Time)
        {
            currentTime = Time;
            SlotReady = true;
            DeleteAction("slotCountdown");
        }
        float width = (Time - currentTime) / Time;
        rect.sizeDelta = new Vector2(width, 1);

    }
}
