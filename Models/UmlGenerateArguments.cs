using Bee.Base.Models.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

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
    /// 缩放级别
    /// </summary>
    public int Scale { get; set; } = 5;


    /// <summary>
    /// 转换为命令行参数
    /// </summary>
    /// <param name="inputFileName">输入文件名</param>
    /// <param name="outputFileName">输出文件名</param>
    /// <returns></returns>
    public List<string> ToCommandLine(string inputFileName, string outputFileName)
    {
        var argsList = new List<string>();
        var options = new Dictionary<Func<bool>, string>
        {
            { () => true, inputFileName },
            { () => true, outputFileName },
            { () => !string.IsNullOrWhiteSpace(OutputFormat), "-s" },
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