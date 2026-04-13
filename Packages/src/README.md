# MyNet

MyNet은 request/response 패킷 흐름과 Unity Multiplayer Services 세션 API를 함께 다루는 Unity 네트워킹 유틸리티입니다.

게임 코드에서 보이는 API는 최대한 작고 명확하게 유지하되, 현재는 패킷 큐와 세션 기반 `Lobby/Room/Player` 비동기 API를 함께 제공합니다.

## 기능

- request/response 패킷 흐름
- 세션 기반 로비 조회와 방/플레이어 수정 비동기 API
- 교체 가능한 transport 진입점
- Unity Multiplayer Services 예외를 MyNet 예외 타입으로 정리하는 보조 기능

## 문서

- [Documentation](Documentation~/index.md)
- [Lobby](Documentation~/lobby.md)
- [Room](Documentation~/room.md)
- [Transport](Documentation~/transport.md)

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

- `Loopback`은 로컬 테스트용 transport이며, 호출부가 같은 프레임 동기 완료를 기대하지 않도록 구성하는 편이 안전합니다.
- 세션 기반 API는 Unity Services 초기화와 인증 이후 사용을 전제로 합니다.
- 기존 `Start...` 계열 메서드는 하위 호환용으로 남아 있고, 새 코드에서는 `...Async(...)` 계열 사용을 권장합니다.
