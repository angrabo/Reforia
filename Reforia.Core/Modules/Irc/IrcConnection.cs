using System.Net.Sockets;
using Reforia.Core.Utils;

namespace Reforia.Core.Modules.Irc;

public class IrcConnection : IAsyncDisposable
{
    public event EventHandler<IrcMessageEventArgs>? MessageReceived;
    public event EventHandler<string>? Disposed;
    public string Nick     = "";

    private TcpClient    _client = new();
    private StreamReader _reader = null!;
    private StreamWriter _writer = null!;

    private readonly CancellationTokenSource _cts = new();

    private readonly HashSet<string> _joinedChannels = [];
    private readonly SemaphoreSlim   _sendLock       = new(1, 1);

    private string _host = "";
    private int    _port;
    private string _password = "";

    private DateTime _lastMessageReceived = DateTime.UtcNow;

    private static readonly TimeSpan WatchdogTimeout = TimeSpan.FromMinutes(1);

    public string Id { get; }

    public IrcConnection(string id)
    {
        Id = id;
    }

    public async Task StartAsync(string host, int port, string nick, string password = "")
    {
        _host = host;
        _port = port;
        Nick = nick;
        _password = password;

        var loginTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        EventHandler<IrcMessageEventArgs> authHandler = null!;
        authHandler = (_, e) =>
        {
            if (e.RawMessage.Contains(" 001 ")) 
                loginTcs.TrySetResult(true);
            else if (e.RawMessage.Contains(" 464 ")) 
                loginTcs.TrySetException(new UnauthorizedAccessException("Invalid Credentials"));

            if (loginTcs.Task.IsCompleted)
                MessageReceived -= authHandler;
        };

        MessageReceived += authHandler;

        _ = Task.Run(ConnectionLoopAsync);

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await using (cts.Token.Register(() => loginTcs.TrySetException(new TimeoutException("IRC Login timeout"))))
                await loginTcs.Task;
        }
        catch
        {
            MessageReceived -= authHandler;
            throw;
        }
    }

    public async Task<bool> JoinChannelAsync(string channel)
    {
        try
        {
            _joinedChannels.Add(channel);
            await SendAsync($"JOIN #{channel}");
            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Could not join channel {channel}");
            return false;
        }
    }

    public async Task<bool> LeaveChannelAsync(string channel)
    {
        try
        {
            _joinedChannels.Remove(channel);
            await SendAsync($"PART #{channel}");
            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Could not leave channel {channel}");
            return false;
        }
    }

    public async Task<bool> SendChannelMessageAsync(string message, string channel)
    {
        try
        {
            await SendAsync($"PRIVMSG #{channel} :{message}");
            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Could not send channel message on #{channel}");
            return false;
        }
    }

    public async Task<bool> SendPrivateMessageAsync(string message, string username)
    {
        try
        {
            await SendAsync($"PRIVMSG {username} :{message}");
            return true;
        }
        catch ( Exception e)
        {
            Logger.Error(e, $"Could not send private message to {username}");
            return false;
        }
    }

    private async Task ConnectionLoopAsync()
    {
        var delay = 2000;
        var retryCount = 0; 

        while (!_cts.IsCancellationRequested || retryCount < 10)
        {
            try
            {
                Logger.Info($"Connecting IRC {Id}");

                await ConnectInternalAsync();

                delay = 2000;

                await ReadLoopAsync();
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, $"IRC disconnected {Id}" );
            }

            if (_cts.IsCancellationRequested)
                break;

            Logger.Info($"Reconnect in {delay}ms");

            try
            {
                await Task.Delay(delay, _cts.Token);
            }
            catch
            {
                // ignored
            }

            delay = Math.Min(delay * 2, 30000);
            retryCount++;
        }

        await DisposeAsync();
    }

    private async Task ConnectInternalAsync()
    {
        _client.Dispose();
        _client = new TcpClient();

        await _client.ConnectAsync(_host, _port);

        var stream = _client.GetStream();

        _reader = new StreamReader(stream);
        _writer = new StreamWriter(stream)
        {
            NewLine = "\r\n",
            AutoFlush = true
        };

        if (!string.IsNullOrWhiteSpace(_password))
            await SendAsync($"PASS {_password}");

        await SendAsync($"NICK {Nick}");
        await SendAsync($"USER {Nick} 0 * :{Nick}");

        await RejoinChannels();

        _lastMessageReceived = DateTime.UtcNow;
        
        Logger.Info($"Connected to IRC {Id}");
    }

    private async Task RejoinChannels()
    {
        foreach (var channel in _joinedChannels)
            await SendAsync($"JOIN #{channel}");
    }

    private async Task ReadLoopAsync()
    {
        var watchdog = WatchdogLoopAsync();

        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var line = await _reader.ReadLineAsync();

                if (line == null)
                    throw new Exception("Connection closed");

                _lastMessageReceived = DateTime.UtcNow;

                MessageReceived?.Invoke(this, new IrcMessageEventArgs
                {
                    ConnectionId = Id,
                    RawMessage = line
                });

                if (!line.StartsWith("PING")) continue;

                var payload = line[5..];
                await SendAsync($"PONG {payload}");
            }
        }
        finally
        {
            try
            {
                await watchdog;
            }
            catch
            {
                // ignored
            }
        }
    }

    private async Task WatchdogLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), _cts.Token);

            if (DateTime.UtcNow - _lastMessageReceived > WatchdogTimeout)
            {
                Logger.Warning($"Watchdog timeout for {Id}");
                throw new Exception("Watchdog timeout");
            }
        }
    }


    private async Task SendAsync(string message)
    {
        await _sendLock.WaitAsync();

        try
        {
            await _writer.WriteLineAsync(message);
        }
        finally
        {
            _sendLock.Release();
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        Logger.Info($"Disposing IRC connection {Id}");

        await _cts.CancelAsync();

        try
        {
            await SendAsync("QUIT");
        }
        catch
        {
            // ignored
        }

        _client.Dispose();
        _sendLock.Dispose();
        
        Disposed?.Invoke(this, Id);
    }
}