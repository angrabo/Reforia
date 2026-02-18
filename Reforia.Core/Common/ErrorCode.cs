namespace Reforia.Core.Common;

public enum ErrorCode
{
    None = 0,
    Unknown = 1,
    
    #region Irc (1000-1999)
    
    CannotGetIrcCredentials = 1000,

    #endregion    
    
    #region Communication (2000-2999)
    
    
    CannotDeserializeRequest = 2000,
    ErrorDuringFunctionExecute = 2001,
    FunctionNotFound = 2002,

    #endregion

    #region Tournement (3000-3999)
    
    Temp = 3000,

    #endregion
}