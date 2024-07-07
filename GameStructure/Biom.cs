using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[Serializable]
public class Biom
{
    public Biom() { }
    public string Music { get; protected set; }
    public Level FirstBiomLevel { get; set; }
    public int ScrollsNeeded { get;private set; }
    public int ScrollsCollected { get; set; } = 0;
    //public Dictionary<int, Level> levels = new Dictionary<int, Level>();
    public List<Level> levels = new List<Level>();

    public Biom(string music, int scrollsNeeded, IBiomGenerator generator = null)
    {
        this.Music = music;
        if (generator!=null)
        {
            generator.GenerateBiom(this);
        }
        ScrollsNeeded = scrollsNeeded;
    }
    //tilesets?
    //monsters to be spawned?
    //other objects to be spawned?
}
