using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Actions that can be assigned to Items - controlled by item itself.
/// </summary>
[Serializable]
public class ItemAction
{
    ItemAction() { }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExecutionType
    {
        ///<summary>Action executes immidiatelly and repeats infinitely</summary>
        EveryTime,
        ///<summary>Action repeats infinitely, but doesn't execute first Time</summary>
        NotFirstTime,
        ///<summary>Action executes only once after duration Time runs out (set duration to 0 to happen at the start of the very next frame)</summary>
        OnlyFirstTime,
        ///<summary>Stops execution of selected action</summary>
        StopExecuting
    }
    /// <summary>
    /// what should happen to actions of items, when player leaves level they were in
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]

    public enum OnLeaveType
    {
        ///<summary>Deletes the running action.</summary>
        Delete,
        ///<summary>Freezes the action where it was.</summary>
        Freeze,
        ///<summary>Keeps running the action</summary>
        KeepRunning
    }
    //Action to be executed
    public string ActionName { get; private set; }
    //Parameters
    public object[] Parameters { get; private set; }
    //whether the action will be repeated and if so, how long it take between each repetition (number of frames)
    [JsonProperty]
    private double repeat;
    public ExecutionType executionType;
    public OnLeaveType onLeaveType;
    public List<ToolsSystem.PauseType> pauseTypes;
    [JsonIgnore]
    public LambdaActions.LambdaAction lambdaAction = null;
    public long NowDifference { get; set; } = 0;
    /// <summary>
    /// Creates ItemAction - any possible action can be assigned to an item
    /// </summary>
    /// <param name="actionName">Name of an action from LambdaActions</param>
    /// <param name="repeat">How many frames to wait between executions (0 = only first Time, 1 = each frame)</param>
    /// <param name="clientAction">Whether this action should be executed on client side</param>
    /// <param name="pauseTypes">"When to stop the action"</param>
    /// <param name="executionType"></param>
    public ItemAction(string actionName, double repeat, ExecutionType executionType = ExecutionType.EveryTime, OnLeaveType onLeaveType = OnLeaveType.Freeze, List<ToolsSystem.PauseType> pauseTypes = null, params object[] parameters)
    {
        if (pauseTypes != null)
            this.pauseTypes = pauseTypes;
        else
            this.pauseTypes = new List<ToolsSystem.PauseType>();

        ActionName = actionName;
        Repeat = repeat;
        this.executionType = executionType;
        this.Parameters = parameters;
        this.onLeaveType = onLeaveType;
    }
    /// <summary>
    /// Creates ItemAction - use this constructor for UI only as this action won't be saved to disk after closing
    /// </summary>
    /// <param name="lambdaAction">Direct lambda action (ActionHandler item, params object[] parameters)</param>
    /// <param name="repeat">How many frames to wait between executions (0 = only first Time, 1 = each frame)</param>
    /// <param name="clientAction">Whether this action should be executed on client side</param>
    /// <param name="pauseTypes">"When to stop the action"</param>
    /// <param name="executionType"></param>
    public ItemAction(LambdaActions.LambdaAction lambdaAction, double repeat, ExecutionType executionType = ExecutionType.EveryTime, OnLeaveType onLeaveType = OnLeaveType.Freeze, List<ToolsSystem.PauseType> pauseTypes = null, params object[] parameters)
    {
        if (pauseTypes != null)
            this.pauseTypes = pauseTypes;
        else
            this.pauseTypes = new List<ToolsSystem.PauseType>();
        this.lambdaAction = lambdaAction;
        ActionName = "";
        Repeat = repeat;
        this.executionType = executionType;
        this.Parameters = parameters;
        this.onLeaveType = onLeaveType;
    }

    public double Repeat
    {
        get { return repeat; }
        set
        {
            if (value > 0)
            {
                if (value <= 1)
                {
                    repeat = 1;
                }
                else
                    repeat = value;
            }
            else
            {
                repeat = 0;
            }
        }
    }

}