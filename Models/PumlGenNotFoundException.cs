
namespace Bee.Plugin.UmlGenerate.Models;

public class PumlGenNotFoundException : ApplicationException
{
    public PumlGenNotFoundException() : base("Puml Generator Not Found!") { }

    public PumlGenNotFoundException(string message)
        : base(message) { }

    public PumlGenNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}