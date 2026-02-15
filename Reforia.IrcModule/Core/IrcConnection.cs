using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using Serilog;

namespace Reforia.IrcModule.Core;

public class IrcConnection : IAsyncDisposable
{
    public event EventHandler<IrcMessageEventArgs>? MessageReceived;

    private readonly TcpClient _client = new();
    private StreamReader _reader;
    private StreamWriter _writer;
    private CancellationTokenSource _cts = new();

    public string Id { get; }

    public IrcConnection(string id)
    {
        Id = id;
    }

    public async Task ConnectAsync(string host, int port, string nick, string password = "")
    {
        Log.Information("Connecting IRC client {ConnectionId} to {Host}:{Port} as {Nick}", Id, host, port, nick);
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
            Log.Debug("Sending PASS for IRC connection {ConnectionId}", Id);
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

            MessageReceived?.Invoke(this, new IrcMessageEventArgs
            {
                ConnectionId = Id,
                RawMessage = line
            });

            if (line.StartsWith("PING"))
            {
                await SendAsync(line.Replace("PING", "PONG"));
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        Log.Information("Disposing IRC connection {ConnectionId}", Id);
        _cts.Cancel();
        _client.Close();
    }
}