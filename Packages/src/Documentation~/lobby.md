# Lobby

`MyNet.Lobby`는 세션 목록을 주기적으로 조회하거나, 필요할 때 즉시 한 번 더 조회하도록 요청할 수 있는 비동기 API를 제공합니다.

기존 `StartUpdate(...)`는 하위 호환용으로 남아 있지만, 새 코드에서는 `UpdateAsync(...)` 사용을 권장합니다.

## 설정 인터페이스

```csharp
using System.Threading;

class LobbyUpdateConfig : MyNet.Lobby.UpdateConfigInterface
{
    public CancellationToken CancellationToken { get; init; }
    public int PollingDelaySeconds { get; init; } = 5;
}
```

## 콜백 인터페이스

```csharp
using System.Collections.Generic;

public interface MyNetLobbyCallbacksInterface : MyNetCallbacksInterface
{
    void OnOk(IEnumerable<MyNetRoomInterface> rooms);
}
```

`MyNetCallbacksInterface`의 공통 콜백은 아래와 같습니다.

```csharp
public interface MyNetCallbacksInterface
{
    void OnBusy();
    void OnException(MyNetSessionException e);
    void OnFailed(MyNetCallbacksInterface.FailureEnum e);
}
```

현재 `FailureEnum`에는 아래 항목이 있습니다.

- `EmptyCode`
- `EmptyPlayerId`
- `EmptyRoomId`
- `NotFoundRoom`
- `NotPermitted`

## 사용 예시

```csharp
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

class LobbyCallbacks : MyNetLobbyCallbacksInterface
{
    public void OnBusy() => Debug.Log("busy");
    public void OnException(MyNetSessionException e) => Debug.LogException(e);
    public void OnFailed(MyNetCallbacksInterface.FailureEnum e) => Debug.LogWarning(e);
    public void OnOk(IEnumerable<MyNetRoomInterface> rooms)
    {
        foreach (var room in rooms)
            Debug.Log($"{room.Id} / {room.Title}");
    }
}
```

```csharp
var config = new LobbyUpdateConfig
{
    CancellationToken = cancellationToken,
    PollingDelaySeconds = 5,
};

await MyNet.Lobby.UpdateAsync(config, callbacks);
```

`OnOk(...)`로 전달되는 항목은 `MyNetRoomInterface`이며 현재 아래 정보를 읽을 수 있습니다.

```csharp
public interface MyNetRoomInterface
{
    string Code { get; }
    bool HasPassword { get; }
    MyNetPlayerInterface Host { get; }
    string HostId { get; }
    string Id { get; }
    bool IsLocked { get; }
    bool IsPrivate { get; }
    int PlayerCount { get; }
    int PlayerCountAvailable { get; }
    int PlayerCountMax { get; }
    IEnumerable<MyNetPlayerInterface> Players { get; }
    string Title { get; }

    string GetData(string key);
}
```

## 수동 갱신과 중지

```csharp
MyNet.Lobby.RequestUpdate();
MyNet.Lobby.StopUpdate();
```

- `RequestUpdate()`는 다음 polling delay를 기다리지 않고 한 번 더 조회하도록 요청합니다.
- `StopUpdate()`는 이후 추가 조회를 멈추게 합니다.

## 동작 메모

- `UpdateAsync(...)`는 시작 직후 한 번 조회하고, 이후 `PollingDelaySeconds` 간격으로 반복 조회합니다.
- `RequestUpdate()`는 내부 대기 상태를 깨워 다음 주기를 기다리지 않고 다시 조회하게 합니다.
- 이미 다른 `UpdateAsync(...)` 호출이 실행 중이면 새 호출은 `OnBusy()`로 정리됩니다.
- 세션 조회가 이미 진행 중일 때 `StopUpdate()`가 호출되면, 진행 중인 `QuerySessionsAsync()` 자체를 중간 취소하지는 못합니다. 대신 조회가 끝난 뒤 결과 콜백을 건너뛰고 종료합니다.
- Unity Services 초기화 이후에 사용하는 전제를 갖습니다.
