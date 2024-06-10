using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEditor.Progress;
using Unity.VisualScripting;

/// <summary>
/// Functionality of a key
/// </summary>
public class RegisteredKey
{
    /// <summary>
    /// When the key is active
    /// </summary>
    public enum PauseType { onPause, onResume, both };
    //whether the key is currently pressed or not
    public bool Pressed { get; set; } = false;
    //whether the functionality of this key is active of not
    public bool Active { get; set; } = true;
    /// <summary>
    /// whether this key is active while the game is running or is active when paused
    /// </summary>
    public PauseType PauseTypeKey { get; set; }
    //actions assigned to each event
    public Action KeyDown { get; set; }
    public Action KeyUp { get; set; }


    /// <summary>
    /// Bind up new key to action
    /// </summary>
    public RegisteredKey(ToolsGame.PlayerActionsEnum playerAction, PauseType pausedKey = PauseType.onResume)
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
        PauseTypeKey = pausedKey;
    }

    /// <summary>
    /// Bind up new key to action
    /// </summary>
    public RegisteredKey(string actionPress, string actionRelease, ActionHandler item, PauseType pausedKey = PauseType.onResume)
    {
        this.KeyDown = () =>
        {
            if (Pressed == false)
            {
                Pressed = true;
                if ((GCon.Paused && pausedKey != PauseType.onResume)|| (!GCon.Paused && pausedKey != PauseType.onPause))
                {
                    LambdaActions.ExecuteAction(actionPress, item);
                }
            }
        };
        this.KeyUp = () =>
        {
            if ((GCon.Paused && pausedKey != PauseType.onResume) || (!GCon.Paused && pausedKey != PauseType.onPause))
            {
                LambdaActions.ExecuteAction(actionRelease, item);
            }
            Pressed = false;
        };
        PauseTypeKey = pausedKey;
    }

}

