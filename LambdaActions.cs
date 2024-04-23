using Newtonsoft.Json;
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
    public delegate void LambdaAction(ActionHandler item, params object[] parameters);
    /// <summary>
    /// Dictionary of actions. Key - name, Value - Action(gvars, id of item)
    /// </summary>
    public static Dictionary<string, LambdaAction> lambdaActions = new();


    public static void SetupLambdaActions()
    {
        lambdaActions.Add("receiveDamage", (item, parameter) =>
        {
            (item as Character).ChangeLives(-(float)parameter[0]);
        });
        lambdaActions.Add("randomWalk", (item, parameter) =>
        {
            if ((item as Movable).MovementsControlled["randomMovement"].MovementSpeed == 0)
            {
                (item as Movable).MovementsControlled["randomMovement"].ResetMovementSpeed((item as Movable).BaseSpeed / 4);
                (item as Movable).RotateControlledMovement("randomMovement", (float)(ToolsGame.Rng() * Math.PI * 2));
            }
        });
        lambdaActions.Add("up", (item, parameter) =>
        {
            (item as Movable).UpdateCompositeMovement("movement", "up");
        });
        lambdaActions.Add("down", (item, parameter) =>
        {
            (item as Movable).UpdateCompositeMovement("movement", "down");
        });
        lambdaActions.Add("left", (item, parameter) =>
        {
            (item as Movable).UpdateCompositeMovement("movement", "left");
        });
        lambdaActions.Add("right", (item, parameter) =>
        {
            (item as Movable).UpdateCompositeMovement("movement", "right");
        });
        //Rotate to face pointer
        lambdaActions.Add("faceCursor", (item, parameter) =>
        {
            Player p = item as Player;
            var mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            p.Prefab.GetComponentInChildren<SpriteRenderer>().transform.up = new Vector2(mousePosition.x - p.Prefab.transform.position.x, mousePosition.y - p.Prefab.transform.position.y);
            p.Angle = ToolsMath.GetAngleFromLengts(mousePosition.x - p.Prefab.transform.position.x, mousePosition.y - p.Prefab.transform.position.y);
        });
        lambdaActions.Add("fire1", (item, parameters) =>
        {
            Character character = item as Character;
            if (!character.Weapon.Reloaded)
            {
                character.DeleteAction("fire");
                character.Weapon.Reloaded = true;
            }
            else
            {
                character.Weapon.Fire();
            }

        }
        );
        lambdaActions.Add("fire2", (item, parameter) => { (item as Character).Weapon.Reloaded = true; });
        lambdaActions.Add("dispose", (item, parameter) =>
        {
            item.Dispose();
        });
        lambdaActions.Add("death", (item, parameter) =>
        {
            (item as Character).Death();
        });
        lambdaActions.Add("faceInDirection", (item, parameter) =>
        {
            if (!((item is Movable m) && m.SetAngle && m.GetMovementSpeed() == 0))
            {
                var angle = ToolsMath.PolarToCartesian((item as Item).Angle, 1);
                (item as Item).Prefab.GetComponentInChildren<SpriteRenderer>().transform.up = new Vector2(angle.Item1, angle.Item2);
            }
        });
    }
    public static void ExecuteAction(string actionName, ActionHandler item, params object[] parameters)
    {
        if (lambdaActions.ContainsKey(actionName))
        {
            lambdaActions[actionName].Invoke(item, parameters);
        }

    }
}

