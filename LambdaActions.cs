﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
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

    public static void AddItemLambdaAction(string name, ActionHandler item, LambdaAction action)
    {
        lambdaActions.Add(name + item.GetType().Name, action);
    }

    public static void SetupLambdaActions()
    {
        lambdaActions.Add("addDelayedAction", (item, parameter) =>
        {
            item.AddAction(parameter[0] as ItemAction, 0, (ActionHandler.RewriteEnum)parameter[1]);
        });
        lambdaActions.Add("receiveDamage", (item, parameter) =>
        {
            var character = (item as Character);
            float difference = (float)Convert.ToDouble(parameter[0]);
            character.LivedHandler.ReceiveDamage(difference);
        });
        lambdaActions.Add("randomWalk", (item, parameter) =>
        {
            if ((item as Movable).MovementsControlled["randomMovement"].MovementSpeed == 0)
            {
                ((item as Movable).MovementsControlled["randomMovement"] as AcceleratedMovement).ResetRealMovementSpeed((item as Movable).BaseSpeed / 4);
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
        lambdaActions.Add("disposeShot", (item, parameter) =>
        {
            (item as Shot).OnLand();
        });
        lambdaActions.Add("death", (item, parameter) =>
        {
            (item as Character).Death();
        });
        lambdaActions.Add("faceInDirection", (item, parameter) =>
        {
            var angle = ToolsMath.PolarToCartesian((item as Item).Angle, 1);
            (item as Item).Prefab.GetComponentInChildren<SpriteRenderer>().transform.up = new Vector2(angle.Item1, angle.Item2);

        });
        lambdaActions.Add("followCursor", (item, parameter) =>
        {
            (item as UIItem).Go.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        });
        lambdaActions.Add("unfreeze", (item, parameter) =>
        {
            GCon.freezeCamera = false;
        });
        lambdaActions.Add("heal", (item, parameter) =>
        {
            GCon.game.Player.LivedHandler.ChangeLives((float)parameter[0]);
        });
        lambdaActions.Add("acceleratedHeal", (item, parameter) =>
        {
            GCon.game.Player.LivedHandler.ChangeLives((float)parameter[0]);
            GCon.game.Player.ChangeItemActionParameter((string)parameter[parameter.Length - 1], 0, (float)parameter[0] + 0.001f);
        });
        lambdaActions.Add("stopBonus", (item, parameter) =>
        {
            GCon.game.Player.DeleteAction((string)parameter[0]);
        });
        lambdaActions.Add("moveOneTileUp", (item, parameter) =>
        {
            GCon.game.Player.Prefab.transform.position = new Vector2(GCon.game.Player.Prefab.transform.position.x, GCon.game.Player.Prefab.transform.position.y + 1);
        });
        lambdaActions.Add("stopFastReload", (item, parameter) =>
        {
            GCon.game.Player.CharReloadTime *= 2;
        });
        lambdaActions.Add("slotCountdown", (item, parameter) =>
        {
            LoadingSlotTemplate lst = parameter[0] as LoadingSlotTemplate;
            lst.SetLoadingValue(GCon.frameTime);
        });
        lambdaActions.Add("stopSpeedIncrease", (item, parameter) =>
        {
            GCon.game.Player.BaseSpeed /= 1.5f;
        });

        lambdaActions.Add("ShowDescription", (item, parameter) =>
        {
            if (item == null || (item as SlotTemplate).Go == null || (item as SlotTemplate).SlotableRef.Prefab == null || !(item as SlotTemplate).Go.activeInHierarchy)
            {
                item.DeleteAction("ShowDescription");
                return;
            }
            var ui = (item as SlotTemplate).SlotableRef;
            ToolsUI.descriptionPanel.Go.SetActive(true);
            ToolsUI.descriptionPanel.Go.transform.position = /*Camera.main.ScreenToWorldPoint(new Vector3(0, 0)) +*/ (item as UIItem).Go.transform.position;
            var rect = ToolsUI.descriptionPanel.Go.GetComponent<RectTransform>();



            if (Camera.main.WorldToScreenPoint(new Vector3(0, rect.position.y - 0.5f * rect.rect.height)).y < 0)
            {
                ToolsUI.descriptionPanel.Go.transform.position = new Vector3(ToolsUI.descriptionPanel.Go.transform.position.x, Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).y + 0.5f * rect.rect.height);
            }
            if (Camera.main.WorldToScreenPoint(new Vector3(0, rect.position.y + 0.5f * rect.rect.height)).y > Screen.height)
            {
                ToolsUI.descriptionPanel.Go.transform.position = new Vector3(ToolsUI.descriptionPanel.Go.transform.position.x, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height)).y - 0.5f * rect.rect.height);
            }
            if (Camera.main.WorldToScreenPoint(new Vector3(rect.position.x - 0.5f * rect.rect.width, 0)).x < 0)
            {
                ToolsUI.descriptionPanel.Go.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.5f * rect.rect.width, ToolsUI.descriptionPanel.Go.transform.position.y);
            }
            if (Camera.main.WorldToScreenPoint(new Vector3(rect.position.x + 0.5f * rect.rect.width, 0)).x > Screen.width)
            {
                ToolsUI.descriptionPanel.Go.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0)).x - 0.5f * rect.rect.width, ToolsUI.descriptionPanel.Go.transform.position.y);
            }
            rect.pivot = new Vector2(0.5f, 0.5f);
            ToolsUI.descriptionPanel.Go.transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ui.Name;
            ToolsUI.descriptionPanel.Go.transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ui.Subheading;
            ToolsUI.descriptionPanel.Go.transform.GetChild(2).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ui.Description;
            ToolsUI.descriptionPanel.Go.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = ui.Prefab.transform.GetChild(0).gameObject.GetComponent<Image>().sprite;
        });
        lambdaActions.Add("swirl", (item, parameter) =>
        {
            var shot = item as SwarmShot;
            shot.RotateControlledMovement("swirl", (int)parameter[0] * ToolsGame.Rng() * (float)Math.PI / 10 * (float)parameter[1]);
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

