# OOJJRS' Unity Netcode Helper

`MyNet` request/response 패킷 흐름과 Unity Multiplayer Services 세션 API를 제공하는 Unity 런타임 패키지입니다.

## 설치

Unity Package Manager의 **Add package from git URL**에 입력합니다.

```text
https://github.com/oojjrs/unity_onet.git?path=/Packages/src
```

대상은 Unity 6000.0 이상이며 패키지 ID는 `com.oojjrs.onet`입니다.

## 구성 요소

| 구성 요소 | 종류 | 용도 |
| --- | --- | --- |
| `MyNet.Packets.Client` / `Server` | 패킷 API | request/response 큐 송수신 |
| `MyNet.Lobby` | 세션 API | 세션 목록 조회와 갱신 |
| `MyNet.Room` | 세션 API | 방 생성, 참가, 퇴장, 추방, 정보 수정 |
| `MyNet.Player` | 세션 API | 현재 플레이어 속성 수정 |

## 사용 범위

`MyNet.SetTransport(MyNet.TransportKindEnum.Loopback)`으로 같은 프로세스 안의 패킷 흐름을 사용할 수 있습니다. 현재 공개 transport 구현은 `Loopback`입니다.

세션 API는 Unity Services 초기화와 인증이 끝난 뒤 사용하며, 인증과 계정 선택은 이 패키지 범위에 포함되지 않습니다.

## 문서

- [패키지 README](Packages/src/README.md)
- [전체 문서](Packages/src/Documentation~/index.md)
- [Lobby](Packages/src/Documentation~/lobby.md)
- [Room](Packages/src/Documentation~/room.md)
- [Transport](Packages/src/Documentation~/transport.md)
