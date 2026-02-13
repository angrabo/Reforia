using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace Reforia.IrcModule.Core;

public class IrcConnection : IAsyncDisposable
{
    public event EventHandler<IrcMessageEventArgs>? MessageReceived;

    private readonly TcpClient _client = new();
    private readonly ILogger<IrcConnection> _logger;
    private StreamReader _reader;
    private StreamWriter _writer;
    private CancellationTokenSource _cts = new();

    public string Id { get; }

    public IrcConnection(string id, ILogger<IrcConnection> logger)
    {
        Id = id;
        _logger = logger;
    }

    public async Task ConnectAsync(string host, int port, string nick, string password = "")
    {
        _logger.LogInformation("Connecting IRC client {ConnectionId} to {Host}:{Port} as {Nick}", Id, host, port, nick);
        await _client.ConnectAsync(host, port);

        var stream = _client.GetStream();

        _reader = new StreamReader(stream);
        _writer = new StreamWriter(stream)
        {
            NewLine = "\r\n",
            AutoFlush = true
        };

        if (!string.IsNullOrWhiteSpace(password))
        {
            _logger.LogDebug("Sending PASS for IRC connection {ConnectionId}", Id);
            await SendAsync($"PASS {password}");
        }

        await SendAsync($"NICK {nick}");
        await SendAsync($"USER {nick} 0 * :{nick}");
        await SendAsync($"PRIVMSG BanchoBot :!help");

        _ = Task.Run(ReadLoopAsync);
    }

    public Task SendAsync(string message)
        => _writer.WriteLineAsync(message);

    private async Task ReadLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            var line = await _reader.ReadLineAsync();
            if (line == null)
                break;

            _logger.LogTrace("IRC {ConnectionId} received: {Message}", Id, line);
            MessageReceived?.Invoke(this, new IrcMessageEventArgs
            {
                ConnectionId = Id,
                RawMessage = line
            });

            if (line.StartsWith("PING"))
            {
                _logger.LogDebug("IRC {ConnectionId} received PING", Id);
                await SendAsync(line.Replace("PING", "PONG"));
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Disposing IRC connection {ConnectionId}", Id);
        _cts.Cancel();
        _client.Close();
    }
}