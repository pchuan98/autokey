using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Lift.UI.Controls;
using Serilog;

namespace AutoKey.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();

    public App()
    {
        //AllocConsole();
        //CreateConsole();      useless

      
    }

    private void CreateConsole()
    {
        var console = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c start",
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
        };
        System.Diagnostics.Process.Start(console);

        // 重定向Console的输出
        var writer = new StreamWriter(Console.OpenStandardOutput());
        writer.AutoFlush = true;
        Console.SetOut(writer);
    }
}