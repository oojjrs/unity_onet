using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;

namespace oojjrs.onet
{
    internal class InternalRoomUnity : MyNetRoomInterface
    {
        private readonly Lobby _lobby;

        MyNetPlayerInterface MyNetRoomInterface.Host => ((MyNetRoomInterface)this).Players.FirstOrDefault(t => t.IsHost);
        string MyNetRoomInterface.HostId => _lobby.HostId;
        string MyNetRoomInterface.Id => _lobby.Id;
        bool MyNetRoomInterface.IsPrivate => _lobby.IsPrivate;
        string MyNetRoomInterface.Name => _lobby.Name;
        int MyNetRoomInterface.PlayerCount => _lobby.Players.Count;
        int MyNetRoomInterface.PlayerCountMax => _lobby.MaxPlayers;
        IEnumerable<MyNetPlayerInterface> MyNetRoomInterface.Players => _lobby.Players.Select(player => MyNet.Player.GetOrCreate(player, () =>
        {
            if (player.Data.TryGetValue(MyNet.PlayerPropertyNickname, out var value))
                return new(this, player, value.Value);
            // 프로필에 있는 Name을 쓰면 처음에 데이터 동기화를 안 해준다 이색기가.
            else if (string.IsNullOrWhiteSpace(player.Profile?.Name) == false)
                return new(this, player, player.Profile.Name);
            else
                return new(this, player, player.Id);
        }));

        internal InternalRoomUnity(Lobby lobby)
        {
            _lobby = lobby;
        }

        string MyNetRoomInterface.GetData(string key)
        {
            if (_lobby.Data.TryGetValue(key, out var value))
                return value.Value;
            else
                return string.Empty;
        }
    }
}
