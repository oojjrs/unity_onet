# MyNet

MyNet is a Unity networking utility built around request/response packets.

It uses Unity Multiplayer Services where appropriate, while keeping the gameplay-facing API small and explicit.

## Features

- Authentication helpers
- Player and lobby-related data helpers
- Request/response packet flow
- Swappable transport entry point

## Transport Model

MyNet separates packet ownership from packet transport.

- `MyNet.Packets.Client`
  Client-side entry point. Sends `MyNetRequest` packets and receives `MyNetResponse` packets.
- `MyNet.Packets.Server`
  Server-side entry point. Receives `MyNetRequest` packets and sends `MyNetResponse` packets.
- `MyNet.SetTransport(...)`
  Selects how packets move between the client side and the server side.

Current transport kinds:

- `MyNet.TransportKindEnum.Loopback`

`Loopback` does not perform real network I/O. It forwards packets between the client-side queue and the server-side queue inside the same process.

Even so, `Loopback` is intentionally treated as asynchronous transport. It should not be relied on as a same-frame round trip path.

## Basic Flow

```csharp
MyNet.SetTransport(MyNet.TransportKindEnum.Loopback);

MyNet.Packets.Client.Send(request);

if (MyNet.Packets.Server.TryDequeue(out MyNetRequest serverRequest))
{
    // Handle request on the server side.
    MyNet.Packets.Server.Send(response);
}

if (MyNet.Packets.Client.TryDequeue(out MyNetResponse clientResponse))
{
    // Consume response on the client side.
}
```

## Documents

- [`Docs/Lobby.md`](Docs/Lobby.md)
- [`Docs/Transport.md`](Docs/Transport.md)

## Notes

- Unity Lobby is used as one of the backing services, but MyNet keeps its own packet-oriented API surface.
- `Loopback` exists to support local packet flow without making callers depend on synchronous behavior.
- Real-time transport details may continue to evolve, but the client/server packet boundary is intended to stay clear.

## License

MIT
