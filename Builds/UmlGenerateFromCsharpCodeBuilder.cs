
using Bee.Plugin.UmlGenerate.Models;

namespace Bee.Plugin.UmlGenerate.Builds;

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
    /// 转换为命令行参数
    /// </summary>
    /// <param name="inputPath">输入路径</param>
    /// <param name="outputPath">输出文件名</param>
    /// <returns></returns>
    public List<string> BuildCommandLine(string inputPath, string outputPath)
    {
        var argsList = new List<string>();
        var options = new Dictionary<Func<bool>, string>
        {
            { () => true, inputPath },
            { () => true, outputPath },
            { () => true, "-dir" },
            { () => _args.IsPublic, "-public" },
            // 与 -public 参数指定一个即可
            //{ () => true, "-ignore Private,Protected" },
            { () => true, "-createAssociation" },
            { () => true, "-allInOne" },
            { () => _args.ExcludePaths != null, "-excludePaths" + string.Join(",", _args.ExcludePaths ?? []) },
            // { () => _args.ExcludeUmlBeginEndTags, "-excludeUmlBeginEndTags" },
        };

        foreach (var option in options)
        {
            if (option.Key())
            {
                argsList.AddRange(option.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
        }
        return argsList;
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