
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

/// <summary>
/// Object that actions can be assigned to
/// </summary>
public class ActionHandler
{
    /// <summary>
    /// Enum for determining what to do with action with the same name
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RewriteEnum { Ignore = 0, Rewrite = 1, AddNew = 2 }
    private int storeIndex = 0;
    //actions of this item with information when to be execuded, accessed by name.
    [JsonProperty]
    protected Dictionary<string, (long, ItemAction)> actions = new();
    //actions of this item to be executed everyFrame
    [JsonProperty]
    protected Dictionary<string, ItemAction> actionsEveryFrame = new();
    [JsonProperty]
    protected Dictionary<string, (long, ItemAction)> actionsFrozen = new();
    //actions of this item to be executed everyFrame
    [JsonProperty]
    protected Dictionary<string, ItemAction> actionsEveryFrameFrozen = new();
    [JsonProperty]
    public int Id { get; set; }
    public bool IsInLevel { get; set; }

    public ActionHandler()
    {
    }

    public ActionHandler(bool dontBeCalledByJson)
    {
        this.Id = GCon.game.IdItems++;
    }


    /// <summary>
    /// Save non-serializeble variables here
    /// </summary>
    public virtual void SaveItem()
    {

    }
    /// <summary>
    /// Actions to be executed in the current frame. If action is supposed to repeat, it will be added again to the list.
    /// Due to possible differences in duration of particular frames, actions will be executed by number of frames, not real time
    /// </summary>
    public void ExecuteActions(long now)
    {
        if (this is Movable m && m.IsInLevel)
        {
            m.CorrectSpeed();
        }
        Dictionary<string, ItemAction> tempActionsEveryFrame = new Dictionary<string, ItemAction>(actionsEveryFrame);
        foreach (var action in tempActionsEveryFrame.Values)
        {
            LambdaActions.ExecuteAction(action.ActionName, this, action.Parameters);
        }
        if (this is Movable mm && mm.IsInLevel)
        {
            mm.Move();
        }

        Dictionary<string, (long, ItemAction)> tempActions = new Dictionary<string, (long, ItemAction)>(actions);
        foreach (var actionName in tempActions.Keys)
        {
            if (tempActions[actionName].Item1 + tempActions[actionName].Item2.NowDifference < now)
            {
                if (actions[actionName].Item2.executionType == ItemAction.ExecutionType.EveryTime || (actions[actionName].Item2.Repeat == 0 && actions[actionName].Item2.executionType == ItemAction.ExecutionType.OnlyFirstTime))
                {
                    LambdaActions.ExecuteAction(actions[actionName].Item2.ActionName, this, actions[actionName].Item2.Parameters);
                }
                else if (actions[actionName].Item2.executionType == ItemAction.ExecutionType.NotFirstTime)
                {
                    actions[actionName].Item2.executionType = ItemAction.ExecutionType.EveryTime;
                }
                actions.Remove(actionName);
                if (tempActions[actionName].Item2.Repeat > 0 && tempActions[actionName].Item2.executionType != ItemAction.ExecutionType.StopExecuting)
                {
                    actions.Add(actionName, (now + (long)(tempActions[actionName].Item2.Repeat), tempActions[actionName].Item2));
                    if (tempActions[actionName].Item2.executionType == ItemAction.ExecutionType.OnlyFirstTime)
                    {
                        tempActions[actionName].Item2.Repeat = 0;
                    }
                }

            }
        }
    }
    public bool ChangeRepeatTime(double repeat, string actionName)
    {
        if (actions.ContainsKey(actionName))
        {
            actions[actionName].Item2.Repeat = repeat;

            if (repeat <= 1 && repeat > 0)
            {
                actionsEveryFrame.Add(actionName, actions[actionName].Item2);
                actions.Remove(actionName);
            }
            return true;
        }
        else if (actionsEveryFrame.ContainsKey(actionName))
        {
            if (repeat > 1)
            {
                actions.Add(actionName, (0, actionsEveryFrame[actionName]));
                actionsEveryFrame.Remove(actionName);
            }
            return true;
        }
        else { return false; }
    }
    public void ChangeNowDifference(long nowDifference)
    {
        foreach (var action in actions.Values)
        {
            action.Item2.NowDifference += nowDifference;
        }
    }
    /// <summary>
    /// Adds a new action to be executed
    /// </summary>
    /// <param name="action">ItemAction to add</param>
    /// <param name="rewrite">Whether to rewrite running action</param>
    /// <param name="delay">"When to execute the action. Gotta be (now + delay) "</param>
    public void AddAction(ItemAction action, long delay = 0, RewriteEnum rewrite = RewriteEnum.Rewrite)
    {
        this.AddAction(action, action.ActionName, delay, rewrite);
    }
    /// <summary>
    /// Adds a new action to be executed
    /// </summary>
    /// <param name="action">ItemAction to add</param>
    /// <param name="storeName">The name the action will be stored in the dictionary under</param>
    /// <param name="rewrite">Whether to rewrite running action</param>
    public void AddAction(ItemAction action, string storeName, long delay = 0, RewriteEnum rewrite = RewriteEnum.Rewrite)
    {
        Dictionary<string, (long, ItemAction)> temp;
        Dictionary<string, ItemAction> tempEveryFrame;
        if (IsInLevel || action.onLeaveType == ItemAction.OnLeaveType.KeepRunning || action.onLeaveType == ItemAction.OnLeaveType.Delete)
        {
            temp = actions;
            tempEveryFrame = actionsEveryFrame;
            if (!GCon.game.ItemsStep.ContainsKey(this.Id))
            {
                GCon.game.ItemsStep.Add(this.Id, this);
            }
        }
        else
        {
            temp = actionsFrozen;
            tempEveryFrame = actionsEveryFrameFrozen;
        }
        if (action.Repeat > 1 || action.Repeat == 0)
        {
            if (!temp.ContainsKey(storeName))
                temp.Add(storeName, (delay, action));
            else if (rewrite == RewriteEnum.Rewrite)
            {
                temp.Remove(storeName);
                temp.Add(storeName, (delay, action));
            }
            else if (rewrite == RewriteEnum.AddNew)
            {
                temp.Add(storeName + storeIndex.ToString(), (delay, action));
                storeIndex++;
            }
        }
        else
        {
            if (!tempEveryFrame.ContainsKey(storeName))
                tempEveryFrame.Add(storeName, action);
            else if (rewrite == RewriteEnum.Rewrite)
            {
                tempEveryFrame.Remove(storeName);
                tempEveryFrame.Add(storeName, action);
            }
        }

    }

    public void DeleteAction(string name)
    {
        if (actions.ContainsKey(name))
        {
            actions[name].Item2.executionType = ItemAction.ExecutionType.StopExecuting;
            actions.Remove(name);
        }
        if (actionsEveryFrame.ContainsKey(name))
            actionsEveryFrame.Remove(name);
        if (actions.Count == 0 && actionsEveryFrame.Count == 0)
        {
            GCon.game.ItemsStep.Remove(this.Id);
        }
    }

    /// <summary>
    /// what should happen during disposal
    /// </summary>
    public virtual void InnerDispose()
    {
        if (GCon.game.ItemsStep.ContainsKey(Id))
        {
            GCon.game.ItemsStep.Remove(Id);
        }
    }

    /// <summary>
    /// Tells program to dispose this object at the end of the frame
    /// </summary>
    public void Dispose()
    {
        GCon.game.ItemsToBeDestroyed.Add(this);
    }

    public virtual void OnLevelLeave()
    {
        bool keepRunning = false;


        Dictionary<string, (long, ItemAction)> temp = new(actions);
        foreach (var action in temp)
        {
            if (action.Value.Item2.onLeaveType == ItemAction.OnLeaveType.Delete)
            {
                actions.Remove(action.Key);
            }
            if (action.Value.Item2.onLeaveType == ItemAction.OnLeaveType.Freeze)
            {
                actionsFrozen.Add(action.Key, action.Value);
                actions.Remove(action.Key);
            }
            if (action.Value.Item2.onLeaveType == ItemAction.OnLeaveType.KeepRunning)
            {
                keepRunning = true;
            }
        }
        Dictionary<string, ItemAction> tempEvery = new(actionsEveryFrame);
        foreach (var action in tempEvery)
        {
            if (action.Value.onLeaveType == ItemAction.OnLeaveType.Delete)
            {
                actionsEveryFrame.Remove(action.Key);
            }
            if (action.Value.onLeaveType == ItemAction.OnLeaveType.Freeze)
            {
                actionsEveryFrameFrozen.Add(action.Key, action.Value);
                actionsEveryFrame.Remove(action.Key);
            }
            if (action.Value.onLeaveType == ItemAction.OnLeaveType.KeepRunning)
            {
                keepRunning = true;
            }
        }
        if (!keepRunning)
        {
            if (GCon.game.ItemsStep.ContainsKey(Id))
            {
                GCon.game.ItemsStep.Remove(Id);
            }
        }
    }
    public virtual void OnLevelEnter()
    {
        bool getRunning = false;
        foreach (var action in actionsFrozen)
        {
            actions.Add(action.Key, action.Value);
            getRunning = true;
        }
        foreach (var action in actionsEveryFrameFrozen)
        {
            actionsEveryFrame.Add(action.Key, action.Value);
            getRunning = true;
        }
        actionsEveryFrameFrozen.Clear();
        actionsFrozen.Clear();
        if (getRunning)
        {
            if (!GCon.game.ItemsStep.ContainsKey(Id))
            {
                GCon.game.ItemsStep.Add(Id, this);
            }
        }
    }
}
