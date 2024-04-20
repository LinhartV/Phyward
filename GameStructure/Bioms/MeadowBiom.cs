using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[Serializable]
public class MeadowBiom : Biom
{
    public MeadowBiom() { }

    public MeadowBiom(string music, IBiomGenerator generator)
    {
        this.Music = music;
        levels = generator.GenerateBiom(this);
    }


    //tilesets?
    //monsters to be spawned?
    //other objects to be spawned?
}
