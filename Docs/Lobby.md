# Lobby

방 목록 조회는 `MyNet.Lobby.StartUpdate(...)`로 시작하고, 필요할 때 `RequestUpdate()`로 즉시 새로고침하고, 끝낼 때 `StopUpdate()`를 호출합니다.

## 기본 조회

```csharp
MyNet.Lobby.StartUpdate(
    5f,
    rooms =>
    {
        foreach (var room in rooms)
            Debug.Log($"{room.Title} ({room.PlayerCount}/{room.PlayerCountMax})");
    },
    exception =>
    {
        Debug.LogError(exception);
    });
```

## 세션 기반 조회

```csharp
using System.Threading;

class SessionLobbyUpdateConfig : MyNet.Lobby.UpdateConfigInterface
{
    public CancellationToken CancellationToken { get; }
    public int PollingDelaySeconds => 5;

    public SessionLobbyUpdateConfig(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
    }
}
```

```csharp
var config = new SessionLobbyUpdateConfig(cancellationToken);

MyNet.Lobby.StartUpdate(
    config,
    rooms =>
    {
        foreach (var room in rooms)
            Debug.Log($"{room.Id} / {room.Title}");
    },
    exception =>
    {
        Debug.LogError(exception);
    });
```

## 수동 갱신과 중지

```csharp
MyNet.Lobby.RequestUpdate();
MyNet.Lobby.StopUpdate();
```

## 주의

- `MyNet.Lobby.StartUpdate(float, ...)`는 `Action<MyNetException>`를 받습니다.
- `MyNet.Lobby.StartUpdate(UpdateConfigInterface, ...)`는 `Action<MyNetSessionException>`를 받습니다.
- Unity Services 초기화 이후에 사용하는 편이 안전합니다.
