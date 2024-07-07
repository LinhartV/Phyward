using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Transitable
{
    protected AnimationCurve curve;
    protected float elapsed = 0;
    private float duration;
    private float returnDuration;
    public Action onForwardEnd;
    public Action onReturnEnd;
    public Component component;
    public bool returning = false;
    protected Vector4 endPosition;

    protected Transitable(float duration, Action onAnimationEnd, Action onReturnEnd, AnimationCurve curve, float returnDuration = -1)
    {
        if (curve != null)
            this.curve = curve;
        else
            this.curve = ToolsUI.linear;
        this.duration = duration;
        this.onForwardEnd = onAnimationEnd;
        this.onReturnEnd = onReturnEnd;
        if (returnDuration == -1)
            this.returnDuration = duration;
        else
            this.returnDuration = returnDuration;
    }
    protected float GetDuration()
    {
        return returning ? this.returnDuration : this.duration;
    }
    public virtual void Initialize()
    {

    }
    public void EndAbruptly()
    {
        if (!returning)
        {
            ExecuteTransition(-elapsed);
        }
        else
        {
            ExecuteTransition(GetDuration() - elapsed);
        }
    }
    public virtual void Start()
    {
        if (returning)
        {
            endPosition *= -1;
        }
        returning = false;
        if (elapsed != 0)
        {
            this.elapsed = duration - (elapsed / returnDuration * duration);
        }

    }
    protected Vector4 ComputeLerp(float elapsedTemp)
    {
        if (returning)
            return -Vector4.LerpUnclamped(Vector4.zero, endPosition, curve.Evaluate((GetDuration() - elapsed) / GetDuration())) + Vector4.LerpUnclamped(Vector4.zero, endPosition, curve.Evaluate((GetDuration() - elapsedTemp) / GetDuration()));
        else
            return Vector4.LerpUnclamped(Vector4.zero, endPosition, curve.Evaluate(elapsed / GetDuration())) - Vector4.LerpUnclamped(Vector4.zero, endPosition, curve.Evaluate(elapsedTemp / GetDuration()));

    }
    public virtual void Return()
    {
        if (returning == false)
        {
            returning = true;
            endPosition *= -1;
            this.elapsed = returnDuration - (elapsed / duration * returnDuration);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <returns>Returns true if transition ended completely</returns>
    public virtual bool ExecuteTransition(float deltaTime)
    {
        elapsed += deltaTime;
        if (elapsed >= GetDuration())
        {
            elapsed = GetDuration();
            return true;
        }
        return false;
    }
    public void ResetTransition()
    {
        if (returning)
        {
            endPosition *= -1;
        }
        this.elapsed = 0;
        returning = false;
    }
    public bool IsForwardComplete()
    {
        return elapsed == duration && !returning;
    }
}