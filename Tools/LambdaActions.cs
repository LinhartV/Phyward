using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

/// <summary>
/// Class for every Action delegate used in the game (for the case of JSON)
/// </summary>
public static class LambdaActions
{
    delegate void LambdaAction(ActionHandler item, params object[] parameters);
    /// <summary>
    /// Dictionary of actions. Key - name, Value - Action(gvars, id of item)
    /// </summary>
    private static Dictionary<string, LambdaAction> lambdaActions = new();


    public static void SetupLambdaActions()
    {
        lambdaActions.Add("up", (item, parameter) =>
        {
            (item as Movable).UpdateControlledMovement("up");
        });
        lambdaActions.Add("down", (item, parameter) =>
        {
            (item as Movable).UpdateControlledMovement("down");
        });
        lambdaActions.Add("left", (item, parameter) =>
        {
            (item as Movable).UpdateControlledMovement("left");
        });
        lambdaActions.Add("right", (item, parameter) =>
        {
            (item as Movable).UpdateControlledMovement("right");
        });
        //Rotate to face pointer
        lambdaActions.Add("faceCursor", (item, parameter) =>
        {
            Player p = item as Player;
            var mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            p.Prefab.GetComponentInChildren<SpriteRenderer>().transform.up = new Vector2(mousePosition.x - p.Prefab.transform.position.x, mousePosition.y - p.Prefab.transform.position.y);
        });
        lambdaActions.Add("move", (item, parameters) =>
        {
            (item as Movable).Move();
        });
        /*lambdaActions.Add("fire1", (item, parameters) =>
        {
            Character character = gvars.Items[id] as Character;
            if (!character.WeaponNode.Weapon.Reloaded)
            {
                character.DeleteAction("fire");
                character.WeaponNode.Weapon.Reloaded = true;
            }
            else
            {
                character.WeaponNode.Weapon.Fire(gvars);
            }

        }
        );*/

    }
    public static void ExecuteAction(string actionName, ActionHandler item, params object[] parameters)
    {
        if (lambdaActions.ContainsKey(actionName))
        {
            lambdaActions[actionName].Invoke(item, parameters);
        }
    }
}

