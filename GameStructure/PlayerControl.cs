using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Class for controling player stats (not player as ingame character, but player as current game session)
/// </summary>
[Serializable]
public class PlayerControl
{
    public PlayerControl() { }
    /// <summary>
    /// not to be called by JSON
    /// </summary>
    /// <param name="just"></param>
    public PlayerControl(bool just = true)
    {
        SetupUnitCount();
    }

    public Dictionary<PreUnit, int> unitCount = new Dictionary<PreUnit, int>();
    public void SetupUnitCount()
    {
        foreach (var unit in Units.allUnits)
        {
            unitCount.Add(unit, 0);
        }
    }
}
