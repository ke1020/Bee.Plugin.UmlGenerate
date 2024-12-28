
using Bee.Base.Abstractions.Tasks;
using Bee.Base.ViewModels;
using Bee.Plugin.UmlGenerate.Models;

using Ke.Bee.Localization.Localizer.Abstractions;

namespace Bee.Plugin.UmlGenerate.ViewModels;

public partial class IndexViewModel : WorkspaceViewModel
{
    /// <summary>
    /// 任务列表控件视图模型
    /// </summary>
    public ITaskListViewModel<UmlGenerateArguments> TaskList { get; }
    /// <summary>
    /// 生成模式
    /// </summary>
    public IEnumerable<UmlGenerateMode> UmlGenerateModes => Enum.GetValues(typeof(UmlGenerateMode))
        .Cast<UmlGenerateMode>()
        .Where(e => (int)e > 0)
        ;
        
    public IndexViewModel(IServiceProvider serviceProvider, ILocalizer l, TaskListViewModel<UmlGenerateArguments> taskList) : base(serviceProvider, l)
    {
        IsPaneOpen = true;
        TaskList = taskList;
        TaskList.InitialArguments(UmlGenerateConsts.PluginName);
        TaskList.SetInputExtensions(UmlGenerateConsts.AvailableInputExtensions);
    }

    
    public void OnUmlGenerateModeChanged(UmlGenerateMode mode)
    {
        TaskList.SetInputExtensions(mode switch
        {
            UmlGenerateMode.FromPumlFile => UmlGenerateConsts.AvailableInputExtensions,
            _ => null
        });
    }
}