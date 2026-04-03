# MyNet

Unity Multiplayer Services 기반 네트워크 유틸리티.

Unity Services의 복잡한 API를 직접 다루지 않고,  
간결한 인터페이스로 흐름을 구성하기 위한 레이어.

---

## Features

- Auth (인증)
- Player (플레이어 데이터)
- Room (상위 개념)
- Lobby (실제 방)

---

## Install

https://github.com/oojjrs/unity_onet.git

---

## Documents

- Docs/Auth.md
- Docs/Player.md
- Docs/Room.md
- Docs/Lobby.md

---

## Structure

- Auth → 인증 처리
- Player → 플레이어 정보 관리
- Room → 상위 공간 (로비 집합 개념)
- Lobby → 실제 방 (Unity Lobby 기반)

---

## Design

- 인터페이스 기반 요청 구조
- 기능별 독립 실행 (MonoBehaviour)
- Polling 기반 상태 갱신
- 최소한의 추상화

---

## Notes

- Unity Services 초기화 이후 사용
- Lobby는 상태 저장소 역할
- 실시간 동기화는 직접 처리

---

## License

MIT
