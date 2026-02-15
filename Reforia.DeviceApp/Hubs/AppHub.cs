using Microsoft.AspNetCore.SignalR;
using Reforia.Rpc.Contracts;
using Reforia.Rpc.Core;

namespace ReforiaBackend.Hubs;

public class AppHub : Hub
{
    private readonly WebDispatcher _dispatcher;

    public AppHub(WebDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task JoinIrcConnection(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            throw new HubException("connectionId is required.");

        await Groups.AddToGroupAsync(Context.ConnectionId, connectionId, Context.ConnectionAborted);
    }

    public async Task LeaveIrcConnection(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionId, Context.ConnectionAborted);
    }

    public async Task<WebResponse> Call(WebRequest request) => await _dispatcher.Dispatch(request);
}