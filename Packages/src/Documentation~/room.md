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

## 방 생성

```csharp
await MyNet.Room.CreateAsync(config, callbacks);
```

## 방 참가

```csharp
await MyNet.Room.JoinByCodeAsync(config, callbacks);
await MyNet.Room.JoinByIdAsync(config, callbacks);
```

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

## 방 정보 수정

```csharp
await MyNet.Room.UpdateAsync(config, callbacks);
```

현재 세션에서 호스트 권한이 필요한 작업입니다.

## 플레이어 속성 수정

```csharp
await MyNet.Player.UpdateAsync(config, callbacks);
```

현재 플레이어 자신의 속성만 수정할 수 있으며, 다른 플레이어 ID를 넣으면 `NotPermitted`로 실패합니다.
