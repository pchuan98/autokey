using System.Collections.ObjectModel;
using Accessibility;
using AutoKey.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Gma.System.MouseKeyHook;
using Serilog;

namespace AutoKey.Core;

public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// 
    /// </summary>
    public ObservableCollection<Input> InputCollection { get; } = new();

    [ObservableProperty]
    private int _repeatCount = 1;

    [ObservableProperty]
    private Input? _focusInput;

    [ObservableProperty]
    private bool _isEnable = true;

    /// <summary>
    /// if true, the task will continue
    /// </summary>
    private bool _runFlag = false;

    [RelayCommand]
    void Clear() => InputCollection.Clear();

    [RelayCommand]
    void AddKey() => InputCollection.Add(new KeyInput());

    [RelayCommand]
    void AddMouse() => InputCollection.Add(new MouseInput());

    [RelayCommand]
    void AddText() => InputCollection.Add(new TextInput());

    [RelayCommand]
    void Run() => Task.Run(() =>
    {
        _runFlag = true;
        IsEnable = false;

        Log.Information(".............................................................  Start  .............................................................");
        for (var i = 0; i < RepeatCount; i++)
        {
            if (!_runFlag)
            {
                Log.Warning(".............................................................  Pause  .............................................................");
                break;
            }

            foreach (var input in InputCollection)
            {
                if (!_runFlag)
                {
                    Log.Warning(".............................................................  Pause  .............................................................");
                    break;
                }
                FocusInput = input;
                input.Excute();
            }
        }

        Log.Information(".............................................................  Ending  .............................................................");
        TextInput.I = 0;
        TextInput.J = 0;

        IsEnable = true;
    });

    public MainViewModel()
    {
        WeakReferenceMessenger.Default.Register<KeyInput>(this, ((recipient, message) =>
        {
            InputCollection.Add(message);
        }));

        WeakReferenceMessenger.Default.Register<MouseInput>(this, ((recipient, message) =>
        {
            InputCollection.Add(message);
        }));

        WeakReferenceMessenger.Default.Register<TextInput>(this, ((recipient, message) =>
        {
            InputCollection.Add(message);
        }));

        WeakReferenceMessenger.Default.Register<string>(this, ((recipient, message) =>
        {
            if (message.ToLower() == "run") Run();
            if (message.ToLower() == "stop") _runFlag = false;
        }));
    }
}
