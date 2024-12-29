
namespace Bee.Plugin.UmlGenerate;

public class UmlGenerateConsts
{
    public const string PluginName = "UmlGenerate";
    /// <summary>
    /// 可处理的输入文件后缀
    /// </summary>
    public static string[] AvailableInputExtensions => ["puml"];
    /// <summary>
    /// 输出格式。["pdf", "html", "xmi", "scxml", "latex", "latex:nopreamble"] 等格式，需要其它版本的 jar 包
    /// </summary>
    public static string[] AvailableOutputFormats => ["png", "svg", "eps", "eps:text", "vdx", "txt", "utxt", "braille"];
    /// <summary>
    /// 可用主题
    /// </summary>
    public static string[] AvailableThemes =>
    [
        "amiga",
        "aws-orange",
        "black-knight",
        "bluegray",
        "blueprint",
        "carbon-gray",
        "cerulean-outline",
        "cerulean",
        "cloudscape-design",
        "crt-amber",
        "crt-green",
        "cyborg-outline",
        "cyborg",
        "hacker",
        "lightgray",
        "mars",
        "materia-outline",
        "materia",
        "metal",
        "mimeograph",
        "minty",
        "mono",
        "_none_",
        "plain",
        "reddress-darkblue",
        "reddress-darkgreen",
        "reddress-darkorange",
        "reddress-darkred",
        "reddress-lightblue",
        "reddress-lightgreen",
        "reddress-lightorange",
        "reddress-lightred",
        "sandstone",
        "silver",
        "sketchy-outline",
        "sketchy",
        "spacelab-white",
        "spacelab",
        "sunlust",
        "superhero-outline",
        "superhero",
        "toy",
        "united",
        "vibrant"
    ];
}