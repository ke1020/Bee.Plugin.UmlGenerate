

using System.Text;

using Avalonia.Media.Imaging;
using Avalonia.Platform;

using Bee.Base;
using Bee.Base.Abstractions.Tasks;
using Bee.Base.Models.Tasks;
using Bee.Plugin.UmlGenerate.Models;

using CliWrap;

using Serilog;

namespace Bee.Plugin.UmlGenerate.Tasks;

public class UmlGenerateTaskHandler(UmlGenerateOptions umlGenerateOptions) :
    ITaskHandler<UmlGenerateArguments>
{
    private readonly UmlGenerateOptions _umlGenerateOptions = umlGenerateOptions;

    /// <summary>
    /// 生成模式创建任务列表处理器映射字典
    /// </summary>
    private readonly Dictionary<UmlGenerateMode, Action<List<TaskItem>, List<string>>> _modeCreateTaskProcessors = new()
    {
        {
            // .puml 文件
            UmlGenerateMode.FromPumlFile, (tasks, inputPaths) =>
            {
                using Stream stream = AssetLoader.Open(AssetConsts.Uml);
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
                using Stream stream = AssetLoader.Open(AssetConsts.Folder);
                foreach (var path in inputPaths)
                {
                    if (!Directory.Exists(path))
                    {
                        continue;
                    }

                    var id = Guid.NewGuid().ToString();
                    tasks.Add(new TaskItem
                    {
                        // 任务封面图片
                        Source = new Bitmap(stream),
                        // 任务名
                        Name = Path.GetFileName(id),

                        Input = path
                    });
                }
            }
        }
    };

    private readonly Dictionary<UmlGenerateMode, Action> _modeExecuteProcessors = new()
    {
        {
            // .puml 文件
            UmlGenerateMode.FromPumlFile, () =>
            {

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
    public async Task<List<TaskItem>> CreateTasksFromInputPathsAsync(List<string> inputPaths,
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

    public async Task<bool> ExecuteAsync(TaskItem taskItem,
        UmlGenerateArguments? arguments,
        Action<double> progressCallback,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (arguments == null)
        {
            throw new ArgumentNullException(nameof(arguments));
        }

        if (!File.Exists(_umlGenerateOptions.JavaPath))
        {
            throw new JavaPathNotSpecifiedException(nameof(_umlGenerateOptions.JavaPath));
        }

        var pathName = Path.GetDirectoryName(taskItem.Input) ?? Guid.NewGuid().ToString();
        CommandResult? r;
        if (arguments.GenerateMode == UmlGenerateMode.CSharpCode)
        {
            r = await GeneratePumlFileFromCSharpCodeAsync(taskItem, arguments, pathName, cancellationToken);
            if (!(r?.IsSuccess == true))
            {
                return false;
            }
        }

        /*
        var argumentsObj = new UmlGenerateArgumentsBuilder()
            .Build()
            ;

        var outputFileName = Path.Combine(arguments.OutputDirectory, $"uml{Guid.NewGuid()}.{arguments.OutputFormat}");

        var argList = argumentsObj.ToCommandLine(taskItem.Input, outputFileName);

        var command = Cli.Wrap(_umlGenerateOptions.PumlGenPath).WithArguments(argList);

        var r = await command.ExecuteAsync(cancellationToken);
        if (!r.IsSuccess)
        {
            throw new CommandExecutionException(command, r.ExitCode, string.Empty);
        }

        command = Cli.Wrap(_umlGenerateOptions.JavaPath).WithArguments(
            [
                "-jar",
                $"-DPLANTUML_LIMIT_SIZE={_umlGenerateOptions.UmlLimitSize}",
                _umlGenerateOptions.PlantumlJarPath,
                $"-t{arguments.OutputFormat}",
                outputFileName
            ]);

        r = await command.ExecuteAsync(cancellationToken);
        return r.IsSuccess;
        */

        return true;

        /*
        // 输出目录
        var outputPath = Path.Combine(umlGenerateRequest.OutputPath, PluginName);
        // 命令参数
        //var arguments = new StringBuilder();
        // 命令参数构建
        arguments.Append($"{umlGenerateRequest.InputPath} {outputPath} -dir -ignore Private,Protected -createAssociation -allInOne");
        // 排除文件
        if (umlGenerateRequest.ExcludeFiles?.Length > 0)
        {
            arguments.Append($" -excludePaths {string.Join(",", umlGenerateRequest.ExcludeFiles)}");
        }

        // 调用 puml-gen 生成 .puml 文件
        var process = ProcessUtils.CreateProcess(umlGenerateRequest.Options.PumlGenPath, arguments.ToString());

        process.Start();
        process.WaitForExit();

        // 生成成功
        if (process.ExitCode == 0)
        {
            string outputType = umlGenerateRequest.OutputType.ToString()?.ToLower() ?? "png";

            arguments.Clear();

            var umlIncludeFile = Path.Combine(outputPath, "include.puml");
            var startTag = "@startuml";
            var umlIncludeString = File.ReadAllText(umlIncludeFile);
            // 修改输出文件大小
            if (umlIncludeString.IndexOf($"{startTag}\nscale ") == -1)
            {
                var sb = new StringBuilder(umlIncludeString);
                sb.Insert(umlIncludeString.IndexOf($"{startTag}\n") + startTag.Length + 1, $"\nscale {umlGenerateRequest.Scale}");
                File.WriteAllText(umlIncludeFile, sb.ToString());
            }

            arguments.Append($"-jar -DPLANTUML_LIMIT_SIZE={umlGenerateRequest.Options.UmlLimitSize} {umlGenerateRequest.Options.PlantumlJarPath} -t{outputType} {umlIncludeFile}");
            // 调用 java -jar 生成输出
            process = ProcessUtils.CreateProcess(umlGenerateRequest.Options.JavaPath, arguments.ToString());
            process.Start();
            process.WaitForExit();
        }
        return true;
        */
    }

    private async Task<CommandResult?> GeneratePumlFileFromCSharpCodeAsync(TaskItem taskItem, 
        UmlGenerateArguments umlGenerateArguments, 
        string pathName,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_umlGenerateOptions.PumlGenPath))
        {
            throw new PumlGenNotFoundException();
        }

        

        var cs = new UmlGenerateFromCsharpCodeBuilder()
            .Build()
            ;

        var outputPath = Path.Combine(umlGenerateArguments.OutputDirectory, pathName);
        var args = cs.ToCommandLine(taskItem.Input, outputPath);
        StringBuilder stdErrBuffer = new();
        var r = await Cli.Wrap(_umlGenerateOptions.PumlGenPath)
            .WithArguments(args)
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
            .ExecuteAsync(cancellationToken)
            ;

        if (!r.IsSuccess)
        {
            // 记录到日志
            Log.Error(stdErrBuffer.ToString());
        }

        return r;
    }
}