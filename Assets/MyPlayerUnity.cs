using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace oojjrs.onet
{
    internal class MyPlayerUnity : MyPlayerInterface, MyNet.MyPlayer.UpdateConfigInterface
    {
        private string _nickname;
        private readonly Player _player;
        private readonly MyRoomInterface _room;

        string MyPlayerInterface.Id => _player.Id;
        bool MyPlayerInterface.IsHost => _player.Id == _room.HostId;
        string MyPlayerInterface.Nickname
        {
            get => _nickname;
            set
            {
                _nickname = value;
                MyNet.MyPlayer.StartUpdate(this);
            }
        }

        IEnumerable<MyNet.Field> MyNet.MyPlayer.UpdateConfigInterface.PlayerFields => MyNet.MyPlayer.GetFields(_nickname);
        string MyNet.MyPlayer.UpdateConfigInterface.PlayerId => _player.Id;
        string MyNet.MyPlayer.UpdateConfigInterface.RoomId => _room.Id;

        internal MyPlayerUnity(MyRoomInterface room, Player player, string nickname)
        {
            _nickname = nickname;
            _player = player;
            _room = room;
        }
    }
}
