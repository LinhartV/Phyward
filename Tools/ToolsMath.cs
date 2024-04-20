using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Tools for game management
/// </summary>
public static class ToolsMath
{
    public static T GetRandomElement<T>(T[] array)
    {
        return array[ToolsGame.Rng(0, array.Length)];
    }
    /// <summary>
    /// Changes angle from polar coordinates to cartesian
    /// </summary>
    /// <returns>width and height</returns>
    public static (float, float) PolarToCartesian(float angle, float size)
    {
        float x, y;
        (x, y) = ((float)(size * Math.Sin(angle)), (float)(size * Math.Cos(angle)));
        if (Math.Abs(x) < Constants.MIN_VALUE)
            x = 0;
        if (Math.Abs(y) < Constants.MIN_VALUE)
            y = 0;
        return (x, y);
    }
    /// <summary>
    /// Changes width and height to polar coordinates
    /// </summary>
    /// <returns>(angle, size)</returns>
    public static (float, float) CartesianToPolar(float width, float height)
    {
        return (GetAngleFromLengts(width, height), (float)Math.Sqrt(width * width + height * height));
    }
    static public float GetAngleFromLengts(Item it1, Item it2)
    {
        return GetAngleFromLengts(it2.Prefab.transform.position.x - it1.Prefab.transform.position.x, it2.Prefab.transform.position.y - it1.Prefab.transform.position.y);
    }
    /// <summary>
    /// Get angle between two items
    /// </summary>
    /// <param name="it1">"him"</param>
    /// <param name="it2">"me"</param>
    /// <returns></returns>
    static public float GetDistance(Item it1, Item it2)
    {
        return (float)Math.Sqrt(Math.Pow(it1.Prefab.transform.position.x - it2.Prefab.transform.position.x, 2) + Math.Pow(it1.Prefab.transform.position.y - it2.Prefab.transform.position.y, 2));
    }
    static public float GetAngleFromLengts(float xlength, float ylength)
    {
        float newAngle;
        if (xlength == 0)
        {
            newAngle = 0;
        }
        else
        {
            newAngle = (float)Math.Atan(Math.Abs(xlength / ylength));
        }
        if (xlength >= 0 && ylength <= 0)
            newAngle = (float)(Math.PI - newAngle);
        else if (xlength <= 0 && ylength <= 0)
            newAngle = (float)Math.PI + newAngle;
        else if (xlength <= 0 && ylength >= 0)
            newAngle = (float)(2 * Math.PI - newAngle);
        return newAngle;
    }
    /// <summary>
    /// Converts seconds to frames
    /// </summary>
    public static float SecondsToFrames(float seconds)
    {
        return seconds / GCon.percentageOfFrame;
    }
}
