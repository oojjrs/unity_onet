# MyNet

Unity Netcode 및 Services를 위한 경량 헬퍼 유틸리티입니다.
이벤트 기반 구조를 내부적으로 유지하면서, 외부에서는 간결한 콜백 방식으로 사용할 수 있도록 설계되었습니다.

---

## 빠른 시작

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

## 기능

* 로비 시스템 → [Docs/Lobby.md](Docs/Lobby.md)

---

## 의존성

```json
{
  "com.unity.services.authentication": "3.5.2",
  "com.unity.services.deployment": "1.6.2",
  "com.unity.multiplayer.center": "1.0.1",
  "com.unity.services.multiplayer": "2.0.0",
  "com.unity.multiplayer.tools": "2.2.8",
  "com.unity.netcode.gameobjects": "2.11.0",
  "com.unity.transport": "2.6.0"
}
```

---

## 요구 사항

* Unity 6000.0 이상
* Unity Services 초기화 필요

---

## 설계 방향

* 불필요한 추상화 배제
* 내부 이벤트 기반 + 외부 콜백 인터페이스
* 최소한의 런타임 오버헤드
* 기존 프로젝트에 쉽게 결합 가능
