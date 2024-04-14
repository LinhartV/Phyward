using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Units
{
    public static PreUnit time = new PreUnit("Čas", "Prostě za jak dlouho se něco stane... ", "sekunda [s]");
    public static PreUnit distance = new PreUnit("Dráha", "Jaká je vzdálenost, kolik to měří, jak to je daleko...", "metr [m]");
    public static PreUnit mass = new PreUnit("Hmotnost", "Kolik to váží", "kilogram [kg]");
    public static PreUnit current = new PreUnit("Proud", "Jak moc proudí elektřina", "ampér [A]");
    public static PreUnit luminiscence = new PreUnit("Svítivost", "Udává intenzitu světla", "kandela [cd]");
    public static PreUnit temperature = new PreUnit("Teplota", "Čím vyšší teplota, tím to je teplejší, čím nižší teplota, tím to je studenější.", "stupeň Celsia [°C]");
    public static PreUnit speed = new PreUnit("Rychlost", "Kolik metrů urazíš za jednu sekundu", "[m/s]", new List<PreUnit>() { distance }, new List<PreUnit>() { time });
    public static PreUnit acceleration = new PreUnit("Zrychlení", "O kolik zvýšíš svoji rychlost za jednu sekundu", "[m/s^2]", new List<PreUnit>() { speed }, new List<PreUnit>() { time });
    public static PreUnit force = new PreUnit("Síla", "Vyjadřuje, jak moc na sebe tělesa působí", "newton [N]", new List<PreUnit>() { mass, acceleration }, new List<PreUnit>() { });
    public static PreUnit work = new PreUnit("Práce", "Působení síly po dráze - když zvednu kámen nad hlavu, působil jsem silou třeba 10 N a urazil jsem třeba 2 metry, takže práce je 20 J, tradá.", "joule [J]", new List<PreUnit>() { force, distance }, new List<PreUnit>() { });
    public static PreUnit power = new PreUnit("Výkon", "Kolik práce vykonám za jednu sekundu", "watt [W]", new List<PreUnit>() { work }, new List<PreUnit>() { time });
    public static PreUnit frequency = new PreUnit("Frekvence", "Kolikrát se něco stane za sekundu", "hertz [Hz]", new List<PreUnit>() { }, new List<PreUnit>() { time });

    public static PreUnit voltage = new PreUnit("Napětí", "Rozdíl elektrického potenciálu mezi dvěma body...", "volt [V]", new List<PreUnit>() { power }, new List<PreUnit>() { current });
    public static PreUnit resistance = new PreUnit("Odpor", "Udává, jak moc se prostředí brání proti vedení proudu", "ohm [Ω]", new List<PreUnit>() { voltage }, new List<PreUnit>() { current });
    public static PreUnit charge = new PreUnit("Náboj", "Jak moc je něco nabité... vím, skvělé vysvětlení... ", "coulomb [C]", new List<PreUnit>() { time, current }, new List<PreUnit>() { });
    public static PreUnit capacitance = new PreUnit("Capacita", "Udává schopnost udržet si náboj... na způsob 'nabít, pal!' ", "farad [F]", new List<PreUnit>() { charge }, new List<PreUnit>() { voltage });
    public static PreUnit area = new PreUnit("Obsah", "Velikost nějaké plochy", "[m^2]", new List<PreUnit>() { distance, distance }, new List<PreUnit>() { });
    public static PreUnit volume = new PreUnit("Objem", "Velikost tělesa", "[m^3]", new List<PreUnit>() { distance, distance, distance }, new List<PreUnit>() { });
    public static PreUnit density = new PreUnit("Hustota", "Kolik váží metr krychlový dané látky", "[kg/m^3]", new List<PreUnit>() { mass }, new List<PreUnit>() { volume });
    public static PreUnit pressure = new PreUnit("Tlak", "Jak moc působí síla na danou plochu", "pascal [Pa]", new List<PreUnit>() { force }, new List<PreUnit>() { area });
    public static PreUnit inertia = new PreUnit("Hybnost", "Schopnost tělesa se pohybovat a svůj pohyb si uchovat", "[kg*m]", new List<PreUnit>() { speed, mass }, new List<PreUnit>() { });


    public static List<PreUnit> allUnits = new List<PreUnit>() { time, distance, mass, current, luminiscence, temperature, speed, acceleration, force, work, power, frequency, voltage, charge, capacitance, area, volume, density, pressure, inertia};



    public static PreUnit ComposeUnit(List<PreUnit> numerator, List<PreUnit> denominator)
    {
        List<PreUnit> composedNumerator = new List<PreUnit>();
        List<PreUnit> composedDenominator = new List<PreUnit>();
        foreach (var unit in numerator)
        {
            composedNumerator.AddRange(unit.unitNumeratorList);
            composedDenominator.AddRange(unit.unitDenominatorList);
        }
        foreach (var unit in denominator)
        {
            composedDenominator.AddRange(unit.unitNumeratorList);
            composedNumerator.AddRange(unit.unitDenominatorList);
        }
        FractionReduction(ref composedNumerator, ref composedDenominator);
        foreach (var unit in allUnits)
        {
            if (ToolsSystem.ScrambledEquals(unit.unitNumeratorList, composedNumerator) && ToolsSystem.ScrambledEquals(unit.unitDenominatorList, composedDenominator))
            {
                return unit;
            }
        }
        return null;
    }
    public static void FractionReduction(ref List<PreUnit> numeratorList, ref List<PreUnit> denominatorList)
    {
        for (int i = 0; i < numeratorList.Count; i++)
        {
            for (int j = 0; j < denominatorList.Count; j++)
            {
                if (j != i && numeratorList[i] == denominatorList[j])
                {
                    numeratorList.Remove(numeratorList[i]);
                    denominatorList.Remove(denominatorList[j]);
                    j--;
                    i--;
                    continue;
                }
            }
        }
    }


}

