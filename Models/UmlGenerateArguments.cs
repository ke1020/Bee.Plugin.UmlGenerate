using Bee.Base.Models.Tasks;

namespace Bee.Plugin.UmlGenerate.Models;

public partial class UmlGenerateArguments : TaskArgumentBase
{
    /// <summary>
    /// 生成模式
    /// </summary>
    public UmlGenerateMode GenerateMode { get; set; } = UmlGenerateMode.FromPumlFile;
    /// <summary>
    /// 输出格式
    /// </summary>
    public string OutputFormat { get; set; } = "png";
    /// <summary>
    /// 是否启用暗黑模式
    /// </summary>
    public bool EnableDarkMode { get; set; } = false;
    /// <summary>
    /// 缩放模式
    /// </summary>
    public UmlScaleMode ScaleMode { get; set; } = UmlScaleMode.ByScale;
    /// <summary>
    /// 缩放值，根据 ScaleMode 确定。
    /// </summary>
    public int Scale { get; set; } = 3;
    /// <summary>
    /// 主题
    /// </summary>
    public string? Theme { get; set; } = "mars";
    /// <summary>
    /// 为图表指定配置文件
    /// </summary>
    public string? ConfigFile { get; set; }
}