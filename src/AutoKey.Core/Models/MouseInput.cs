using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using InputSimulatorStandard;
using Serilog;
using WindowsInput.Events;

namespace AutoKey.Core.Models;

/// <summary>
/// 
/// </summary>
public partial class MouseInput : Input
{
    private static readonly InputSimulator _simulator = new();

    /// <summary>
    /// 
    /// </summary>
    public MouseEventArgs? Args => new(Button, 0, X, Y, 0);

    [ObservableProperty]
    private int _x;

    [ObservableProperty]
    private int _y;

    [ObservableProperty]
    private MouseButtons _button = MouseButtons.None;

    public override void Excute()
    {
        ArgumentNullException.ThrowIfNull(Args, nameof(Args));

        Thread.Sleep(TimeSpan.FromMilliseconds(Before));

        Log.Information("Mouse Input: {x},{y}", Args.X, Args.Y);

        WindowsInput.Simulate.Events()
            .Wait(TimeSpan.FromMilliseconds(Before))
            .MoveTo(Args.X, Args.Y).Wait(TimeSpan.FromMilliseconds(10))
            .Hold(ButtonCode.Left).Wait(TimeSpan.FromMilliseconds(Duration))
            .Release(ButtonCode.Left)
            .Wait(TimeSpan.FromMilliseconds(After))
            .Invoke(new InvokeOptions());

        //_simulator.Mouse
        //    .Sleep(TimeSpan.FromMilliseconds(Before))
        //    .MoveMouseTo(Args.X, Args.Y)
        //    .Sleep(TimeSpan.FromMilliseconds(10))
        //    .Down(Button)
        //    .Sleep(TimeSpan.FromMilliseconds(Duration))
        //    .Up(Button)
        //    .Sleep(TimeSpan.FromMilliseconds(After));
    }
}

static class MouseSimulatorExtension
{
    internal static IMouseSimulator Down(this IMouseSimulator mouse, MouseButtons button)
       => button switch
       {
           MouseButtons.None => mouse,
           MouseButtons.Left => mouse.LeftButtonDown(),
           MouseButtons.Right => mouse.RightButtonDown(),
           MouseButtons.Middle => mouse.MiddleButtonDown(),
           MouseButtons.XButton1 => mouse.XButtonDown(1),
           MouseButtons.XButton2 => mouse.XButtonDown(2),
           _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
       };

    internal static IMouseSimulator Up(this IMouseSimulator mouse, MouseButtons button)
       => button switch
       {
           MouseButtons.None => mouse,
           MouseButtons.Left => mouse.LeftButtonUp(),
           MouseButtons.Right => mouse.RightButtonUp(),
           MouseButtons.Middle => mouse.MiddleButtonUp(),
           MouseButtons.XButton1 => mouse.XButtonUp(1),
           MouseButtons.XButton2 => mouse.XButtonUp(2),
           _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
       };
}