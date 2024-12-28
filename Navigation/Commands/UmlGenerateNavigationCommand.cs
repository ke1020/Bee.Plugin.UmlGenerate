using Bee.Base.Abstractions.Navigation;
using Bee.Plugin.UmlGenerate.ViewModels;
using Bee.Plugin.UmlGenerate.Views;

namespace Bee.Plugin.UmlGenerate.Navigation.Commands;

/// <summary>
/// 导航命令
/// </summary>
/// <param name="key"></param>
/// <param name="vm"></param>
public class UmlGenerateNavigationCommand(IndexViewModel vm) : 
    NavigationCommandBase<IndexViewModel>(UmlGenerateConsts.PluginName, vm)
{
}