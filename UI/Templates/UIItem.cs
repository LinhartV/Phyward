using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Progress;

public class UIItem : ActionHandler
{
    /// <summary>
    /// GameObject which will be transformed
    /// </summary>
    public GameObject Go { get; protected set; }
    /// <summary>
    /// GameObject which should react to event
    /// </summary>
    protected GameObject referenceGo;
    //Reference to this UIItem
    public Action<UIItem> OnMouseEnter { private get; set; }
    public Action<UIItem> OnMouseExit { private get; set; }
    public Action<UIItem> OnMouseDown { private get; set; }
    public Action<UIItem> OnMouseUp { private get; set; }
    /// <summary>
    /// all transitions associated with this UIItem
    /// </summary>
    private Dictionary<string, Transitable> transitions = new Dictionary<string, Transitable>();
    /// <summary>
    /// All currently running transitions
    /// </summary>
    private Dictionary<string, Transitable> runningTransitions = new Dictionary<string, Transitable>();
    //public UIItem() : base() { ToolsUI.UIItems.Add(this); }

    public bool hoverActivated;
    public bool clickActivated;

    public UIItem(GameObject go, GameObject referenceGo, ToolsSystem.PauseType pauseType, params ToolsSystem.PauseType[] pauseTypes) : base(true, pauseType, pauseTypes)
    {
        SetupUIItem(go, referenceGo);
        ToolsUI.UIItems.Add(this);
    }
    public UIItem(GameObject go, ToolsSystem.PauseType pauseType, params ToolsSystem.PauseType[] pauseTypes) : base(true, pauseType, pauseTypes)
    {
        SetupUIItem(go);
        ToolsUI.UIItems.Add(this);
    }
    public UIItem(GameObject go, GameObject referenceGo = null) : base(true, ToolsSystem.PauseType.Inventory)
    {
        SetupUIItem(go, referenceGo);
        ToolsUI.UIItems.Add(this);
    }
    /// <param name="go">Game object to transform</param>
    /// <param name="referenceGo">Game object which reacts to event</param>
    protected void SetupUIItem(GameObject go, GameObject referenceGo = null)
    {
        if (go == null)
        {
            return;
        }
        this.Go = go;
        UIScript just;
        if (!go.TryGetComponent<UIScript>(out just))
        {
            var scr = this.Go.AddComponent<UIScript>();
            scr.item = this;
        }
        this.referenceGo = referenceGo == null ? go : referenceGo;
    }
    public virtual void OnMouseEnterDefault()
    {
        if (this.pauseTypes.Contains(GCon.GetPausedType()))
        {
            if (OnMouseEnter != null)
            {
                OnMouseEnter(this);
            }
        }
        hoverActivated = true;

    }
    public virtual void OnMouseExitDefault()
    {
        if (this.pauseTypes.Contains(GCon.GetPausedType()))
        {
            if (OnMouseExit != null)
            {
                OnMouseExit(this);
            }
        }
        hoverActivated = false;

    }
    public virtual void OnMouseDownDefault()
    {
        if (this.pauseTypes.Contains(GCon.GetPausedType()))
        {
            if (OnMouseDown != null)
            {
                OnMouseDown(this);
            }
        }
        clickActivated = true;

    }
    public virtual void OnMouseUpDefault()
    {
        if (this.pauseTypes.Contains(GCon.GetPausedType()))
        {
            if (OnMouseUp != null)
            {
                OnMouseUp(this);
            }
        }
        clickActivated = false;

    }
    public void StartTransition(string name = "", bool onlyIfEnded = false)
    {
        if (transitions.ContainsKey(name))
        {
            if (!runningTransitions.ContainsKey(name) /*&& ((!transitions[name].CanBeReturned() && onlyIfEnded) || !onlyIfEnded)*/)
            {
                transitions[name].Start();
                runningTransitions.Add(name, transitions[name]);
            }
            else if (!onlyIfEnded)
            {
                transitions[name].Start();
            }
            if (!ToolsUI.transitables.Contains(this))
            {
                ToolsUI.transitables.Add(this);
            }
        }
    }
    public void ReturnTransition(string name)
    {
        //if transition can be returned, than return it
        if (transitions.ContainsKey(name) && (runningTransitions.ContainsKey(name) || transitions[name].CanBeReturned()))
        {
            transitions[name].Return();
            if (!runningTransitions.ContainsKey(name))
            {
                runningTransitions.Add(name, transitions[name]);
                if (!ToolsUI.transitables.Contains(this))
                {
                    ToolsUI.transitables.Add(this);
                }
            }
        }
    }
    public void AddTransition(Transitable transitable, string name = "")
    {
        if (!transitions.ContainsKey(name))
        {
            transitable.component = this.referenceGo.transform;
            transitable.Initialize();
            transitions.Add(name, transitable);
        }
    }
    public void AddAnimation(Transitable animation, Transitable beginTransition = null, string name = "")
    {
        if (!transitions.ContainsKey(name + "animation"))
        {
            AddTransition(animation, name + "animation");
        }
        animation.onAnimationEnd = () =>
        {
            ReturnTransition(name + "animation");
        };
        animation.onReturnEnd = () =>
        {
            AddTransition(animation, name + "animation");
            StartTransition(name + "animation");
        };
        if (beginTransition != null)
        {
            AddTransition(beginTransition, name + "beginAnimation");
            StartTransition(name + "beginAnimation");
            beginTransition.onAnimationEnd = () =>
            {
                StartTransition(name + "animation");
            };
        }
        else
        {
            StartTransition(name + "animation");
        }

    }
    public void EndAnimation(string name = "", bool endAbruptly = false)
    {
        if (transitions.ContainsKey(name + "animation"))
        {
            if (endAbruptly)
            {
                transitions[name + "animation"].EndAbruptly();
                EndTransition(name + "animation", true);
                transitions[name + "animation"].ResetTransition();
            }
            else
            {
                transitions[name + "animation"].onReturnEnd = () => { };
            }
            ReturnTransition(name + "beginAnimation");
        }
    }
    public void ProcedeTransitions(float deltaTime)
    {
        var temp = new Dictionary<string, Transitable>(runningTransitions);
        foreach (var transition in temp)
        {
            if (transition.Value.ExecuteTransition(deltaTime))
                EndTransition(transition.Key);
        }
    }

    public override void InnerDispose()
    {
        base.InnerDispose();

        if (Go != null)
        {
            try
            {
                UnityEngine.Object.Destroy(this.Go);
                UnityEngine.Object.Destroy(this.referenceGo);
            }
            catch (Exception)
            {
            }

        }
        if (ToolsUI.transitables.Contains(this))
        {
            ToolsUI.transitables.Remove(this);
        }

    }


    private void EndTransition(string name, bool endingAnimation = false)
    {
        runningTransitions.Remove(name);
        if (!endingAnimation)
        {
            if (transitions[name].returning)
            {
                if (transitions[name].onReturnEnd != null)
                {
                    transitions[name].onReturnEnd();
                }
            }
            else
            {
                if (transitions[name].onAnimationEnd != null)
                {
                    transitions[name].onAnimationEnd();
                }
            }
        }
        /*if (!transitions[name].returning)
            transitions[name].ResetTransition();*/
        if (runningTransitions.Count == 0)
        {
            ToolsUI.transitables.Remove(this);
        }

    }
}