# Dev notes

## Target framework
- `net10.0`

## SignalR
- Hub route: `/hub`
- RPC entrypoint: `Call(WebRequest) -> WebResponse`

## CORS
CORS policy is configured for a desktop/web client origin list.
(See host configuration in `Reforia.Backend`.)

## Error semantics
- `WebResponse.Ok(...)` -> `statusCode = 200`
- `WebResponse.BadRequest(...)` -> `statusCode = 403`
- `WebResponse.Error(...)` -> `statusCode = 500`

## Request correlation
Clients must generate `requestId` and match responses by `requestId`.