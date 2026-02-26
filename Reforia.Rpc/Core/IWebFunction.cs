using Reforia.Rpc.Contracts;

namespace Reforia.Rpc.Core;

public interface IWebFunction
{
    string Name { get; }
    
    Task<WebResponse> Execute(string jsonBody, IServiceProvider provider);
}