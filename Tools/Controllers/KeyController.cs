﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RegisteredKey;

/// <summary>
/// Class for controlling actions assigned to particular keys by their keycode
/// </summary>
public static class KeyController
{
    //Dictionary of actions assigned to particular key (string is acutally the keycode of the key)
    public static Dictionary<UnityEngine.KeyCode, Stack<RegisteredKey>> registeredKeys = new();

    //gets the registeredKey on top of the stack for particulare keycode.
    public static RegisteredKey GetRegisteredKey(UnityEngine.KeyCode keyName)
    {
        if (registeredKeys.ContainsKey(keyName))
        {
            foreach (var key in registeredKeys[keyName])
            {
                if ((GCon.Paused && key.PauseTypeKey != PauseType.onResume) || (!GCon.Paused && key.PauseTypeKey != PauseType.onPause))
                {
                    return key;
                }
            }
            return null;
        }
        else
            return null;
    }
    //Pops the topmost functionality of particular key (if W is for move up, but in settings W is for new soundtrack (idk), than after return to the game, the setting functionality can be popped)
    public static void PopKey(UnityEngine.KeyCode keyName)
    {
        if (registeredKeys.ContainsKey(keyName) && registeredKeys[keyName].Count > 0)
            registeredKeys[keyName].Pop();
    }
    /// <summary>
    ///Pushes or creates new functionality for particular key 
    /// </summary>
    /// <param name="keyName">Name of key (lowercase)</param>
    /// <param name="registeredKey"></param>
    /// <param name="additionalKeys">List of keys to do the same</param>
    public static void AddKey(UnityEngine.KeyCode keyName, RegisteredKey registeredKey, params UnityEngine.KeyCode[] additionalKeys)
    {
        if (!registeredKeys.ContainsKey(keyName))
            registeredKeys.Add(keyName, new Stack<RegisteredKey>());
        registeredKeys[keyName].Push(registeredKey);
        foreach (var item in additionalKeys)
        {
            if (!registeredKeys.ContainsKey(item))
                registeredKeys.Add(item, new Stack<RegisteredKey>());
            registeredKeys[item].Push(registeredKey);
        }
    }
    public static void SetPressedStateToOtherSameKeys(UnityEngine.KeyCode key, bool pressed)
    {
        foreach (var item in registeredKeys[key])
        {
            item.Pressed = pressed;
        }
    }
}
