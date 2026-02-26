using System.Collections.Concurrent;

namespace Reforia.IrcModule.Core;

public class IrcConnectionManager
{
    private readonly ConcurrentDictionary<string, IrcConnection> _connections = new();

    public async Task<IrcConnection> CreateAsync(string id, string host, int port, string nick, string password = "")
    {
        var conn = new IrcConnection(id);

        if (!_connections.TryAdd(id, conn))
            throw new Exception("Exists");

        await conn.ConnectAsync(host, port, nick, password);

        return conn;
    }

    public bool TryGet(string id, out IrcConnection connection)
        => _connections.TryGetValue(id, out connection!);
}