using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;

namespace AutoKey.Core.Models;

public partial class KeyInput : Input
{
    /// <summary>
    /// 
    /// </summary>
    public KeyEventArgs? Args { get; private set; }

    [ObservableProperty]
    private HashSet<Keys> _keyCodes = [];

    public override void Excute()
    {
        Log.Warning("Not support.");
    }
}