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

    /// <summary>
    /// 转换为命令行参数
    /// </summary>
    /// <param name="inputPath">输入路径</param>
    /// <param name="outputPath">输出文件名</param>
    /// <returns></returns>
    public List<string> ToCommandLine(string inputPath, string outputPath)
    {
        var argsList = new List<string>();
        var options = new Dictionary<Func<bool>, string>
        {
            { () => true, inputPath },
            { () => true, outputPath },
            { () => true, "-dir" },
            { () => IsPublic, "-public" },
            // 与 -public 参数指定一个即可
            //{ () => true, "-ignore Private,Protected" },
            { () => true, "-createAssociation" },
            { () => true, "-allInOne" },
            { () => ExcludePaths != null, "-excludePaths" + string.Join(",", ExcludePaths ?? []) },
            { () => ExcludeUmlBeginEndTags, "-excludeUmlBeginEndTags" },
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
}