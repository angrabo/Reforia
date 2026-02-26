using Microsoft.AspNetCore.SignalR;
using Reforia.IrcModule.Core;
using ReforiaBackend.Hubs;
using Serilog;

namespace ReforiaBackend.Utils;

public class IrcSignalRBridge : IIrcConnectionObserver
{
    private readonly IHubContext<AppHub> _hub;

    public IrcSignalRBridge(IHubContext<AppHub> hub)
    {
        _hub = hub;
    }

    public Task AttachAsync(IrcConnection connection, CancellationToken ct = default)
    {
        Log.Information("Attaching IRC connection {ConnectionId} to SignalR bridge", connection.Id);
        connection.MessageReceived += OnMessage;
        return Task.CompletedTask;
    }

    private async void OnMessage(object? sender, IrcMessageEventArgs e)
    {
        try
        {
            // TODD: filter and final format
            if (e.RawMessage.Contains("QUIT") || e.RawMessage.Contains("JOIN"))
                return;
            
            await _hub.Clients
                .Group(e.ConnectionId)
                .SendAsync("ircMessage", e.RawMessage);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to forward IRC message from {ConnectionId}", e.ConnectionId);
        }
    }
}