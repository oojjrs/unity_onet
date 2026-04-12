using System.Collections.Generic;
using System.Threading;
using Unity.Services.Lobbies.Models;

namespace oojjrs.onet
{
    internal class InternalPlayerUnity : MyNetPlayerInterface, MyNet.Player.UpdateConfigInterface
    {
        private string _nickname;
        private readonly Player _player;
        private readonly MyNetRoomInterface _room;

        string MyNetPlayerInterface.Id => _player.Id;
        bool MyNetPlayerInterface.IsHost => _player.Id == _room.HostId;
        string MyNetPlayerInterface.Nickname => ((MyNetPlayerInterface)this).GetData(MyNet.PlayerPropertyNickname);

        CancellationToken MyNet.Player.UpdateConfigInterface.CancellationToken => CancellationToken.None;
        IEnumerable<MyNet.Field> MyNet.Player.UpdateConfigInterface.PlayerFields => MyNet.Player.GetFields(_nickname);
        string MyNet.Player.UpdateConfigInterface.PlayerId => _player.Id;
        string MyNet.Player.UpdateConfigInterface.RoomId => _room.Id;

        internal InternalPlayerUnity(MyNetRoomInterface room, Player player, string nickname)
        {
            _nickname = nickname;
            _player = player;
            _room = room;
        }

        string MyNetPlayerInterface.GetData(string key)
        {
            if (_player.Data.TryGetValue(key, out var value))
                return value.Value;
            else
                return string.Empty;
        }
    }
}
