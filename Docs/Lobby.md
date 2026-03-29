# Lobby

로비 목록을 주기적으로 조회하고, 결과를 이벤트로 전달하는 기능을 제공합니다.

---

## 개요

- 자동 로비 조회 (Polling)
- 수동 업데이트 트리거 지원
- 이벤트 기반 결과 전달
- 예외 외부 전달
- Time.timeScale의 영향을 받지 않음

---

## 사용 방법

### 시작

```csharp
MyNet.Lobby.StartUpdate();
```

---

### 이벤트 구독

```csharp
MyNet.Lobby.OnUpdate += lobbies =>
{
    Debug.Log($"Lobby Count: {lobbies.Count}");
};

MyNet.Lobby.OnException += e =>
{
    Debug.LogError(e);
};
```

---

### 수동 업데이트

```csharp
MyNet.Lobby.UpdateRequested = true;
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

## 이벤트

```csharp
public static event Action<LobbyServiceException> OnException;
public static event Action<List<Lobby>> OnUpdate;
```

---

## 동작 방식

- `Time.realtimeSinceStartup` 기반으로 동작
- 내부 `MonoBehaviour`를 통해 업데이트 루프 실행
- 재시작 시 updater 자동 재생성
- async 처리 중 객체 파괴에 대한 안전 처리 포함

---

## 주의 사항

- Updater GameObject는 내부에서 관리되며 직접 생성할 필요 없음
- `MyNetLobbyUpdater`를 수동으로 추가하지 말 것
- 사용 전에 Unity Services 초기화 필요