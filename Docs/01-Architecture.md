# Architecture

## Components

- **ASP.NET Core host** (`Reforia.Backend`)
    - Configures DI, SignalR, CORS, OpenAPI.
    - Maps SignalR hub at `/hub`.

- **SignalR Hub** (`AppHub`)
    - Exposes:
        - `Call(WebRequest) -> WebResponse` (RPC entrypoint)
        - `JoinIrcConnection(connectionId)` / `LeaveIrcConnection(connectionId)` (group membership)

- **RPC core** (`Reforia.Rpc`)
    - `WebDispatcher` dispatches `WebRequest` to a registered function.
    - `WebFunction<TRequest,TResponse>` is the base class for typed functions.

- **Feature modules** (`Reforia.TestModule`, `Reforia.IrcModule`, ...)
    - Register services into DI.
    - Register functions via assembly scanning.

## Call Flow (End-to-End)

```text
+--------------------+
|       Client       |
| (SignalR connection|
|       to /hub)     |
+---------+----------+
          |
          | 1) Call(WebRequest)
          v
+---------+-------------+
|        AppHub         |
|  - minimal validation |
|  - forwards request   |
+---------+-------------+
          |
          | 2) Dispatch(request)
          v
+---------+-------------+
|    WebDispatcher      |
|  - locate function    |
|  - deserialize body   |
|  - execute function   |
|  - build WebResponse  |
+---------+-------------+
          |
          | 3) Execute typed handler
          v
+---------+----------------------+
| WebFunction<TRequest,TResponse>|
|  - Handle(req, DI)             |
+---------+----------------------+
          |
          | 4) Return WebResponse
          v
+--------------------+
|       Client       |
| Receives response  |
+--------------------+
```
## SignalR groups (Join/Leave)

`JoinIrcConnection(connectionId)` adds the current SignalR connection to a named group (used for IRC broadcasts).

Client connectionId (SignalR) ---> joins group "connectionId" for targeted broadcasts

This is separate from RPC calls; it is used for server-to-client fanout to a selected audience.

