﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Armor : Upgradable
{
    public override ICollectableRef DeepClone()
    {
        return new Armor();
    }
}

