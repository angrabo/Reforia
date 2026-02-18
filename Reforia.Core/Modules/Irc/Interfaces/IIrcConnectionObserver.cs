namespace Reforia.Core.Modules.Irc.Interfaces;

public interface IIrcConnectionObserver
{
    Task AttachAsync(IrcConnection connection, CancellationToken ct = default);
}