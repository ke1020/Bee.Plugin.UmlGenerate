namespace Bee.Plugin.UmlGenerate.Models;

/// <summary>
/// UML 缩放模式
/// </summary>
public enum UmlScaleMode
{
    None = 0,
    /// <summary>
    /// 按缩放级别
    /// </summary>
    ByScale,
    /// <summary>
    /// 按宽度，高度等比
    /// </summary>
    ByWidth,
    /// <summary>
    /// 按高度，宽度等比
    /// </summary>
    ByHeight
}