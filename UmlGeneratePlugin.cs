﻿
using System.Text.Json;

using Bee.Base.Abstractions.Navigation;
using Bee.Base.Abstractions.Plugin;
using Bee.Base.Abstractions.Tasks;
using Bee.Base.ViewModels;
using Bee.Plugin.UmlGenerate.Models;
using Bee.Plugin.UmlGenerate.Navigation.Commands;
using Bee.Plugin.UmlGenerate.Tasks;
using Bee.Plugin.UmlGenerate.ViewModels;

using Ke.Bee.Localization.Providers.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Bee.Plugin.UmlGenerate;

public class UmlGeneratePlugin(IServiceProvider serviceProvider) : PluginBase(serviceProvider)
{
    public override string PluginName => UmlGenerateConsts.PluginName;

    public override void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IPlugin, UmlGeneratePlugin>();
        services.AddSingleton<ILocalizationResourceContributor, UmlGenerateLocalizationResourceContributor>();
        services.AddSingleton<INavigationCommand, UmlGenerateNavigationCommand>();

        // 视图模型
        services.AddTransient<IndexViewModel>();
        services.AddTransient<TaskListViewModel<UmlGenerateArguments>>();

        // 任务处理器
        services.AddTransient<ITaskHandler<UmlGenerateArguments>, UmlGenerateTaskHandler>();

        ConfigureUmlGenerate(services);
    }

    private void ConfigureUmlGenerate(IServiceCollection services)
    {
        // 插件根目录
        var pluginRootPath = Path.Combine(AppSettings.PluginPath, PluginName);
        // 配置文件
        var cfg = Path.Combine(pluginRootPath, "Configs", "uml.json");
        // 文档转换配置
        UmlGenerateOptions umlGenerateOptions;
        // 优先使用配置文件中指定的配置
        if (File.Exists(cfg))
        {
            umlGenerateOptions = JsonSerializer.Deserialize<UmlGenerateOptions>(File.ReadAllBytes(cfg))!;
        }
        else
        {
            umlGenerateOptions = new UmlGenerateOptions
            {
                PumlGenPath = Path.Combine(pluginRootPath, "gen/puml-gen.exe"),
                PlantumlJarPath = Path.Combine(pluginRootPath, "gen/plantuml.jar"),
                UmlLimitSize = 16384
            };
        }

        services.AddSingleton(umlGenerateOptions);
    }
}
