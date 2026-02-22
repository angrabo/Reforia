using Reforia.Core.Common;

namespace Reforia.Core.Modules.Communication.Exceptions;

public class CommunicationException : Exception
{
    public EErrorCode EErrorCode { get; }

    public CommunicationException(EErrorCode eErrorCode, string? message = null, Exception? innerException = null) 
        : base(message ?? $"Error occurred with code: {eErrorCode}", innerException)
    {
        EErrorCode = eErrorCode;
    }
}