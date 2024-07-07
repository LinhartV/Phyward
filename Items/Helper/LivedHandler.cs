using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handler for anything that can have lives
/// </summary>
public class LivedHandler
{
    public LivedHandler() { }
    private ILived lived;
    public LivedHandler(ILived lived, float lives)
    {
        this.lived = lived;
        Lives = lives;
        MaxLives = lives;
    }
    private float lives;
    public float Lives
    {
        get => lives;
        private set
        {
            lives = value;
            if (GCon.GameStarted)
            {
                lived.UpdateHealthBar();
            }
        }
    }
    public float MaxLives { get; set; }
    public void ChangeLives(float change, bool relative = true)
    {
        if (GCon.game.TutorialPhase < 5 && lived is Player pl && Lives < 5 && change < 0)
        {
            if (!GCon.game.showedLowHealth)
            {
                GCon.game.showedLowHealth = true;
                Tutorial.ShowTutorial(new Tutorial.TutorialBlock("Umřel jsi", "Cítíš, jak tvé tělo zkomírá a pomalu opouštíš tento svět...", "Pak si ale uvědomíš, že jsi v tutoriálu a tudíš je vše ok."));
            }
        }
        else
            this.Lives = relative ? this.Lives + change : change;
        if (Lives > MaxLives)
        {
            Lives = MaxLives;
        }
        if (Lives <= 0)
        {
            Lives = 0;
            lived.Death();
        }

    }
    public void ReceiveDamage(float damage)
    {
        if ((lived is Character c) && c.Armor != null)
            ChangeLives(-damage * (1 - c.Armor.DamageReduction));
        else
            ChangeLives(-damage);
    }
}