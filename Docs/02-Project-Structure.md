# Project structure

## Repository root

- `ReforiaBackend.sln` — solution
- `README.md` — quick onboarding and contracts
- `Docs/` — detailed documentation

## Projects

### `Reforia.Backend`
Host project:
- `Program.cs`
  - Adds SignalR
  - Adds RPC (`AddRpc()`) and module registrations
  - Configures CORS
  - Maps hub `/hub`

- `Hubs/AppHub.cs`
  - `Call(WebRequest)` RPC entrypoint
  - Group join/leave

### `Reforia.Rpc`
RPC framework project:
- `Contracts/`
  - `WebRequest`
  - `WebResponse`
- `Core/`
  - `WebDispatcher`
  - `WebFunction<TRequest,TResponse>` (typed function base)
- `RpcBuilder.cs`, `ServiceCollectionExtensions.cs`
  - DI and function registration utilities

### `Reforia.TestModule`
Example module:
- `Services/` — module services registered into DI
- `Functions/`
  - request/response models (DTOs)
  - function classes implementing server-side logic
- `TestModuleExtensions.cs`
  - module registration (services + function scan)

### `Reforia.IrcModule`
Feature module using the same pattern:
- services + functions + module extension registration

### Tools
- `Reforia.RpcTestTool` (WPF)