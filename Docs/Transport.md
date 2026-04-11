# Transport

This document describes how packets move through MyNet.

## Overview

MyNet is organized around two packet queues:

- `MyNet.Packets.Client`
- `MyNet.Packets.Server`

The client side owns outgoing `MyNetRequest` packets and incoming `MyNetResponse` packets.
The server side owns incoming `MyNetRequest` packets and outgoing `MyNetResponse` packets.

The transport layer is responsible only for moving packets between those two sides.

## Current Transport

### `Loopback`

`Loopback` is an in-process transport.

It does not serialize packets onto a real network connection. Instead, it forwards packets between the client queue and the server queue inside the same Unity process.

Even though it is local, it is intentionally treated as asynchronous behavior. Code should not assume that sending a request immediately produces a response in the same frame.

This helps reduce behavior differences between local testing and real network-backed transports.

## Public Packet Flow

### Client side

Send a request:

```csharp
MyNet.Packets.Client.Send(request);
```

Receive a response:

```csharp
if (MyNet.Packets.Client.TryDequeue(out MyNetResponse response))
{
    // Handle response.
}
```

### Server side

Receive a request:

```csharp
if (MyNet.Packets.Server.TryDequeue(out MyNetRequest request))
{
    // Handle request.
}
```

Send a response:

```csharp
MyNet.Packets.Server.Send(response);
```

## Selecting a Transport

Use `MyNet.SetTransport(...)` to choose the active transport implementation.

```csharp
MyNet.SetTransport(MyNet.TransportKindEnum.Loopback);
```

The active transport may change at runtime, so callers should treat transport selection as configuration rather than one-time initialization.
