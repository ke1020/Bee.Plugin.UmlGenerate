namespace Bee.Plugin.UmlGenerate.Models;

public class UmlGenerateArgumentsBuilder
{
    private readonly UmlGenerateArguments _args = new();

    /// <summary>
    /// 设置缩放模式
    /// </summary>
    /// <param name="scaleMode"></param>
    /// <returns></returns>
    public UmlGenerateArgumentsBuilder SetScaleMode(UmlScaleMode scaleMode)
    {
        _args.ScaleMode = scaleMode;
        return this;
    }

    /// <summary>
    /// 设置输出格式
    /// </summary>
    /// <param name="outputFormat"></param>
    /// <returns></returns>
    public UmlGenerateArgumentsBuilder SetOutputFormat(string outputFormat)
    {
        _args.OutputFormat = outputFormat;
        return this;
    }

    /// <summary>
    /// 启用暗黑模式
    /// </summary>
    /// <returns></returns>
    public UmlGenerateArgumentsBuilder EnableDarkMode(bool enableDarkMode = false)
    {
        _args.EnableDarkMode = enableDarkMode;
        return this;
    }

    /// <summary>
    /// 设置缩放值
    /// </summary>
    /// <param name="scale"></param>
    /// <returns></returns>
    public UmlGenerateArgumentsBuilder SetScale(int scale)
    {
        _args.Scale = scale;
        return this;
    }

    /// <summary>
    /// 转换为命令行参数
    /// </summary>
    /// <param name="inputFileName">输入文件名</param>
    /// <param name="outputPath">输出目录</param>
    /// <returns></returns>
    public List<string> BuildCommandLine(string inputFileName, string outputPath, string jarPath, int limitSize)
    {
        var args = new List<string>();
        var options = new Dictionary<Func<bool>, string>
        {
            { () => true, "-jar" },
            { () => true, jarPath },
            { () => _args.ConfigFile != null && _args.ConfigFile.EndsWith(".cfg", StringComparison.OrdinalIgnoreCase), $"-config {_args.ConfigFile}" },
            { () => _args.EnableDarkMode, "-darkmode" },
            { () => true, "-progress" },
            { () => true, $"-DPLANTUML_LIMIT_SIZE={limitSize}" },
            { () => !string.IsNullOrWhiteSpace(_args.OutputFormat), $"-t{_args.OutputFormat}" },
            { () => !string.IsNullOrWhiteSpace(outputPath), $"-o {outputPath}" },
            { () => true, inputFileName },
        };

        foreach (var option in options)
        {
            if (option.Key())
            {
                args.AddRange(option.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
        }
        return args;
    }

    /// <summary>
    /// 构建参数对象
    /// </summary>
    /// <returns></returns>
    public UmlGenerateArguments Build()
    {
        return _args;
    }
}