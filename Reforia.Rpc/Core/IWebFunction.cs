namespace ReforiaBackend.Rpc.Core;

public interface IWebFunction
{
    string Name { get; }
    
    Task<WebResponse> Execute(string jsonBody, IServiceProvider provider);
}