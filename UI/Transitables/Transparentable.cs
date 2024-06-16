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
public class Transparentable : Transitable
{

    private bool relativePosition;
    private CanvasGroup group;
    /// <param name="endAlpha">Alpha from 0 to 1</param>
    public Transparentable(float duration, float endAlpha, AnimationCurve curve = null, bool relativePosition = false, Action onAnimationEnd = null, Action onReturnEnd = null, float returnDuration = -1) : base(duration, onAnimationEnd, onReturnEnd, curve, returnDuration)
    {
        this.endPosition = new Vector4(endAlpha,1);
        this.relativePosition = relativePosition;
    }

    public override void Initialize()
    {
        base.Initialize();
        component.TryGetComponent(out group);
        if (!this.relativePosition)
        {
            this.endPosition -= new Vector4(group.alpha, 1);
        }
    }

    public override bool ExecuteTransition(float deltaTime)
    {
        float elapsedTemp = elapsed;
        bool finished = base.ExecuteTransition(deltaTime);
        group.alpha +=  ComputeLerp(elapsedTemp).x;

        return finished;
    }
}
