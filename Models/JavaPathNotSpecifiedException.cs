
namespace Bee.Plugin.UmlGenerate.Models;

public class JavaPathNotSpecifiedException : ApplicationException
{
    public JavaPathNotSpecifiedException() : base("Java Path Not Specified!") { }

    public JavaPathNotSpecifiedException(string message)
        : base(message) { }

    public JavaPathNotSpecifiedException(string message, Exception innerException)
        : base(message, innerException) { }
}