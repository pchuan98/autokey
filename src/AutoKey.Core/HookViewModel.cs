using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;
using AutoKey.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Gma.System.MouseKeyHook;
using Serilog;
using WindowsInput.Events;

namespace AutoKey.Core;

public partial class HookViewModel : ObservableObject
{
    [ObservableProperty]
    private int _x = 0;

    [ObservableProperty]
    private int _y = 0;

    [ObservableProperty]
    private MouseButtons _button = MouseButtons.None;

    [ObservableProperty]
    private bool _isAlt = false;

    [ObservableProperty]
    private bool _isShift = false;

    [ObservableProperty]
    private bool _isControl = false;

    [ObservableProperty]
    private bool _isWin = false;

    [ObservableProperty]
    private HashSet<Keys> _keyCodes = [];

    [ObservableProperty]
    private string _keyString = "(none)";

    [ObservableProperty]
    private bool _handle = true;

    partial void OnKeyStringChanged(string value)
        => KeyString = string.IsNullOrEmpty(value) ? "(none)" : value;

    private readonly IKeyboardMouseEvents _hook;

    private readonly HashSet<Keys> _alt = [Keys.LMenu, Keys.RMenu, Keys.Alt];
    private readonly HashSet<Keys> _control = [Keys.LControlKey, Keys.RControlKey, Keys.Control, Keys.ControlKey];
    private readonly HashSet<Keys> _win = [Keys.LWin, Keys.LWin];
    private readonly HashSet<Keys> _shift = [Keys.LShiftKey, Keys.RShiftKey, Keys.Shift, Keys.ShiftKey];
    private readonly HashSet<Keys> _special = [
        Keys.LMenu, Keys.RMenu, Keys.Alt,
        Keys.LControlKey, Keys.RControlKey, Keys.Control, Keys.ControlKey,
        Keys.LWin, Keys.LWin,
        Keys.LShiftKey, Keys.RShiftKey, Keys.Shift, Keys.ShiftKey];

    public HookViewModel()
    {
        _hook = Hook.GlobalEvents();

        _hook.MouseMove += ((_, args) =>
        {
            X = args.X;
            Y = args.Y;

            Button = args.Button;
        });

        _hook.KeyDown += ((_, args) =>
        {
            KeyCodes.Add(args.KeyCode);

            IsAlt = KeyCodes.Overlaps(_alt);
            IsControl = KeyCodes.Overlaps(_control);
            IsShift = KeyCodes.Overlaps(_shift);
            IsWin = KeyCodes.Overlaps(_win);

            // ctrl + l  ->  left mouse click
            // ctrl + r  ->  right mouse click
            // ctrl + m  ->  middle mouse click
            // ctrl + d  ->  double mouse click
            // ctrl + t  ->  input text
            // ctrl + p  ->  pause

            if (IsControl
                && !IsAlt
                && !IsShift
                && !IsWin)
            {
                var key = args.KeyCode;

                switch (key)
                {
                    case Keys.L:
                        WeakReferenceMessenger.Default.Send<MouseInput>(new()
                        {
                            X = X,
                            Y = Y,
                            Button = MouseButtons.Left
                        });
                        break;
                    case Keys.R:
                        WeakReferenceMessenger.Default.Send<MouseInput>(new()
                        {
                            X = X,
                            Y = Y,
                            Button = MouseButtons.Right
                        });
                        break;
                    case Keys.M:
                        WeakReferenceMessenger.Default.Send<MouseInput>(new()
                        {
                            X = X,
                            Y = Y,
                            Button = MouseButtons.Middle
                        });
                        break;
                    case Keys.D:
                        WeakReferenceMessenger.Default.Send<MouseInput>(new()
                        {
                            X = X,
                            Y = Y,
                            Button = MouseButtons.Left,
                            After = 1000,
                            Before = 10,
                            Duration = 10
                        });
                        WeakReferenceMessenger.Default.Send<MouseInput>(new()
                        {
                            X = X,
                            Y = Y,
                            Button = MouseButtons.Left,
                            After = 0,
                            Before = 1000,
                            Duration = 10
                        });
                        break;
                    case Keys.P:
                        WeakReferenceMessenger.Default.Send("run");
                        break;
                    case Keys.S:
                        WeakReferenceMessenger.Default.Send("stop");
                        break;
                    case Keys.T:
                        WeakReferenceMessenger.Default.Send<TextInput>(new());
                        break;
                    default:
                        break;
                }
            }

            KeyString = string.Join("+", KeyCodes.Except(_special));
            args.Handled = Handle;
        });

        _hook.KeyUp += ((_, args) =>
        {
            KeyCodes.Remove(args.KeyCode);
            KeyString = string.Join("+", KeyCodes.Except(_special));

            if (KeyCodes.Count != 0) return;

            IsAlt = false;
            IsControl = false;
            IsShift = false;
            IsWin = false;

            args.Handled = Handle;
        });

        AddBossKey();
    }

    /// <summary>

    /// </summary>
    void AddBossKey()
    {
        _hook.OnCombination(new Dictionary<Combination, Action>()
        {
            {
                Combination.FromString("Control+L"),
                () =>
                {

                }
            },
            {
                Combination.TriggeredBy(Keys.Control).With(Keys.R),
                () =>
                {
                    Log.Debug("Ctrl + R");
                    WeakReferenceMessenger.Default.Send<MouseInput>(new ()
                    {
                        X = X,
                        Y = Y,
                        Button = MouseButtons.Right
                    });
                }
            },
            {
                Combination.TriggeredBy(Keys.Control).With(Keys.M),
                () =>
                {
                    Log.Debug("Ctrl + M");

                    WeakReferenceMessenger.Default.Send<MouseInput>(new ()
                    {
                        X = X,
                        Y = Y,
                        Button = MouseButtons.Middle
                    });
                }
            },
            {
                Combination.TriggeredBy(Keys.Control).With(Keys.T),
                () =>
                {
                    Log.Debug("Ctrl + T");

                    WeakReferenceMessenger.Default.Send<TextInput>(new ());
                }
            }
        });
    }
}