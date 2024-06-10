using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class for handling animation using Lerp function
/// </summary>
public class Lerpable : Transitable
{
    private bool localPosition;
    private bool relativePosition;
    /// <summary>
    /// Initialize Lerpable object and immidiatelly starts animation
    /// </summary>
    /// <param name="component">What is being animated</param>
    /// <param name="duration">Duration of animation</param>
    /// <param name="endPosition">Where it should end</param>
    /// <param name="curve">Animation curve - leave null for linear</param>
    /// <param name="relativePosition">Whether endPosition is taken relatively to current position</param>
    /// <param name="localPosition">Whether to use localPosition or position (just Unity stuff...)</param>
    /// <param name="onAnimationEnd">What should happen when animation ends (forward)</param>
    /// <param name="onReturnEnd">What should happen when returning animation ends (returning)</param>
    public Lerpable(float duration, Vector2 endPosition, AnimationCurve curve = null, bool relativePosition = false, bool localPosition = false, Action onAnimationEnd = null, Action onReturnEnd = null, float returnDuration = -1) : base(duration, onAnimationEnd, onReturnEnd, curve, returnDuration)
    {
        this.endPosition = endPosition;
        this.localPosition = localPosition;
        this.relativePosition = relativePosition;
    }

    public override void Initialize()
    {
        base.Initialize();
        if (!this.relativePosition)
        {
            this.endPosition -= (Vector4)(localPosition ? component.transform.localPosition : component.transform.position);
        }
    }

    public override bool ExecuteTransition(float deltaTime)
    {
        float elapsedTemp = elapsed;
        bool finished = base.ExecuteTransition(deltaTime);
        Vector4 newPos = ComputeLerp(elapsedTemp);
        if (localPosition)
            component.transform.localPosition += (Vector3)newPos;
        else
            component.transform.position += (Vector3)newPos;

        return finished;
    }

}
