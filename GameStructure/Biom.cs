using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[Serializable]
public class Biom
{
    public Biom() { }
    public string Music { get; private set; }
    public ToolsGame.BiomType BiomType { get; private set; }
    public Dictionary<int, Level> levels = new Dictionary<int, Level>();

    public Biom(string music, ToolsGame.BiomType biomType, IBiomGenerator generator, GameControl gameControlReference)
    {
        this.Music = music;
        BiomType = biomType;
        levels = generator.GenerateBiom(this, gameControlReference);
    }

    //tilesets?
    //monsters to be spawned?
    //other objects to be spawned?
}
