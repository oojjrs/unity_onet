# Changelog

## 1.7.0

- UGS 인증과 플레이어 이름 조회 책임을 `com.oojjrs.oauth`로 분리하고 `MyNetAuthenticator` 및 `MyNetAuthenticationException`을 제거했습니다.
- `MyNetAuthenticator` 사용자는 `Authenticator.CallbackInterface`로 이관해야 합니다. `MyNetAuthenticationException`은 `AuthenticationServiceException`, 인증 중 발생한 `MyNetRequestFailedException`은 `AuthenticationRequestFailedException`으로 대체됩니다. 네트워크 API가 전달하는 `MyNetRequestFailedException`은 그대로 유지됩니다.
- `MyNetAuthenticationException.Notifications`는 `AuthenticationServiceException.Notifications`의 `AuthenticationNotification` 목록으로 대체됩니다.
- Authentication 패키지 직접 의존성을 제거했습니다.
- 직접 사용하는 Core와 Multiplayer만 패키지 의존성으로 유지했습니다.
- 레거시 heartbeat API가 인증 싱글턴 대신 호출자가 제공한 플레이어 ID를 사용하도록 변경했습니다.

## 1.6.11

- `MyNetAuthenticator`가 콜백 없이도 경고만 남기고 안전하게 종료되도록 정리했습니다.
- 애플리케이션 종료 중에는 `MyNetAuthenticator`가 자신을 파괴하지 않도록 정리했습니다.

## 1.6.10

- 세션 기반 `Lobby`, `Room`, `Player` 비동기 API의 현재 동작과 입력 조건을 문서에 반영했습니다.
- `MyNetRoomInterface`, `MyNetPlayerInterface`, 공통 콜백 인터페이스의 노출 표면을 문서에 정리했습니다.
- 패키지 README와 패키지 문서 인덱스를 현재 구현 범위 기준으로 갱신했습니다.
- 패킷 처리 흐름을 큐 polling 예시 대신 `OnReceived`, `OnFinishThisHandling` 이벤트 기준으로 정리했습니다.

## 1.6.9

- 세션 프로퍼티 업데이트 API를 추가했습니다.
- 세션 업데이트 시 공개 여부(`IsPrivate`)도 함께 반영하도록 수정했습니다.
- 세션 기반 플레이어 속성 수정 API를 추가했습니다.
- 세션 기반 방 생성, 참가, 퇴장, 수정 API의 콜백 인터페이스와 실패 코드를 정리했습니다.
- 세션 기반 로비 조회를 `UpdateAsync(...)`와 `MyNetLobbyCallbacksInterface` 기준으로 정리했습니다.
- `KickAsync(...)`와 `LeaveAsync(...)`의 성공 콜백을 `roomId`, `playerId`를 함께 넘기도록 정리했습니다.
