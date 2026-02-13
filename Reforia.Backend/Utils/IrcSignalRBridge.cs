using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Reforia.IrcModule.Core;
using ReforiaBackend.Hubs;

namespace ReforiaBackend.Utils;

public class IrcSignalRBridge : IIrcConnectionObserver
{
    private readonly IHubContext<AppHub> _hub;
    private readonly ILogger<IrcSignalRBridge> _logger;

    public IrcSignalRBridge(IHubContext<AppHub> hub, ILogger<IrcSignalRBridge> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public Task AttachAsync(IrcConnection connection, CancellationToken ct = default)
    {
        _logger.LogInformation("Attaching IRC connection {ConnectionId} to SignalR bridge", connection.Id);
        connection.MessageReceived += OnMessage;
        return Task.CompletedTask;
    }

    private async void OnMessage(object? sender, IrcMessageEventArgs e)
    {
        try
        {
            _logger.LogTrace("Forwarding IRC message from {ConnectionId}", e.ConnectionId);
            await _hub.Clients
                .Group(e.ConnectionId)
                .SendAsync("ircMessage", e.RawMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to forward IRC message from {ConnectionId}", e.ConnectionId);
        }
    }
}