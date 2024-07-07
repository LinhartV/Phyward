using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Armor
{
    public float DamageReduction { get; set; }
    public float DamageAmplifier { get; set; }
    public Func<Shot, Shot> ShotModificator { get; private set; }
    private string shotModificatorDescription;

    public Armor() { }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="damageReduction">Percentage of blocked damage</param>
    /// <param name="damageAmplifier">Multiplicator of dealt damage</param>
    /// <param name="shotModificator">How this armor modifies shot</param>
    public Armor(float damageReduction, float damageAmplifier, Func<Shot, Shot> shotModificator, string shotModificatorDescription)
    {
        this.DamageAmplifier = damageAmplifier;
        this.DamageReduction = damageReduction;
        this.ShotModificator = shotModificator;
        this.shotModificatorDescription = shotModificatorDescription;
    }

    /// <summary>
    /// Returns armor stats in a string format for display
    /// </summary>
    /// <returns></returns>
    public string GetStats()
    {
        return (DamageReduction*100) + "% obrana\n" + DamageAmplifier + "x útok\n" + shotModificatorDescription;
    }
}

