namespace Bee.Plugin.UmlGenerate.Models;

public class UmlGenerateOptions
{
    public string? JavaPath { get; set; }
    public string? PumlGenPath { get; set; }
    public string? PlantumlJarPath { get; set; }
    public int UmlLimitSize { get; set; } = 16384;
}