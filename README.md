# MyNet

Unity Multiplayer Services 기반 네트워크 유틸리티.

Unity Services의 혼란스러운 네이밍(Lobby = Room)을 보정하여,  
명확한 구조로 사용할 수 있도록 설계됨.

---

## Features

- Auth (인증)
- Player (플레이어 데이터)
- Lobby (상위 공간 / 채널)
- Room (실제 방, Unity Lobby 래핑)

---

## Documents

- Docs/Auth.md
- Docs/Player.md
- Docs/Lobby.md
- Docs/Room.md

---

## Structure

- Auth → 인증 처리
- Player → 플레이어 정보
- Lobby → 상위 공간 (Room 리스트 관리)
- Room → 실제 방 (Unity Lobby 기반)

---

## Notes

- Unity Lobby는 내부적으로 Room으로 사용됨
- Lobby는 별도의 추상 개념
- 실시간 동기화는 직접 처리 필요

---

## License

MIT
