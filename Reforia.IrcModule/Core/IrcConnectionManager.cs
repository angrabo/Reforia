using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Serilog;

namespace Reforia.IrcModule.Core;

public class IrcConnectionManager
{
    private readonly ConcurrentDictionary<string, IrcConnection> _connections = new();
    

    public async Task<IrcConnection> CreateAsync(string id, string host, int port, string nick, string password = "")
    {
        Log.Information("Creating IRC connection {ConnectionId} to {Host}:{Port} as {Nick}", id, host, port, nick);

        var conn = new IrcConnection(id);

        if (!_connections.TryAdd(id, conn))
        {
            Log.Warning("IRC connection with id {ConnectionId} already exists", id);
            throw new InvalidOperationException($"IRC connection with id '{id}' already exists.");
        }

        try
        {
            await conn.ConnectAsync(host, port, nick, password);
            Log.Information("IRC connection {ConnectionId} connected", id);
        }
        catch (Exception ex)
        {
            _connections.TryRemove(id, out _);
            Log.Error(ex, "IRC connection {ConnectionId} failed to connect", id);
            throw;
        }

        return conn;
    }

    public bool TryGet(string id, out IrcConnection connection)
    {
        var found = _connections.TryGetValue(id, out connection!);
        if (!found)
            Log.Debug("IRC connection {ConnectionId} not found", id);
        return found;
    }
}