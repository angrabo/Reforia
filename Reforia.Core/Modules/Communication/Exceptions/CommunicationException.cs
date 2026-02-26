using Reforia.Core.Common;

namespace Reforia.Core.Modules.Communication.Exceptions;

public class CommunicationException : Exception
{
    public ErrorCode ErrorCode { get; }

    public CommunicationException(ErrorCode errorCode, string? message = null, Exception? innerException = null) 
        : base(message ?? $"Error occurred with code: {errorCode}", innerException)
    {
        ErrorCode = errorCode;
    }
}