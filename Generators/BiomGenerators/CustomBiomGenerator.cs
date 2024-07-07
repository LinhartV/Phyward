using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class CustomGenerator : IBiomGenerator
{
    public Dictionary<int, Level> structures;
    public CustomGenerator(int cellSize) : base(0, cellSize, 0, true, 0, null)
    {
    }

    public override void GenerateBiom(Biom biom)
    {
    }
}