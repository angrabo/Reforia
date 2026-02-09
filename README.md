# ReforiaBackend

ASP.NET Core backend exposing a **SignalR** hub (`/hub`) and a custom **RPC** layer (`Reforia.Rpc`) for invoking server-side functions using typed request/response models.

---

## Solution Layout

### `Reforia.Backend`
ASP.NET Core host application responsible for:
- Dependency Injection
- SignalR hub
- CORS configuration
- OpenAPI / Swagger
- Module registration

### `Reforia.Rpc`
Core RPC framework providing:
- Contracts
- Dispatcher
- Function base types
- Function registration & discovery

### `Reforia.IrcModule`
Feature module registered into RPC:
- IRC-related functions
- IRC services

### `Reforia.TestModule`
Example module demonstrating:
- Typed RPC functions
- `WebFunction<TRequest, TResponse>` usage

### `Reforia.RpcTestTool` (WPF)
Local UI client used for manual RPC testing.

### `Docs/`
Technical documentation.

---

## Runtime Endpoints

### SignalR Hub
/hub

#### Methods

##### `Call(WebRequest) -> WebResponse`
Main RPC entrypoint.

##### `JoinIrcConnection(connectionId)`
Adds client to SignalR group.

##### `LeaveIrcConnection(connectionId)`
Removes client from SignalR group.

---

## RPC Contract

### Request (`WebRequest`)

```json
{
  "functionName": "exampleFunction",
  "requestId": "client-generated-id",
  "body": {
    "dummy": 123
  }
}
```
## Architecture
### Transport + RPC Flow

```
+------------------------+
| Client (Desktop / Web) |
+-----------+------------+
            |
            | SignalR
            v
+------------------------+
| ASP.NET Core Host      |
| Reforia.Backend        |
+-----------+------------+
            |
            | hub.Invoke("Call", WebRequest)
            v
+------------------------+
| AppHub (/hub)          |
| Transport Layer        |
+-----------+------------+
            |
            | forwards request
            v
+------------------------+
| WebDispatcher          |
| Dispatch(WebRequest)   |
+-----------+------------+
            |
            | resolve function
            | execute function
            v
+------------------------------+
| WebFunction<TReq, TRes>      |
| (Module Function)            |
+-----------+------------------+
            |
            | uses DI services
            v
+------------------------------+
| Module Services              |
| (e.g. ITestService, IRC etc) |
+------------------------------+
```
### Module Registration Flow
```
+----------------------+
| Reforia.Backend      |
| Program.cs           |
+----------+-----------+
           |
           | AddRpc()
           | RpcBuilder
           v
+----------------------+
| Reforia.Rpc          |
| Function Registry    |
| Discovery + Mapping  |
+----------+-----------+
           |
           | AddFunctionsFromAssembly()
           v
+----------------------+
| Feature Modules      |
| - Reforia.TestModule |
| - Reforia.IrcModule  |
+----------------------+
```

## Documentation 
- [Docs/01-Architecture.md](Docs/01-Architecture.md)
- [Docs/02-Project-Structure.md](Docs/02-Project-Structure.md)
- [Docs/03-RPC-Functions-System.md](Docs/03-RPC-Functions-System.md)
- [Docs/04-Adding-New-Module.md](Docs/04-Adding-New-Module.md)
- [Docs/05-Dev-Notes.md](Docs/05-Dev-Notes.md)