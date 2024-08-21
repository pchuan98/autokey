using CommunityToolkit.Mvvm.ComponentModel;
using InputSimulatorStandard;
using InputSimulatorStandard.Native;
using Serilog;

namespace AutoKey.Core.Models;

/// <summary>
/// 
/// </summary>
public partial class TextInput : Input
{
    public static int I = 0;
    public static int J = 0;

    // HACK: 这里会有个输出缓冲的问题，但是暂时应该解决不了，该问题出现在一些特殊软件中
    private static readonly InputSimulator _simulator = new();

    /// <summary>
    /// 原始输出text，可被format
    /// </summary>
    public string Text => Format
        ?.Replace("{i}", I++.ToString())
        ?.Replace("{j}", J++.ToString())
        ?.Replace("{time}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")) ?? "";

    /// <summary>
    /// 格式化字符
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor((nameof(Text)))]
    private string? _format;

    public override void Excute()
    {
        try
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(Before));

            var text = Text; // note: must use text instead of Text, otherwise I,J auto-increment error will occur.

            var ts = text.Split("\\n");

            if (ts.Length <= 1)
                _simulator.Keyboard.TextEntry(text);
            else
            {
                _simulator.Keyboard.TextEntry(ts[0]);

                for (var i = 1; i < ts.Length; i++)
                {
                    _simulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);

                    if (string.IsNullOrEmpty(ts[i])) continue;
                    _simulator.Keyboard.TextEntry(ts[i]);
                }
            }

            Log.Information($"Text input: {text}");
            Thread.Sleep(TimeSpan.FromMilliseconds(After));
        }
        catch (Exception e)
        {
            Log.Error(e, "TextInput Error :");
        }
    }
}
