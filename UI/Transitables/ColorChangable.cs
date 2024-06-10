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
public class ColorChangable : Transitable
{
    public static ColorChangable StandardHover() { return new ColorChangable(0.1f, new Vector4(-20, -20, -20, 0), null, true, null, null); }
    public static ColorChangable StandardSelect() { return new ColorChangable(0.55f, new Vector4(-10, -10, 50, 0), null, true, null, null); }

    private bool relativePosition;
    private Image image;

    public ColorChangable(float duration, Color endPosition, AnimationCurve curve = null, bool relativePosition = false, Action onAnimationEnd = null, Action onReturnEnd = null, float returnDuration = -1) : base(duration, onAnimationEnd, onReturnEnd, curve, returnDuration)
    {
        endPosition /= 255;
        this.endPosition = ToolsMath.ColorToVector(endPosition);
        this.relativePosition = relativePosition;
    }

    public override void Initialize()
    {
        base.Initialize();
        component.TryGetComponent(out image);
        if (!this.relativePosition)
        {
            this.endPosition -= ToolsMath.ColorToVector(image.color);
        }
    }

    public override bool ExecuteTransition(float deltaTime)
    {
        float elapsedTemp = elapsed;
        bool finished = base.ExecuteTransition(deltaTime);
        image.color = ToolsMath.VectorToColor(ToolsMath.ColorToVector(image.color) + ComputeLerp(elapsedTemp));

        return finished;
    }
}
