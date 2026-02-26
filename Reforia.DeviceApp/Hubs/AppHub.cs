using Microsoft.AspNetCore.SignalR;
using Reforia.Core.Modules.Communication.Contracts;
using Reforia.Core.Modules.Communication.Core;
using Serilog;

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
        {
            Log.Warning("JoinIrcConnection called with empty connection id. Caller: {ConnectionId}", Context.ConnectionId);
            throw new HubException("connectionId is required.");
        }

        Log.Information("Client {ConnectionId} joining IRC group {IrcConnectionId}", Context.ConnectionId, connectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, connectionId, Context.ConnectionAborted);
    }

    public async Task LeaveIrcConnection(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
        {
            Log.Debug("LeaveIrcConnection called with empty connection id. Caller: {ConnectionId}", Context.ConnectionId);
            return;
        }

        Log.Information("Client {ConnectionId} leaving IRC group {IrcConnectionId}", Context.ConnectionId, connectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionId, Context.ConnectionAborted);
    }

    public async Task<WebResponse> Call(WebRequest request)
    {
        Log.Debug("Dispatching request {RequestId} to {FunctionName}", request.RequestId, request.FunctionName);
        return await _dispatcher.Dispatch(request);
    }
}