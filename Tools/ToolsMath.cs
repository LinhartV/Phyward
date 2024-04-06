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
    public static (float, float) PolarToCartesian(double angle, double size)
    {
        return ((float)(size * Math.Sin(angle)), -(float)(size * Math.Cos(angle)));
    }
    static public double GetAngleFromLengts(Item it1, Item it2)
    {
        return GetAngleFromLengts(it2.Prefab.transform.position.x - it1.Prefab.transform.position.x, it2.Prefab.transform.position.y - it1.Prefab.transform.position.y);
    }
    static public double GetAngleFromLengts(double xlength, double ylength)
    {
        double newAngle;
        if (xlength == 0)
        {
            newAngle = Math.PI / 2;
        }
        else
        {
            newAngle = Math.Atan(Math.Abs(ylength / xlength));
        }
        if (xlength >= 0 && ylength <= 0)
            newAngle = 2 * Math.PI - newAngle;
        else if (xlength <= 0 && ylength <= 0)
            newAngle = Math.PI + newAngle;
        else if (xlength <= 0 && ylength >= 0)
            newAngle = Math.PI - newAngle;
        return newAngle;
    }
}
