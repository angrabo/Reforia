using System.Collections.Concurrent;
using Reforia.Core.Utils;

namespace Reforia.Core.Modules.Irc;

public class IrcConnectionManager
{
    private readonly ConcurrentDictionary<string, IrcConnection> _connections = new();

    public async Task<IrcConnection> CreateAsync(string id, string host, int port, string nick, string password = "")
    {
        Logger.Info($"Creating IRC connection {id} to {host}:{port} as {nick}");

        var conn = new IrcConnection(id);

        if (!_connections.TryAdd(id, conn))
        {
            Logger.Warning($"IRC connection with id {id} already exists");
            throw new InvalidOperationException($"IRC connection with id '{id}' already exists.");
        }

        try
        {
            await conn.StartAsync(host, port, nick, password);
            Logger.Info($"IRC connection {id} connected");
        }
        catch (Exception ex)
        {
            await conn.DisposeAsync();
            _connections.TryRemove(id, out _);
            Logger.Error(ex, $"IRC connection {id} failed to connect");
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
        Logger.Debug($"IRC connection for user {user} not found");
        return false;
    }

    public bool TryGet(string id, out IrcConnection connection)
    {
        var found = _connections.TryGetValue(id, out connection!);
        
        if (!found)
            Logger.Debug($"IRC connection {id} not found");
        
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
            Logger.Info($"IRC connection {id} removed from manager");
        else
            Logger.Debug($"Attempted to remove IRC connection {id} but it was not found");
        
        return removed;
    }
    
    private void OnConnectionDisposed(object? sender, string id)
    {
        if (_connections.TryRemove(id, out _))
            Logger.Info($"IRC connection {id} removed from manager");
    }
}