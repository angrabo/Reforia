# RPC function system (Reforia.Rpc)

## Transport contract

RPC is exposed through SignalR:

- Hub: `/hub`
- Method: `Call(WebRequest) -> WebResponse`

`WebRequest.Body` is a **JSON string** representing the function-specific request model.

## Contracts

### WebRequest
```json
{ 
  "functionName": "exampleFunction",
  "requestId": "client-correlation-id",
  "body": {
    "someField":
    "value"
  }
}
```

Fields:
- `functionName`: selects which function to execute
- `requestId`: returned back unchanged in `WebResponse`
- `body`: JSON payload encoded as string (function input model)

### WebResponse

Success:
```json
{ 
  "requestId": "client-correlation-id",
  "statusCode": 200,
  "body": { 
    "resultField": "value"
  },
  "errors": []
}
```
Bad request (input issues):
```json
{ "requestId": "client-correlation-id",
  "statusCode": 403,
  "body": null,
  "errors": [
    "Validation error message"
  ]
}
```
Server error:
```json
{ "requestId": "client-correlation-id",
  "statusCode": 500,
  "body": null,
  "errors": [
    "Unhandled exception summary"
  ]
}
```
## Function model

Functions are implemented as typed classes:

- Base: `WebFunction<TRequest, TResponse>`
- Override: `Handle(TRequest request, IServiceProvider provider)`

Dispatcher responsibilities (conceptual):
- map `functionName` -> function type
- deserialize `Body` JSON string into `TRequest`
- create function instance (DI)
- run `Handle(...)`
- wrap output into `WebResponse`

## Naming conventions

A function name should be stable and unique in the registry.

# Adding a new RPC function

This describes the required structure for creating a new typed function in a module using `Reforia.Rpc`.

## 1) Create request model (DTO)

- Must be JSON-serializable.
- Use required markers if your serialization settings enforce them.

Example JSON that the client will send (encoded into `WebRequest.body`):

## 2) Create response model (DTO)
Example JSON returned in `WebResponse.body`:
```json
{ 
  "displayName": "ExampleUser",
  "roles": [
    "Admin",
    "User"
  ]
}
```
## 3) Implement the function

Function class shape:
- Inherit `WebFunction<TRequest, TResponse>`
- Implement `Handle(request, provider)`
- Resolve module services using `provider.GetRequiredService<T>()`

Execution is controlled by the dispatcher; the function only implements logic.

## 4) Register function + services in the module

Module registration pattern:
- Add required services to DI (typically `Scoped`)
- Register functions via assembly scan (`AddFunctionsFromAssembly(...)`)

## 5) Ensure module is added in the host

Host (`Reforia.Backend`) must call the module extension in the RPC builder chain, e.g.:

- `AddRpc().AddYourModule()`

## 6) Client call (wire format)

Full `Call` request (wire format) example:
```json
{
  "functionName": "YourModule.GetUserProfile",
  "requestId": "GUID",
  "body": {
    "userId":42
  }
}
```

Expected `Call` response example:
```json 
{ 
  "requestId": "req-1000",
  "statusCode": 200,
  "body": {
    "displayName": "ExampleUser",
    "roles": [
      "Admin", "User"
    ] 
  },
  "errors": []
}
```