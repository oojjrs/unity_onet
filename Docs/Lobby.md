# Lobby

로비 조회, 생성, 참가, 퇴장 기능을 제공합니다.

---

## 개요

- 로비 목록 자동 조회 (Polling)
- 수동 업데이트 요청
- 로비 생성
- 로비 참가
- 로비 퇴장 / 플레이어 제거
- 콜백 기반 결과 전달
- Time.timeScale의 영향을 받지 않음

---

## 로비 조회

### 시작

```csharp
MyNet.Lobby.StartUpdate(
    5f,
    lobbies =>
    {
        Debug.Log($"Lobby Count: {lobbies.Count}");
    },
    exception =>
    {
        Debug.LogError(exception);
    });
```

### 수동 업데이트 요청

```csharp
MyNet.Lobby.RequestUpdate();
```

### 중지

```csharp
MyNet.Lobby.StopUpdate();
```

---

## 로비 생성

### CreateConfigInterface 구현

```csharp
class MyLobbyConfig : MyNet.Lobby.CreateConfigInterface
{
    public string Title => "My Lobby";
    public int MaxPlayers => 4;
    public bool IsPrivate => false;
    public string Account => "player_001";

    public IEnumerable<MyNet.Lobby.Field> LobbyFields => new[]
    {
        new MyNet.Lobby.Field
        {
            key = "mode",
            value = "rank",
            visibility = MyNet.Lobby.Field.VisibilityEnum.Public
        }
    };

    public IEnumerable<MyNet.Lobby.Field> PlayerFields => new[]
    {
        new MyNet.Lobby.Field
        {
            key = "rank",
            value = "gold",
            visibility = MyNet.Lobby.Field.VisibilityEnum.Member
        }
    };
}
```

### 생성

```csharp
MyNet.Lobby.StartCreate(
    new MyLobbyConfig(),
    lobby =>
    {
        Debug.Log($"Created Lobby: {lobby.Id}");
    },
    () =>
    {
        Debug.LogError("Create Failed");
    },
    exception =>
    {
        Debug.LogError(exception);
    });
```

### 중지

```csharp
MyNet.Lobby.StopCreate();
```

---

## 로비 참가

### JoinConfigInterface 구현

```csharp
class MyJoinConfig : MyNet.Lobby.JoinConfigInterface
{
    public string Account => "player_001";
    public string LobbyId => "LOBBY_ID";

    public IEnumerable<MyNet.Lobby.Field> PlayerFields => new[]
    {
        new MyNet.Lobby.Field
        {
            key = "rank",
            value = "gold",
            visibility = MyNet.Lobby.Field.VisibilityEnum.Member
        }
    };
}
```

### 참가

```csharp
MyNet.Lobby.StartJoin(
    new MyJoinConfig(),
    lobby =>
    {
        Debug.Log($"Joined Lobby: {lobby.Id}");
    },
    () =>
    {
        Debug.LogError("Join Failed");
    },
    exception =>
    {
        Debug.LogError(exception);
    });
```

### 중지

```csharp
MyNet.Lobby.StopJoin();
```

---

## 로비 퇴장

### ExitConfigInterface 구현

```csharp
class MyExitConfig : MyNet.Lobby.ExitConfigInterface
{
    public string LobbyId => "LOBBY_ID";
    public string PlayerId => "player_001";
}
```

### 퇴장 요청

```csharp
MyNet.Lobby.StartExit(
    new MyExitConfig(),
    (lobbyId, playerId) =>
    {
        Debug.Log($"Exited Lobby: {lobbyId}, Player: {playerId}");
    },
    exception =>
    {
        Debug.LogError(exception);
    });
```

---

## 콜백

```csharp
// Update
Action<List<Lobby>> onUpdate

// Create / Join
Action<Lobby> onOk
Action onFailed

// Exit
Action<string, string> onOk

// Common
Action<LobbyServiceException> onException
```

---

## 동작 방식

- `Time.realtimeSinceStartup` 기반
- 기능별 MonoBehaviour 실행
- GameObject 자동 생성 및 제거
- async 안전 처리 포함
- Exit는 1회 실행 후 자기 자신을 파괴하는 단발성 구조

---

## 내부 구조

- Update → `MyNetLobbyUpdater`
- Create → `MyNetLobbyCreator`
- Join → `MyNetLobbyJoiner`
- Exit → `MyNetLobbyExiter`
- API Entry → `MyNet.Lobby`

---

## 주의 사항

- 내부 GameObject는 자동 관리됩니다.
- 직접 컴포넌트를 추가하지 않는 것을 권장합니다.
- Unity Services 초기화 이후 사용해야 합니다.
- Exit는 `StopExit()` 없이 요청 단위로 동작합니다.
- Exit는 일반 퇴장뿐 아니라 특정 플레이어 제거 용도로도 사용할 수 있습니다.
