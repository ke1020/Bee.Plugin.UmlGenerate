

using Bee.Base.Abstractions.Localization;
using Bee.Base.Models;

using Microsoft.Extensions.Options;

namespace Bee.Plugin.UmlGenerate;

/// <summary>
/// 本地化资源贡献器
/// </summary>
/// <param name="appSettings"></param>
public class UmlGenerateLocalizationResourceContributor(IOptions<AppSettings> appSettings) :
    LocalizationResourceContributorBase(appSettings, UmlGenerateConsts.PluginName)
{
}