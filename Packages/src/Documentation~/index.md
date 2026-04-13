# MyNet

MyNet은 request/response 패킷 흐름과 Unity Multiplayer Services 세션 API를 함께 다루는 Unity 네트워킹 유틸리티입니다.

패킷 송수신 경계는 `MyNet.Packets`에 유지하고, 로비/방/플레이어 관리에는 `MyNet.Lobby`, `MyNet.Room`, `MyNet.Player`의 세션 기반 비동기 API를 제공합니다.

## 현재 구현 범위

- `MyNet.SetTransport(...)`로 transport를 교체할 수 있으며, 현재 공개 구현은 `Loopback`입니다.
- `MyNet.Packets.Client`, `MyNet.Packets.Server`로 request/response 패킷 큐를 다룹니다.
- `MyNet.Lobby.UpdateAsync(...)`로 세션 목록을 반복 조회하고 `RequestUpdate()`로 즉시 재조회할 수 있습니다.
- `MyNet.Room.CreateAsync(...)`, `JoinByCodeAsync(...)`, `JoinByIdAsync(...)`, `LeaveAsync(...)`, `KickAsync(...)`, `UpdateAsync(...)`를 제공합니다.
- `MyNet.Player.UpdateAsync(...)`로 현재 플레이어 속성을 수정할 수 있습니다.
- 기존 `StartCreate(...)`, `StartJoin(...)`, `StartUpdate(...)` 같은 시작 메서드는 하위 호환용 `Obsolete` API로 남아 있습니다.

## 문서

- [Lobby](lobby.md)
- [Room](room.md)
- [Transport](transport.md)

## 빠른 시작

```csharp
MyNet.SetTransport(MyNet.TransportKindEnum.Loopback);

MyNet.Packets.Server.OnReceived += request =>
{
    // 서버 측에서 요청 처리
    MyNet.Packets.Server.Send(response);
};

MyNet.Packets.Client.OnReceived += response =>
{
    // 클라이언트 측에서 응답 처리
};

MyNet.Packets.Client.Send(request);
```

## 메모

- Unity Multiplayer Services 초기화와 인증 이후에 세션 API를 사용하는 흐름을 전제로 합니다.
- `Loopback`은 로컬 테스트용이지만, 호출부가 같은 프레임 동기 완료를 기대하지 않도록 구성하는 편이 안전합니다.
- 패킷 흐름과 세션 관리 API는 함께 쓸 수 있지만, 각각의 책임 경계를 나눠 두는 방향을 유지하고 있습니다.
