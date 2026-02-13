using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Reforia.IrcModule.Core;

public class IrcConnectionManager
{
    private readonly ConcurrentDictionary<string, IrcConnection> _connections = new();
    private readonly ILogger<IrcConnection> _connectionLogger;
    private readonly ILogger<IrcConnectionManager> _logger;

    public IrcConnectionManager(ILogger<IrcConnection> connectionLogger, ILogger<IrcConnectionManager> logger)
    {
        _connectionLogger = connectionLogger;
        _logger = logger;
    }

    public async Task<IrcConnection> CreateAsync(string id, string host, int port, string nick, string password = "")
    {
        _logger.LogInformation("Creating IRC connection {ConnectionId} to {Host}:{Port} as {Nick}", id, host, port, nick);

        var conn = new IrcConnection(id, _connectionLogger);

        if (!_connections.TryAdd(id, conn))
        {
            _logger.LogWarning("IRC connection with id {ConnectionId} already exists", id);
            throw new InvalidOperationException($"IRC connection with id '{id}' already exists.");
        }

        try
        {
            await conn.ConnectAsync(host, port, nick, password);
            _logger.LogInformation("IRC connection {ConnectionId} connected", id);
        }
        catch (Exception ex)
        {
            _connections.TryRemove(id, out _);
            _logger.LogError(ex, "IRC connection {ConnectionId} failed to connect", id);
            throw;
        }

        return conn;
    }

    public bool TryGet(string id, out IrcConnection connection)
    {
        var found = _connections.TryGetValue(id, out connection!);
        if (!found)
            _logger.LogDebug("IRC connection {ConnectionId} not found", id);
        return found;
    }
}