using Microsoft.AspNetCore.SignalR;
using Reforia.Core.Modules.Irc;
using Reforia.Core.Modules.Irc.Dto;
using Reforia.Core.Modules.Irc.Interfaces;
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

            Console.WriteLine(e.RawMessage);
            
            if (e.RawMessage.Contains("PRIVMSG"))
            {
                var parts = e.RawMessage.Split(' ', 4);
                var source = parts[0].TrimStart(':');
                var target = parts[2];
                var message = parts[3].TrimStart(':');

                var model = new IrcMessageDto()
                {
                    ConnectionId = e.ConnectionId,
                    ChatId = target.Contains('#') ? target : source.Split('!')[0],
                    Sender = source.Split('!')[0],
                    Message = message,
                    Timestamp = DateTime.UtcNow
                };
                await _hub.Clients
                    .Group(e.ConnectionId)
                    .SendAsync("ircMessage", model);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to forward IRC message from {ConnectionId}", e.ConnectionId);
        }
    }
}