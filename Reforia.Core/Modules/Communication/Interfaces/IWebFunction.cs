using Reforia.Core.Modules.Communication.Contracts;

namespace Reforia.Core.Modules.Communication.Interfaces;

public interface IWebFunction
{
    string Name { get; }
    
    Task<WebResponse> Execute(string jsonBody, IServiceProvider provider);
}