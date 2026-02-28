namespace Reforia.Core.Common;

public enum EErrorCode
{
    None = 0,
    Unknown = 1,
    
    #region Irc (1000-1999)
    
    CannotGetIrcCredentials = 1000,
    CannotConnectToIrc = 1001,
    UserNotAllowedToConnect = 1002,

    #endregion    
    
    #region Communication (2000-2999)
    
    
    CannotDeserializeRequest = 2000,
    ErrorDuringFunctionExecute = 2001,
    FunctionNotFound = 2002,
    CannotGetApiData = 2003,

    #endregion

    #region Tournement (3000-3999)
    
    InvalidTourneyClientPath = 3000,

    #endregion
}