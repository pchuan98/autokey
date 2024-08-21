using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoKey.Core.Models;

/// <summary>
/// 
/// </summary>
public abstract partial class Input : ObservableObject
{
    /// <summary>
    /// 输入事件前等待多久(ms)
    /// </summary>
    [ObservableProperty]
    private int _before = 1000;

    /// <summary>
    /// 输入事件后等待多久(ms)
    /// </summary>
    [ObservableProperty]
    private int _after = 1000;

    /// <summary>
    /// 输入时间的持续时间(ms)
    /// </summary>
    [ObservableProperty]
    private int _duration = 10;

    /// <summary>
    /// 执行输入事件
    /// </summary>
    public virtual void Excute() { }
}