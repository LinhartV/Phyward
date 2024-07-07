using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ExitHandler
{
    /// <summary>
    /// Top right bottom left exits
    /// </summary>
    public List<PreExit>[] ExitsAr { get; protected set; } = new List<PreExit>[4];
    public int ExitId { get; set; } = 0;
    public int Id { get; set; }

    //Position of lastly added exit
    public int LastAddedExitDirection { get; protected set; }
    public int FirstAddedExitDirection { get; protected set; } = -1;
    public ExitHandler() { }
    //Just not to be triggered by JSON
    public ExitHandler(bool loading)
    {
        for (int i = 0; i < ExitsAr.Length; i++)
        {
            ExitsAr[i] = new List<PreExit>();
        }
    }
    /// <summary>
    /// Gets position of exit by it's id
    /// </summary>
    /// <returns>Position of found exit</returns>
    /*public Vector3 GetExitPos(int exitId)
    {
        for (int i = 0; i < ExitsAr.Length; i++)
        {
            for (int j = 0; j < ExitsAr[i].Count; j++)
            {
                if (ExitsAr[i][j].ExitId == exitId)
                {
                    return new Vector3(ExitsAr[i][j].X, ExitsAr[i][j].Y);
                }
            }
        }
        return Vector3.one;
    }*/
    /// <summary>
    /// Adds a new exit
    /// </summary>
    public void AddExitPair(int direction, PreExit exit1, PreExit exit2, ExitHandler otherLevel)
    {
        if (direction < 4 && direction >= 0)
        {
            ExitId = Math.Max(ExitId, otherLevel.ExitId) + 1;
            otherLevel.ExitId = ExitId;
            exit2.ExitId = ExitId;
            exit1.ExitId = ExitId;
            ExitsAr[direction].Add(exit1);
            otherLevel.ExitsAr[(direction + 2) % 4].Add(exit2);
            otherLevel.LastAddedExitDirection = (direction + 2) % 4;
            LastAddedExitDirection = direction;
            if (FirstAddedExitDirection == -1)
            {
                FirstAddedExitDirection = direction;
            }
            if (otherLevel.FirstAddedExitDirection == -1)
            {
                otherLevel.FirstAddedExitDirection = (direction + 2) % 4;
            }
        }
    }
    public void PopExit()
    {
        ExitsAr[LastAddedExitDirection].RemoveAt(ExitsAr[LastAddedExitDirection].Count - 1);
    }
    /// <summary>
    /// Gets one exit which player (probably) didn't use to first get into this room
    /// </summary>
    public Vector2 GetOutExitPosition(bool getInExit = false)
    {
        if (!getInExit)
        {
            return new Vector2(ExitsAr[LastAddedExitDirection][0].X, ExitsAr[LastAddedExitDirection][0].Y);
        }
        else
        {
            return new Vector2(ExitsAr[FirstAddedExitDirection][0].X, ExitsAr[FirstAddedExitDirection][0].Y);
        }
    }
}

