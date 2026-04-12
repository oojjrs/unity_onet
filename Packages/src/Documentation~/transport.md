# Transport

이 문서는 MyNet에서 패킷이 어떻게 이동하는지를 설명합니다.

## 개요

MyNet은 두 개의 패킷 큐를 중심으로 구성됩니다.

- `MyNet.Packets.Client`
- `MyNet.Packets.Server`

클라이언트 측은 나가는 `MyNetRequest`와 들어오는 `MyNetResponse`를 가집니다.
서버 측은 들어오는 `MyNetRequest`와 나가는 `MyNetResponse`를 가집니다.

transport 계층은 이 두 영역 사이에서 패킷을 이동시키는 역할만 담당합니다.

## 현재 transport

### `Loopback`

`Loopback`은 같은 프로세스 안에서 동작하는 transport입니다.

실제 네트워크 연결로 패킷을 시리얼라이즈해 보내는 대신, 같은 Unity 프로세스 안에서 클라이언트 큐와 서버 큐 사이로 패킷을 옮깁니다.

다만 로컬 transport라고 해서 동기 호출처럼 취급하지는 않습니다. request를 보낸 직후 같은 프레임 안에서 response가 도착한다고 가정하면 안 됩니다.

이 규칙은 로컬 테스트와 실제 네트워크 transport 사이의 동작 차이를 줄이기 위한 것입니다.

## 공개 패킷 흐름

### 클라이언트 측

```csharp
MyNet.Packets.Client.Send(request);
```

```csharp
if (MyNet.Packets.Client.TryDequeue(out MyNetResponse response))
{
    // 응답 처리
}
```

### 서버 측

```csharp
if (MyNet.Packets.Server.TryDequeue(out MyNetRequest request))
{
    // 요청 처리
}
```

```csharp
MyNet.Packets.Server.Send(response);
```

## Transport 선택

현재 사용할 transport 구현은 `MyNet.SetTransport(...)`로 선택합니다.

```csharp
MyNet.SetTransport(MyNet.TransportKindEnum.Loopback);
```

transport는 실행 중에도 바뀔 수 있으므로, 호출부에서는 이를 일회성 초기화가 아니라 설정 변경으로 취급하는 편이 맞습니다.
