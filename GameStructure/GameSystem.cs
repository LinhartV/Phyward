using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// System for the game - inventory is one system, animations, menu, gameplay...
/// </summary>
[Serializable]
public class GameSystem
{
    private long now;
    public long NowDifference { get; set; } = 0;
    private Action onActivation;
    private Action onDeactivation;
    public List<ActionHandler> itemStep = new List<ActionHandler>();

    public GameSystem(Action onActivation = null, Action onDeactivation = null)
    {
        this.onActivation = onActivation;
        this.onDeactivation = onDeactivation;
    }

    public void OnDeactivation()
    {
        if (onDeactivation!=null)
        {
            this.onDeactivation();
        }
    }
    public void OnActivation()
    {
        if (onActivation != null)
        {
            this.onActivation();
        }
    }
}

