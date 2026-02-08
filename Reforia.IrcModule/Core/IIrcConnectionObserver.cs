namespace Reforia.IrcModule.Core;

public interface IIrcConnectionObserver
{
    Task AttachAsync(IrcConnection connection, CancellationToken ct = default);
}