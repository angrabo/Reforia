using Microsoft.AspNetCore.SignalR;
using Reforia.IrcModule.Core;
using ReforiaBackend.Hubs;

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
        connection.MessageReceived += OnMessage;
        return Task.CompletedTask;
    }

    private void OnMessage(object? sender, IrcMessageEventArgs e)
    {
        _ = _hub.Clients
            .Group(e.ConnectionId)
                .SendAsync("ircMessage", e.RawMessage);
    }
}