# Lobby

로비 조회 및 생성 기능을 제공합니다.

---

## 개요

- 로비 목록 자동 조회 (Polling)
- 수동 업데이트 요청
- 로비 생성 기능
- 콜백 기반 결과 전달
- 예외 전달
- Time.timeScale의 영향을 받지 않음

---

## 로비 조회

### 시작

```csharp
MyNet.Lobby.StartUpdate(
    lobbies =>
    {
        Debug.Log($"Lobby Count: {lobbies.Count}");
    },
    exception =>
    {
        Debug.LogError(exception);
    });
```

---

### 수동 업데이트 요청

```csharp
MyNet.Lobby.RequestUpdate();
```

---

### 업데이트 간격 설정

```csharp
MyNet.Lobby.UpdateIntervalSeconds = 2f;
```

---

### 중지

```csharp
MyNet.Lobby.StopUpdate();
```

---

## 로비 생성

### ConfigInterface 구현

```csharp
class MyLobbyConfig : MyNet.Lobby.ConfigInterface
{
    public string Title => "My Lobby";
    public int MaxPlayers => 4;
    public bool IsPrivate => false;
    public string Account => "player_001";

    public IEnumerable<Field> LobbyFields => new[]
    {
        new Field { key = "mode", value = "rank", visibility = Field.VisibilityEnum.Public }
    };

    public IEnumerable<Field> PlayerFields => new[]
    {
        new Field { key = "rank", value = "gold", visibility = Field.VisibilityEnum.Member }
    };
}
```

---

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

---

### 중지

```csharp
MyNet.Lobby.StopCreate();
```

---

## 콜백

```csharp
// Update
Action<List<Lobby>> onUpdate

// Create
Action<Lobby> onCreate
Action onCreateFailed

// 공통
Action<LobbyServiceException> onException
```

---

## 동작 방식

- `Time.realtimeSinceStartup` 기반
- 내부 `MonoBehaviour`를 통해 실행
- updater / creator GameObject 자동 생성 및 제거
- async 처리 중 객체 파괴 안전 처리 포함

---

## 내부 구조

- Update → `MyNetLobbyUpdater` :contentReference[oaicite:0]{index=0}  
- Create → `MyNetLobbyCreator` :contentReference[oaicite:1]{index=1}  
- API Entry → `MyNet.Lobby` :contentReference[oaicite:2]{index=2}  

---

## 주의 사항

- 내부 GameObject는 자동 관리됨
- 직접 `Updater` / `Creator`를 붙이지 말 것
- Unity Services 초기화 후 사용 필요