using Microsoft.AspNetCore.SignalR;
using Reforia.Core.Modules.Irc;
using Reforia.Core.Modules.Irc.Dto;
using Reforia.Core.Modules.Irc.Interfaces;
using Reforia.Core.Modules.Tournament.Services;
using ReforiaBackend.Hubs;
using Serilog;

namespace ReforiaBackend.Utils;

public class IrcSignalRBridge : IIrcConnectionObserver
{
    private readonly IHubContext<AppHub> _hub;
    private readonly LobbyService _lobbyService;

    public IrcSignalRBridge(IHubContext<AppHub> hub, LobbyService lobbyService)
    {
        _hub = hub;
        _lobbyService = lobbyService;
    }

    public Task AttachAsync(IrcConnection connection, CancellationToken ct = default)
    {
        connection.MessageReceived += OnMessage;
        return Task.CompletedTask;
    }

    private async void OnMessage(object? sender, IrcMessageEventArgs e)
    {
        try
        {
            if (!e.RawMessage.Contains("PRIVMSG")) return;
            
            var parts = e.RawMessage.Split(' ', 4);
            var sourceFull = parts[0].TrimStart(':');
            var senderNick = sourceFull.Split('!')[0];
            var target = parts[2];
            var message = parts[3].TrimStart(':');
            
            var chatId = target.StartsWith("#") ? target : senderNick;

            await _hub.Clients.Group(e.ConnectionId).SendAsync("ircMessage", new IrcMessageDto
            {
                ConnectionId = e.ConnectionId,
                ChatId = chatId,
                Sender = senderNick,
                Message = message,
                Timestamp = DateTime.UtcNow
            });

            if (senderNick.Equals("BanchoBot", StringComparison.OrdinalIgnoreCase))
            {
                var updatedLobby = await _lobbyService.ProcessMessage(chatId, message);
                if (updatedLobby != null) 
                    await _hub.Clients.Group(e.ConnectionId).SendAsync("LobbyStateUpdated", updatedLobby);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to bridge IRC message");
        }
    }
}