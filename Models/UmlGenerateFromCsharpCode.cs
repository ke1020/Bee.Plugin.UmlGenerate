namespace Bee.Plugin.UmlGenerate.Models;

public class UmlGenerateFromCsharpCode
{
    /// <summary>
    /// -public: (可选) 如果指定，只输出公共可见性成员。
    /// </summary>
    /// <remarks>从源码生成有效</remarks>
    public bool IsPublic { get; set; } = false;
    /// <summary>
    /// 排除目录
    /// </summary>
    /// <remarks>从源码生成有效</remarks>
    public string[]? ExcludePaths { get; set; }
    /// <summary>
    /// 排除UML开始和结束标签
    /// </summary>
    public bool ExcludeUmlBeginEndTags { get; set; } = false;
}