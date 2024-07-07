using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Tutorial;

public class TutorialTrigger : Item
{
    public TutorialBlock TutorialBlock { get; set; }
    [JsonProperty]
    private bool triggerOnLevelEnter;
    [JsonProperty]
    private bool destroyOnCollision;
    public bool IsActive { get; set; }
    public TutorialTrigger() { }
    public TutorialTrigger(TutorialBlock tutorialBlock, bool triggerOnLevelEnter = false, bool destroyOnCollision = true, bool isActive = true) : base(GameObjects.empty, false)
    {
        this.triggerOnLevelEnter = triggerOnLevelEnter;
        TutorialBlock = tutorialBlock;
        IsActive = true;
        this.destroyOnCollision = destroyOnCollision;
    }

    protected override void SetupItem()
    {
        base.SetupItem();
    }
    public override void OnCollisionEnter(Item collider)
    {
        base.OnCollisionEnter(collider);
        if (collider is Player && IsActive)
        {
            Tutorial.ShowTutorial(TutorialBlock);
            if (destroyOnCollision)
            {
                Dispose();
            }
        }
    }
    public override void OnLevelEnter()
    {
        base.OnLevelEnter();
        if (triggerOnLevelEnter)
        {
            OnCollisionEnter(GCon.game.Player);
        }
        
    }
}
