using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StandardHealthBar
{
    [JsonIgnore]
    protected LineRenderer fillBar;
    [JsonIgnore]
    protected LineRenderer damagebar;
    public StandardHealthBar() { }
    public void AddHealthBar(GameObject thisPrefab)
    {
        var bar = UnityEngine.Object.Instantiate(GameObjects.healthBarStandard);
        bar.transform.parent = thisPrefab.transform;
        bar.transform.localPosition = new Vector3(0, thisPrefab.transform.localScale.y - 1, thisPrefab.transform.position.z);
        bar.transform.localScale = thisPrefab.transform.localScale;
        fillBar = bar.transform.GetChild(1).GetComponent<LineRenderer>();
        fillBar.transform.position = new Vector3(0, 0, thisPrefab.transform.position.z + 0.5f);
        fillBar.transform.localPosition = new Vector3(0, 0, 0.5f);
        float multiplier = (thisPrefab.transform.localScale.y - 1) * 2 + 1.2f;
        fillBar.widthMultiplier *= multiplier;
        damagebar = bar.transform.GetChild(0).GetComponent<LineRenderer>();
        damagebar.transform.position = new Vector3(0, 0, thisPrefab.transform.position.z + 0.5f);
        damagebar.transform.localPosition = new Vector3(0, 0, 0.6f);
        damagebar.widthMultiplier *= multiplier;
        var outline = bar.transform.GetChild(2).GetComponent<LineRenderer>();
        outline.widthMultiplier *= multiplier * 0.9f;
    }
    public void UpdateHealthBar(LivedHandler handler)
    {
        if (handler != null && handler.MaxLives != 0)
        {
            damagebar.SetPositions(new Vector3[2] { new Vector3(fillBar.GetPosition(0).x + (fillBar.GetPosition(1).x - fillBar.GetPosition(0).x) * handler.Lives / handler.MaxLives, fillBar.GetPosition(1).y, 0), new Vector3(fillBar.GetPosition(1).x, fillBar.GetPosition(1).y, 0) });
        }
    }
}

