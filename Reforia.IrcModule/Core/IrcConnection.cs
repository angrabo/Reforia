using System.Net.Sockets;
using Serilog;

namespace Reforia.IrcModule.Core;

public class IrcConnection : IAsyncDisposable
{
    public event EventHandler<IrcMessageEventArgs>? MessageReceived;

    private readonly TcpClient               _client = new();
    private          StreamReader            _reader = null!;
    private          StreamWriter            _writer = null!;
    private          CancellationTokenSource _cts    = new();

    public string Id { get; }

    public IrcConnection(string id)
    {
        Id = id;
    }

    /// <summary>
    /// Create a connection to the Irc server with provided credentials
    /// </summary>
    /// <param name="host">server host</param>
    /// <param name="port">server port</param>
    /// <param name="nick">user name</param>
    /// <param name="password">user password</param>
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
            await SendAsync($"PASS {password}");

        await SendAsync($"NICK {nick}");
        await SendAsync($"USER {nick} 0 * :{nick}");

        _ = Task.Run(ReadLoopAsync);
    }

    /// <summary>
    /// Join channel
    /// </summary>
    /// <param name="channel">Channel name (without # char)</param>
    /// <returns>Is success</returns>
    public async Task<bool> JoinChannelAsync(string channel)
    {
        try
        {
            await SendAsync($"JOIN #{channel}");
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Leave channel
    /// </summary>
    /// <param name="channel">Channel name (without # char)</param>
    /// <returns>Is Success</returns>
    public async Task<bool> LeaveChannelAsync(string channel)
    {
        try
        {
            await SendAsync($"LEAVE #{channel}");
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Send a message on a channel
    /// </summary>
    /// <param name="message">Message content</param>
    /// <param name="channel">Channel name (without # char)</param>
    /// <returns>Is success</returns>
    public async Task<bool> SendChannelMessageAsync(string message, string channel)
    {
        try
        {
            await SendAsync($"PRIVMSG #{channel} :{message}");
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Send a message to a private user
    /// </summary>
    /// <param name="message">Message content</param>
    /// <param name="username">Username</param>
    /// <returns>Is success</returns>
    public async Task<bool> SendPrivateMessageAsync(string message, string username)
    {
        try
        {
            await SendAsync($"PRIVMSG {username} :{message}");
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    private Task SendAsync(string message)
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
                await SendAsync(line.Replace("PING", "PONG"));
        }
    }

    public async ValueTask DisposeAsync()
    {
        Log.Information("Disposing IRC connection {ConnectionId}", Id);
        _cts.Cancel();
        _client.Close();
    }
}