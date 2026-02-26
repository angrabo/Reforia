using Microsoft.Extensions.DependencyInjection;

namespace Reforia.Rpc.Core;

public class WebFunctionRegistry
{
    private readonly Dictionary<string, Type> _functions = new();

    public WebFunctionRegistry(IEnumerable<IWebFunction> functions)
    {
        foreach (var function in functions)
            _functions[function.Name] = function.GetType();
    }

    public IWebFunction Resolve(string name, IServiceProvider provider)
    {
        if (!_functions.TryGetValue(name, out var type))
            throw new Exception($"Function '{name}' not found");
        
        return (IWebFunction)provider.GetRequiredService(type);
    }
}