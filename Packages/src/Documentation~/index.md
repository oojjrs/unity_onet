# MyNet

MyNet은 request/response 패킷 흐름을 중심으로 구성한 Unity 네트워킹 유틸리티입니다.

Unity Multiplayer Services를 필요에 따라 사용하되, 게임 코드에서 보이는 API는 최대한 작고 명확하게 유지하는 것을 목표로 합니다.

## 기능

- 인증 관련 보조 기능
- 플레이어 및 로비 데이터 처리 보조 기능
- request/response 패킷 흐름
- 교체 가능한 transport 진입점

## 문서

- [Lobby](lobby.md)
- [Room](room.md)
- [Transport](transport.md)

## 빠른 시작

```csharp
MyNet.SetTransport(MyNet.TransportKindEnum.Loopback);

MyNet.Packets.Client.Send(request);

if (MyNet.Packets.Server.TryDequeue(out MyNetRequest serverRequest))
{
    // 서버 측에서 요청 처리
    MyNet.Packets.Server.Send(response);
}

if (MyNet.Packets.Client.TryDequeue(out MyNetResponse clientResponse))
{
    // 클라이언트 측에서 응답 처리
}
```

## 메모

- Unity Lobby는 백엔드 서비스 중 하나로 사용할 수 있지만, MyNet은 자체적인 패킷 중심 API 표면을 유지합니다.
- `Loopback`은 로컬 테스트를 위한 경로이지만, 호출부가 동기 동작에 의존하지 않도록 설계되어 있습니다.
- 실시간 transport의 세부 구현은 바뀔 수 있어도, client/server 패킷 경계는 명확하게 유지하는 것을 목표로 합니다.
