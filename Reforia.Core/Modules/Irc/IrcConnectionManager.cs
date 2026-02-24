using System.Collections.Concurrent;
using Serilog;

namespace Reforia.Core.Modules.Irc;

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
            await conn.StartAsync(host, port, nick, password);
            Log.Information("IRC connection {ConnectionId} connected", id);
        }
        catch (Exception ex)
        {
            await conn.DisposeAsync();
            _connections.TryRemove(id, out _);
            Log.Error(ex, "IRC connection {ConnectionId} failed to connect", id);
            throw;
        }

        conn.Disposed += OnConnectionDisposed;
        return conn;
    }
    
    public bool TryGetByUser(string user, out IrcConnection connection)
    {
        foreach (var conn in _connections.Values)
        {
            if (conn.Nick.Equals(user, StringComparison.OrdinalIgnoreCase))
            {
                connection = conn;
                return true;
            }
        }

        connection = null!;
        Log.Debug("IRC connection for user {User} not found", user);
        return false;
    }

    public bool TryGet(string id, out IrcConnection connection)
    {
        var found = _connections.TryGetValue(id, out connection!);
        
        if (!found)
            Log.Debug("IRC connection {ConnectionId} not found", id);
        
        return found;
    }
    
    public async Task<bool> TryRemove(string id)
    {
        
        if (TryGet(id, out var connection))
        {
            await connection.DisposeAsync();
        }
        
        var removed = _connections.TryRemove(id, out _);
        
        if (removed)
            Log.Information("IRC connection {ConnectionId} removed from manager", id);
        else
            Log.Debug("Attempted to remove IRC connection {ConnectionId} but it was not found", id);
        
        return removed;
    }
    
    private void OnConnectionDisposed(object? sender, string id)
    {
        if (_connections.TryRemove(id, out _))
            Log.Information("IRC connection {ConnectionId} removed from manager", id);
    }
}