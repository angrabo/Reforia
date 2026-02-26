using Microsoft.AspNetCore.SignalR;
using ReforiaBackend.Rpc.Core;

namespace ReforiaBackend.Hubs;

public class AppHub : Hub
{
    private readonly WebDispatcher _dispatcher;

    public AppHub(WebDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    public async Task<WebResponse> Call(WebRequest request) => await _dispatcher.Dispatch(request);
}