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
    protected int lastAddedExitDirection;
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
    public Vector3 GetExitPos(int exitId)
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
    }
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
            lastAddedExitDirection = direction;
        }
    }
    public void PopExit()
    {
        ExitsAr[lastAddedExitDirection].RemoveAt(ExitsAr[lastAddedExitDirection].Count - 1);
    }
}

