using Avalonia.Controls;

using Bee.Plugin.UmlGenerate.Models;
using Bee.Plugin.UmlGenerate.ViewModels;

namespace Bee.Plugin.UmlGenerate.Views;

public partial class IndexView : UserControl
{
    public IndexView()
    {
        InitializeComponent();
    }

    private void UmlGenerateMode_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox)
        {
            return;
        }

        if (comboBox.DataContext is not IndexViewModel vm)
        {
            return;
        }

        if (comboBox.SelectedValue is UmlGenerateMode mode)
        {
            vm.OnUmlGenerateModeChanged(mode);
        }
    }
}