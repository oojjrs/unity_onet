# Room

`MyNet.Room`과 `MyNet.Player`는 세션 기반 비동기 API를 제공합니다.

기존 `StartCreate(...)`, `StartJoin(...)`, `StartUpdate(...)` 같은 시작 메서드는 하위 호환용으로 남아 있지만, 새 코드에서는 `...Async(...)` 계열 사용을 권장합니다.

## 공통 콜백 인터페이스

비동기 네트워크 처리 결과는 콜백 인터페이스로 받습니다.

```csharp
public interface MyNetCallbacksInterface
{
    void OnBusy();
    void OnException(MyNetSessionException e);
    void OnFailed(MyNetCallbacksInterface.FailureEnum e);
}
```

성공 결과 형태에 따라 아래 인터페이스를 사용합니다.

```csharp
public interface MyNetVoidCallbacksInterface : MyNetCallbacksInterface
{
    void OnOk();
}

public interface MyNetRoomCallbacksInterface : MyNetCallbacksInterface
{
    void OnOk(MyNetRoomInterface room);
}

public interface MyNetExitCallbacksInterface : MyNetCallbacksInterface
{
    void OnOk(string roomId, string playerId);
}
```

`FailureEnum`에는 현재 아래 항목이 있습니다.

- `EmptyCode`
- `EmptyPlayerId`
- `EmptyRoomId`
- `NotFoundRoom`
- `NotPermitted`

## 방 생성

```csharp
class CreateConfig : MyNet.Room.CreateConfigInterface
{
    public CancellationToken CancellationToken { get; init; }
    public bool IsLocked { get; init; }
    public bool IsPrivate { get; init; }
    public IEnumerable<MyNet.Field> RoomFields { get; init; }
    public int MaxPlayers { get; init; }
    public string Password { get; init; }
    public IEnumerable<MyNet.Field> PlayerFields { get; init; }
    public string PlayerNickname { get; init; }
    public string Title { get; init; }
}

await MyNet.Room.CreateAsync(config, callbacks);
```

## 방 참가

방 코드를 알고 있을 때는 `JoinByCodeAsync(...)`, 방 ID를 알고 있을 때는 `JoinByIdAsync(...)`를 사용합니다.

```csharp
class JoinConfig : MyNet.Room.JoinConfigInterface
{
    public string Account { get; init; }
    public CancellationToken CancellationToken { get; init; }
    public string Code { get; init; }
    public string Password { get; init; }
    public IEnumerable<MyNet.Field> PlayerFields { get; init; }
    public string PlayerNickname { get; init; }
    public string RoomId { get; init; }
}
```

```csharp
await MyNet.Room.JoinByCodeAsync(config, callbacks);
await MyNet.Room.JoinByIdAsync(config, callbacks);
```

- `JoinByCodeAsync(...)`는 현재 `Code`, `PlayerFields`, `PlayerNickname`을 사용합니다.
- `JoinByIdAsync(...)`는 현재 `RoomId`, `Password`, `PlayerFields`, `PlayerNickname`을 사용합니다.
- `JoinConfigInterface`의 `Account`는 현재 구현에서 사용하지 않습니다.

## 방 퇴장과 추방

`LeaveAsync(...)`와 `KickAsync(...)`는 `MyNetExitCallbacksInterface`를 사용하며, 성공 시 `roomId`와 `playerId`를 함께 돌려줍니다.

```csharp
await MyNet.Room.LeaveAsync(config, callbacks);
await MyNet.Room.KickAsync(config, callbacks);
```

입력값이 비어 있으면 서비스 호출 전에 `OnFailed(...)`로 정리됩니다.

- `JoinByCodeAsync(...)`: `Code`가 비어 있으면 `EmptyCode`
- `JoinByIdAsync(...)`, `LeaveAsync(...)`, `KickAsync(...)`, `Room.UpdateAsync(...)`: `RoomId`가 비어 있으면 `EmptyRoomId`
- `Player.UpdateAsync(...)`: `PlayerId`가 비어 있으면 `EmptyPlayerId`

`LeaveAsync(...)`는 현재 플레이어 자신의 퇴장에 사용하고, 다른 플레이어 ID를 넣으면 `NotPermitted`로 실패합니다.
`KickAsync(...)`는 busy 상태와 무관하게 여러 번 호출될 수 있도록 별도로 처리됩니다.

## 방 정보 수정

```csharp
class UpdateRoomConfig : MyNet.Room.UpdateConfigInterface
{
    public CancellationToken CancellationToken { get; init; }
    public bool IsPrivate { get; init; }
    public IEnumerable<MyNet.Field> RoomFields { get; init; }
    public string RoomId { get; init; }
}

await MyNet.Room.UpdateAsync(config, callbacks);
```

현재 구현은 세션 호스트가 `IsPrivate`와 세션 프로퍼티를 함께 저장하는 흐름입니다.

## 방/플레이어 조회 표면

성공 콜백으로 받는 `MyNetRoomInterface`와 `MyNetPlayerInterface`는 현재 아래 멤버를 제공합니다.

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

public interface MyNetPlayerInterface
{
    string Id { get; }
    bool IsHost { get; }
    string Nickname { get; }

    string GetData(string key);
}
```

## 플레이어 속성 수정

```csharp
class UpdatePlayerConfig : MyNet.Player.UpdateConfigInterface
{
    public CancellationToken CancellationToken { get; init; }
    public IEnumerable<MyNet.Field> PlayerFields { get; init; }
    public string PlayerId { get; init; }
    public string RoomId { get; init; }
}

await MyNet.Player.UpdateAsync(config, callbacks);
```

현재 플레이어 자신의 속성만 수정할 수 있으며, 다른 플레이어 ID를 넣으면 `NotPermitted`로 실패합니다.
