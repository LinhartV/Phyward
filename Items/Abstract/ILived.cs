using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Interface for anything that can have LivedHandler
/// </summary>
public interface ILived
{
    public LivedHandler LivedHandler { get; }
    public bool Death();
    public void UpdateHealthBar();
    public void AddHealthBar();
    public StandardHealthBar HealthBar { get; set; }

}