using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
[Serializable]
public class DestroyableBlock : Block, ILived
{
    public DestroyableBlock()
    {
        this.IsSolid = true;
    }

    public DestroyableBlock(float lives,Vector2 pos, GameObject prefab = null) : base(pos, prefab == null ? GameObjects.block : prefab)
    {
        LivedHandler = new LivedHandler(this,lives);
    }

    public LivedHandler LivedHandler { get; protected set; }

    public StandardHealthBar HealthBar { get; set; }

    public void AddHealthBar()
    {
        HealthBar.AddHealthBar(this.Prefab);
    }

    public bool Death()
    {
        this.Dispose();
        return true;
    }

    public void UpdateHealthBar()
    {
        HealthBar.UpdateHealthBar(LivedHandler);
    }
}

