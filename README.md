# ReforiaBackend

ASP.NET Core backend exposing a **SignalR** hub (`/hub`) and a custom **RPC** layer (`Reforia.Rpc`) for invoking server-side functions with typed request/response models.

## Solution layout

- `Reforia.Backend`  
  ASP.NET Core host application (DI, SignalR hub, CORS, OpenAPI).
- `Reforia.Rpc`  
  Core RPC framework (contracts, dispatcher, function base types, registration).
- `Reforia.IrcModule`  
  Feature module registered into RPC (functions + services).
- `Reforia.TestModule`  
  Example module showing typed functions (`WebFunction<TRequest,TResponse>`).
- `Reforia.RpcTestTool` (WPF)  
  Local test client/UI.
- `WebScoketTestTool`  
  Local CLI/tooling for quick transport tests.
- `Docs/`  
  Technical documentation.

## Runtime endpoints

- **SignalR Hub**: `/hub`
    - `Call(WebRequest) -> WebResponse` (RPC entrypoint)
    - `JoinIrcConnection(connectionId)` / `LeaveIrcConnection(connectionId)` (SignalR groups)

## RPC contract

### Request (`WebRequest`)
`Call(...)` accepts:
```json
{ 
  "functionName": "exampleFunction",
  "requestId": "a-client-generated-id",
  "body": {
    "dummy": 123
  }
}
```

## Architecture (ASCII)

### Transport + RPC flow
+---------------------+ SignalR +----------------------+ | Client (Desktop/Web)| <--------------------> | ASP.NET Core Host | +----------+----------+ | Reforia.Backend | | +----------+-----------+ | hub.Invoke("Call", WebRequest) | v v +------+--------------------+ +------+--------------------+ | /hub (AppHub) | | WebDispatcher | | Call(WebRequest) |------------------->| Dispatch(WebRequest) | +---------------------------+ +------+--------------------+ | | resolve + execute v +------------+-------------+ | WebFunction<TReq,TRes> | | (module function) | +------------+-------------+ | | DI services v +------------+-------------+ | Module services (DI) | | e.g. ITestService | +--------------------------+

### Module registration
+---------------------+ AddRpc() / RpcBuilder +----------------------+ | Reforia.Backend |------------------------------>| Reforia.Rpc | | Program.cs | | registration + scan | +----------+----------+ +----------+-----------+ | | | .AddTourneyModule() / .AddIrcModule() | v v +----------+----------+ +----------+-----------+ | Reforia.TestModule | AddFunctionsFromAssembly() | Function registry | | services + functions|------------------------------>| (discovery + mapping) | +---------------------+ +----------------------+

## Documentation

- [Docs/01-Architecture.md](Docs/01-Architecture.md)
- [Docs/02-Project-Structure.md](Docs/02-Project-Structure.md)
- [Docs/03-RPC-Functions-System.md](Docs/03-RPC-Functions-System.md)
- [Docs/04-Adding-New-Module.md](Docs/04-Adding-New-Module.md)
- [Docs/05-Dev-Notes.md](Docs/05-Dev-Notes.md)