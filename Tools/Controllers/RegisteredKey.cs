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
    //whether the key is currently pressed or not
    public bool Pressed { get; set; } = false;
    //whether the functionality of this key is active of not
    public bool Active { get; set; } = true;
    //actions assigned to each event
    public Action KeyDown { get; set; }
    public Action KeyUp { get; set; }


    /// <summary>
    /// Bind up new key to action
    /// </summary>
    public RegisteredKey(ToolsGame.PlayerActionsEnum playerAction)
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
    }

    /// <summary>
    /// Bind up new key to action
    /// </summary>
    public RegisteredKey(string actionPress, string actionRelease, ActionHandler item)
    {
        this.KeyDown = () =>
        {
            if (Pressed == false)
            {
                Pressed = true;
                LambdaActions.ExecuteAction(actionPress, item);
            }
        };
        this.KeyUp = () =>
        {
            Pressed = false;
            LambdaActions.ExecuteAction(actionRelease, item);
        };
    }

}

