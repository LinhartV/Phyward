using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for handling animation using Lerp function
/// </summary>
public class Scalable : Transitable
{
    public static ColorChangable StandardHover() { return new ColorChangable(0.1f, new Vector4(-20, -20, -20, 0), null, true, null, null); }
    public static ColorChangable StandardSelect() { return new ColorChangable(0.55f, new Vector4(-10, -10, 50, 0), null, true, null, null); }

    private bool relativePosition;

    public Scalable(float duration, Vector3 endScale, AnimationCurve curve = null, bool relativePosition = false, Action onAnimationEnd = null, Action onReturnEnd = null, float returnDuration = -1) : base(duration, onAnimationEnd, onReturnEnd, curve, returnDuration)
    {
        this.endPosition = endScale;
        this.relativePosition = relativePosition;
    }

    public override void Initialize()
    {
        base.Initialize();
        var scale = component.transform.localScale;
        if (!this.relativePosition)
        {
            endPosition = endPosition - new Vector4(scale.x, scale.y);
        }
    }

    public override bool ExecuteTransition(float deltaTime)
    {
        float elapsedTemp = elapsed;
        bool finished = base.ExecuteTransition(deltaTime);
        component.transform.localScale += (Vector3)ComputeLerp(elapsedTemp);

        return finished;
    }
}
