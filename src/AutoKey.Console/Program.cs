using InputSimulatorStandard.Native;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Metrics;
using InputSimulatorStandard;
using Serilog;
using System.Windows.Forms;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var simulator = new InputSimulator();
Thread.Sleep(1000);

for (var index = 0; index < 10; index++)
{
    var text = $"{index} {DateTime.Now:F} \\n";

    var ts = text.Split("\\n");

    if (ts.Length <= 1)
        //simulator.Keyboard.TextEntry(text);
        //SendKeys.SendWait(text);
    {
        Clipboard.SetText(text);
        SendKeys.SendWait("^v");
    }

    else
    {
        simulator.Keyboard.TextEntry(ts[0]);

        for (var i = 1; i < ts.Length; i++)
        {
            //simulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            SendKeys.SendWait("{ENTER}");
            if (string.IsNullOrEmpty(ts[i])) continue;
            //simulator.Keyboard.TextEntry(ts[i]);
            //SendKeys.SendWait(ts[i]);
            Clipboard.SetText(ts[i]);
            SendKeys.SendWait("^v");

        }
    }

    Log.Information($"Text input: {text}");
    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
}
