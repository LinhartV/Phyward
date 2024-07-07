using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

/// <summary>
/// Functionality of a key
/// </summary>
public class RegisteredKey
{
    //whether the key is currently pressed or not
    public bool Pressed { get; set; } = false;
    //whether the functionality of this key is active of not
    public bool Active { get; set; } = true;
    /// <summary>
    /// whether this key is active while the game is running or is active when paused
    /// </summary>
    public List<ToolsSystem.PauseType> pauseTypeKeys = new List<ToolsSystem.PauseType>();
    //actions assigned to each event
    public Action KeyDown { get; set; }
    public Action KeyUp { get; set; }


    /// <summary>
    /// Bind up new key to action
    /// </summary>
    public RegisteredKey(ToolsGame.PlayerActionsEnum playerAction, ToolsSystem.PauseType pauseType = ToolsSystem.PauseType.InGame, params ToolsSystem.PauseType[] pauseTypes)
    {
        this.KeyDown = () =>
        {
            if (Pressed == false)
            {
                Pressed = true;

                if (ToolsGame.actions.ContainsKey(playerAction))
                {
                    ToolsGame.actions[playerAction].Item1();
                }
            }
        };
        this.KeyUp = () =>
        {
            Pressed = false;

            if (ToolsGame.actions.ContainsKey(playerAction))
            {
                ToolsGame.actions[playerAction].Item2();
            }

        };
        pauseTypeKeys.Add(pauseType);
        pauseTypeKeys.AddRange(pauseTypes);
    }

    /// <summary>
    /// Bind up new key to action
    /// </summary>
    public RegisteredKey(string actionPress, string actionRelease, ActionHandler item, ToolsSystem.PauseType pauseType = ToolsSystem.PauseType.InGame, params ToolsSystem.PauseType[] pauseTypes)
    {
        this.KeyDown = () =>
        {
            if (Pressed == false)
            {
                Pressed = true;
                if (pauseTypeKeys.Contains(GCon.GetPausedType()))
                {
                    LambdaActions.ExecuteAction(actionPress, item);
                }
            }
        };
        this.KeyUp = () =>
        {
            if (pauseTypeKeys.Contains(GCon.GetPausedType()))
            {
                LambdaActions.ExecuteAction(actionRelease, item);
            }
            Pressed = false;
        };
        pauseTypeKeys.Add(pauseType);
        pauseTypeKeys.AddRange(pauseTypes);
    }

}

