namespace Bee.Plugin.UmlGenerate.Models;


public enum UmlGenerateMode
{
    None = 0,
    /// <summary>
    /// 从 puml 文件生成
    /// </summary>
    FromPumlFile,
    /// <summary>
    /// 从 csharp 代码生成
    /// </summary>
    CSharpCode,    
}