using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;

namespace oojjrs.onet
{
    internal class MyRoomUnity : MyRoomInterface
    {
        private readonly Lobby _lobby;

        MyPlayerInterface MyRoomInterface.Host => ((MyRoomInterface)this).Players.FirstOrDefault(t => t.IsHost);
        string MyRoomInterface.HostId => _lobby.HostId;
        string MyRoomInterface.Id => _lobby.Id;
        bool MyRoomInterface.IsPrivate => _lobby.IsPrivate;
        string MyRoomInterface.Name => _lobby.Name;
        int MyRoomInterface.PlayerCount => _lobby.Players.Count;
        int MyRoomInterface.PlayerCountMax => _lobby.MaxPlayers;
        IEnumerable<MyPlayerInterface> MyRoomInterface.Players => _lobby.Players.Select(player => MyNet.MyPlayer.GetOrCreate(player, () =>
        {
            if (player.Data.TryGetValue(MyNet.PlayerPropertyNickname, out var value))
                return new(this, player, value.Value);
            // 프로필에 있는 Name을 쓰면 처음에 데이터 동기화를 안 해준다 이색기가.
            else if (string.IsNullOrWhiteSpace(player.Profile?.Name) == false)
                return new(this, player, player.Profile.Name);
            else
                return new(this, player, player.Id);
        }));

        internal MyRoomUnity(Lobby lobby)
        {
            _lobby = lobby;
        }
    }
}
