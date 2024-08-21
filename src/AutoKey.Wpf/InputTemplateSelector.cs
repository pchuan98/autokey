using System.Windows;
using System.Windows.Controls;
using AutoKey.Core.Models;

namespace AutoKey.Wpf;

public class InputTemplateSelector : DataTemplateSelector
{
    public DataTemplate? KeyInputTemplate { get; set; }

    public DataTemplate? MouseInputTemplate { get; set; }

    public DataTemplate? TextInputTemplate { get; set; }

    public override DataTemplate SelectTemplate(object? item, DependencyObject container)
        => item switch
        {
            KeyInput => KeyInputTemplate,
            MouseInput => MouseInputTemplate,
            TextInput => TextInputTemplate,
            _ => base.SelectTemplate(item, container) ?? throw new InvalidOperationException()
        } ?? new DataTemplate();
}