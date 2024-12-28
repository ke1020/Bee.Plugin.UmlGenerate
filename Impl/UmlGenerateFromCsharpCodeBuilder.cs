
using Bee.Plugin.UmlGenerate.Models;

namespace Bee.Plugin.UmlGenerate.Tasks;

public class UmlGenerateFromCsharpCodeBuilder()
{
    private readonly UmlGenerateFromCsharpCode _args = new();

    /// <summary>
    /// 只输出公共可见性成员
    /// </summary>
    /// <returns></returns>
    public UmlGenerateFromCsharpCodeBuilder SetPublic()
    {
        _args.IsPublic = true;
        return this;
    }

    /// <summary>
    /// 排除指定路径
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    public UmlGenerateFromCsharpCodeBuilder SetExcludePaths(string[] paths)
    {
        _args.ExcludePaths = paths;
        return this;
    }

    /// <summary>
    /// 排除 UML 开始和结束标签
    /// </summary>
    /// <returns></returns>
    public UmlGenerateFromCsharpCodeBuilder SetExcludeUmlBeginEndTags()
    {
        _args.ExcludeUmlBeginEndTags = true;
        return this;
    }

    /// <summary>
    /// 构建参数对象
    /// </summary>
    /// <returns></returns>
    public UmlGenerateFromCsharpCode Build()
    {
        return _args;
    }
}