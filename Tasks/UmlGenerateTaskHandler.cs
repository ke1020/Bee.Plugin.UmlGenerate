using System.Text;

using Avalonia.Media.Imaging;
using Avalonia.Platform;

using Bee.Base;
using Bee.Base.Abstractions;
using Bee.Base.Abstractions.Tasks;
using Bee.Base.Models.Tasks;
using Bee.Plugin.UmlGenerate.Builds;
using Bee.Plugin.UmlGenerate.Models;

using CliWrap;

using Ke.Bee.Localization.Localizer.Abstractions;

using LanguageExt;
using LanguageExt.Common;

using Microsoft.Extensions.Logging;

namespace Bee.Plugin.UmlGenerate.Tasks;

public class UmlGenerateTaskHandler(UmlGenerateOptions umlGenerateOptions,
    ILocalizer localizer,
    ICoverHandler coverHandler,
    ILogger<UmlGenerateTaskHandler> logger) :
    TaskHandlerBase<UmlGenerateArguments>(coverHandler)
{
    private readonly UmlGenerateOptions _umlGenerateOptions = umlGenerateOptions;
    private readonly ILocalizer _l = localizer;
    private readonly ILogger<UmlGenerateTaskHandler> _logger = logger;

    /// <summary>
    /// 生成模式创建任务列表处理器映射字典
    /// </summary>
    private readonly Dictionary<UmlGenerateMode, Action<List<TaskItem>, List<string>>> _modeCreateTaskProcessors = new()
    {
        {
            // .puml 文件
            UmlGenerateMode.FromPumlFile, (tasks, inputPaths) =>
            {
                foreach(var path in inputPaths)
                {
                    if(!File.Exists(path))
                    {
                        continue;
                    }

                    var ext = Path.GetExtension(path);
                    if(!UmlGenerateConsts.AvailableInputExtensions.Any(x => string.Equals(x, ext[1..], StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    using Stream stream = AssetLoader.Open(ImageAssetConsts.Uml);
                    tasks.Add(new TaskItem
                    {
                        // 任务封面图片
                        Source = new Bitmap(stream),
                        // 任务名
                        Name = Path.GetFileName(path),

                        Input = path
                    });
                }
            }
        },
        {
            // C# 源码目录
            UmlGenerateMode.CSharpCode, (tasks, inputPaths) =>
            {

                foreach (var path in inputPaths)
                {
                    if (!Directory.Exists(path))
                    {
                        continue;
                    }

                    using Stream stream = AssetLoader.Open(ImageAssetConsts.Folder);
                    var name = Path.GetFileName(Path.GetDirectoryName(path));
                    tasks.Add(new TaskItem
                    {
                        // 任务封面图片
                        Source = new Bitmap(stream),
                        // 任务名
                        Name = name,

                        Input = path
                    });
                }
            }
        }
    };

    /// <summary>
    /// 从输入路径创建任务列表
    /// </summary>
    /// <param name="inputPaths"></param>
    /// <param name="inputExtensions"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public override async Task<List<TaskItem>> CreateTasksFromInputPathsAsync(List<string> inputPaths,
        IEnumerable<string>? inputExtensions = null,
        UmlGenerateArguments? arguments = null)
    {
        if (inputPaths == null || arguments == null)
        {
            return [];
        }

        var tasks = new List<TaskItem>();
        if (_modeCreateTaskProcessors.TryGetValue(arguments.GenerateMode, out var processor))
        {
            processor(tasks, inputPaths);
        }
        return await Task.FromResult(tasks);
    }

    /// <summary>
    /// 执行任务
    /// </summary>
    /// <param name="taskItem"></param>
    /// <param name="arguments"></param>
    /// <param name="progressCallback"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="JavaPathNotSpecifiedException"></exception>
    public override async Task<Fin<Unit>> ExecuteAsync(TaskItem taskItem,
        UmlGenerateArguments arguments,
        Action<double> progressCallback,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(_umlGenerateOptions.JavaPath))
        {
            return Fin<Unit>.Fail(new JavaPathNotSpecifiedException(nameof(_umlGenerateOptions.JavaPath)));
        }

        progressCallback(5);

        Fin<Unit> r;
        // 先从 C# 生成 .puml 文件
        if (arguments.GenerateMode == UmlGenerateMode.CSharpCode)
        {
            r = await GeneratePumlFileFromCSharpCodeAsync(taskItem, arguments, taskItem.Name!, cancellationToken);
            if (!r.IsSucc)
            {
                return r;
            }

            r = await GenerateFromPumlFileAsync(arguments,
                Path.Combine(arguments.OutputDirectory, taskItem.Name!, "include.puml"),
                cancellationToken)
                ;
        }
        else
        {
            r = await GenerateFromPumlFileAsync(arguments, taskItem.Input, cancellationToken);
        }

        r.IfSucc(r => progressCallback(100));
        return r;
    }

    /// <summary>
    /// 从 .puml 文件生成 UML
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="pathName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Fin<Unit>> GenerateFromPumlFileAsync(UmlGenerateArguments arguments,
        string pumlFile,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_umlGenerateOptions.JavaPath))
        {
            return Fin<Unit>.Fail(string.Format(_l["Errors.NotFound.File"], _umlGenerateOptions.JavaPath));
        }

        if (!File.Exists(_umlGenerateOptions.PlantumlJarPath))
        {
            return Fin<Unit>.Fail(string.Format(_l["Errors.NotFound.File"], _umlGenerateOptions.PlantumlJarPath));
        }

        // 如果指定了缩放选项或主题
        if (arguments.ScaleMode != UmlScaleMode.None || !string.IsNullOrWhiteSpace(arguments.Theme))
        {
            // 读取 puml 文件到列表中
            var lines = new List<string>(File.ReadLines(pumlFile));

            // 缩放设置
            var scaleValue = arguments.ScaleMode switch
            {
                UmlScaleMode.ByWidth => $"scale {arguments.Scale} width",
                UmlScaleMode.ByHeight => $"scale {arguments.Scale} height",
                _ => $"scale {arguments.Scale}"
            };

            // 是否包含 scale 设置
            bool hasScaleLine = false;
            // 主题
            bool hasThemeLine = false;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("scale ", StringComparison.OrdinalIgnoreCase))
                {
                    hasScaleLine = true;
                    lines[i] = scaleValue;
                }

                if (lines[i].StartsWith("!theme ", StringComparison.OrdinalIgnoreCase))
                {
                    hasThemeLine = true;
                    lines[i] = $"!theme {arguments.Theme}";
                }
            }

            if (!hasScaleLine)
            {
                lines.Insert(1, scaleValue);
            }
            if (!hasThemeLine)
            {
                lines.Insert(1, $"!theme {arguments.Theme}");
            }

            // 重新写回文件
            await File.WriteAllLinesAsync(pumlFile, lines);
        }

        // 参数
        var builder = new UmlGenerateArgumentsBuilder()
            //.SetScaleMode(arguments.ScaleMode)
            //.SetScale(arguments.Scale)
            .SetOutputFormat(arguments.OutputFormat)
            .EnableDarkMode(arguments.EnableDarkMode)
            ;

        // 构建命令行参数
        var args = builder.BuildCommandLine(pumlFile,
            arguments.OutputDirectory,
            _umlGenerateOptions.PlantumlJarPath,
            _umlGenerateOptions.UmlLimitSize)
            ;

        // Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(args));

        StringBuilder stdErrBuffer = new();
        //StringBuilder stdOutBuffer = new();
        // 执行命令
        var r = await Cli.Wrap(_umlGenerateOptions.JavaPath)
            .WithArguments(args)
            //.WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
            .ExecuteAsync(cancellationToken)
            ;

        if (!r.IsSuccess)
        {
            // 记录到日志
            _logger.LogError(stdErrBuffer.ToString());
            //_logger.Error(stdOutBuffer.ToString());
            return Fin<Unit>.Fail(_l["Bee.Plugin.UmlGenerate.Fail.GenerateUml"]);
        }

        return Fin<Unit>.Succ(Unit.Default);
    }

    /// <summary>
    /// 从 C# 源码生成 .puml 文件
    /// </summary>
    /// <param name="taskItem"></param>
    /// <param name="umlGenerateArguments"></param>
    /// <param name="pathName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="PumlGenNotFoundException"></exception>
    private async Task<Fin<Unit>> GeneratePumlFileFromCSharpCodeAsync(TaskItem taskItem,
        UmlGenerateArguments umlGenerateArguments,
        string pathName,
        CancellationToken cancellationToken = default)
    {
        var outputPath = Path.Combine(umlGenerateArguments.OutputDirectory, pathName);
        var pumlFile = Path.Combine(outputPath, "include.puml");
        if (File.Exists(pumlFile))
        {
            return Fin<Unit>.Fail(_l["Bee.Plugin.UmlGenerate.Fail.PumlFileExists"]);
        }

        if (!File.Exists(_umlGenerateOptions.PumlGenPath))
        {
            return Fin<Unit>.Fail(Error.New(new PumlGenNotFoundException()));
        }

        var args = new UmlGenerateFromCsharpCodeBuilder()
            .SetPublic()
            // 去除结尾的路径分隔符 Path.DirectorySeparatorChar
            .BuildCommandLine(taskItem.Input.TrimEnd(Path.DirectorySeparatorChar), outputPath)
            ;

        StringBuilder stdErrBuffer = new();

        // 调用 puml-gen 生成 .puml 文件
        var r = await Cli.Wrap(_umlGenerateOptions.PumlGenPath)
            .WithArguments(args)
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
            .ExecuteAsync(cancellationToken)
            ;

        if (!r.IsSuccess)
        {
            // 记录到日志
            _logger.LogError(stdErrBuffer.ToString());
            return Fin<Unit>.Empty;
        }

        return Fin<Unit>.Succ(Unit.Default);
    }
}