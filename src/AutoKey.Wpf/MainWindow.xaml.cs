using System.IO;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoKey.Core;
using AutoKey.Core.Models;
using Serilog;

namespace AutoKey.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        Box.MouseDoubleClick += (_, _) => Box.Clear();

        Console.SetOut(new TextBoxWriter(Box));

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        DataContext = new MainViewModel();
        Status.DataContext = new HookViewModel();
    }

    private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = false;

        //TaskBox.SelectedItem = ((MainViewModel)DataContext).InputCollection[_index++ % 3];
    }

    private void RemoveTask(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
        {
            Log.Warning("The DataContext is not MainViewModel.");
            return;
        }

        if (TaskBox.SelectedItem is not Input input)
        {
            Log.Warning("The SelectedItem is null.");
            return;
        }

        try
        {
            vm.InputCollection.Remove(input);
            throw new Exception("test error");
        }
        catch (Exception exception)
        {
            Log.Error(exception, "RemoveTask Error:");
        }
    }
}


public class TextBoxWriter(TextBox textBox) : TextWriter
{
    public override void Write(char value)
        => textBox.Dispatcher.Invoke(() => textBox.AppendText(value.ToString()));

    public override void Write(string? value)
    {
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            textBox.Dispatcher.Invoke(() => textBox.AppendText(value));
            textBox.ScrollToEnd();
        });
    }

    public override Encoding Encoding => Encoding.UTF8;
}
