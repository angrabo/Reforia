using Microsoft.AspNetCore.SignalR;
using Reforia.Rpc.Contracts;
using Reforia.Rpc.Core;

namespace ReforiaBackend.Hubs;

public class AppHub : Hub
{
    private readonly WebDispatcher _dispatcher;
    private readonly ILogger<AppHub> _logger;

    public AppHub(WebDispatcher dispatcher, ILogger<AppHub> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task JoinIrcConnection(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
        {
            _logger.LogWarning("JoinIrcConnection called with empty connection id. Caller: {ConnectionId}", Context.ConnectionId);
            throw new HubException("connectionId is required.");
        }

        _logger.LogInformation("Client {ConnectionId} joining IRC group {IrcConnectionId}", Context.ConnectionId, connectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, connectionId, Context.ConnectionAborted);
    }

    public async Task LeaveIrcConnection(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
        {
            _logger.LogDebug("LeaveIrcConnection called with empty connection id. Caller: {ConnectionId}", Context.ConnectionId);
            return;
        }

        _logger.LogInformation("Client {ConnectionId} leaving IRC group {IrcConnectionId}", Context.ConnectionId, connectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionId, Context.ConnectionAborted);
    }

    public async Task<WebResponse> Call(WebRequest request)
    {
        _logger.LogDebug("Dispatching request {RequestId} to {FunctionName}", request.RequestId, request.FunctionName);
        return await _dispatcher.Dispatch(request);
    }
}