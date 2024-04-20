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
    /// What I carry right now
    /// </summary>
    public Dictionary<Slotable, int> backpack = new Dictionary<Slotable, int>();
    /// <summary>
    /// Space of backpack
    /// </summary>
    public int SlotSpace { get; set; } = 20;
    /// <summary>
    /// All I have right now on my base
    /// </summary>
    private Dictionary<Slotable, int> materials= new Dictionary<Slotable, int>();
    public void AddMaterial(Slotable material)
    {
        if (materials.ContainsKey(material))
        {
            materials[material]++;
        }
        else
        {
            materials.Add(material, 0);
        }
    }
    public void SpendMaterial(Slotable material, int count)
    {
        if (materials.ContainsKey(material) && materials[material] >= count)
        {
            materials[material]-=count;
            if (materials[material] == 0)
            {
                materials.Remove(material);
            }
        }
    }
}
