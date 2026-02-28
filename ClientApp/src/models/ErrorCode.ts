export enum ErrorCode {
    None = 0,
    Unknown = 1,


    CannotGetIrcCredentials = 1000,
    CannotConnectToIrc = 1001,
    UserNotAllowedToConnect = 1002,


    CannotDeserializeRequest = 2000,
    ErrorDuringFunctionExecute = 2001,
    FunctionNotFound = 2002,
    CannotGetApiData = 2003,


    Temp = 3000,
}