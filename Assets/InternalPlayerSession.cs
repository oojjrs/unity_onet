using Unity.Services.Multiplayer;

namespace oojjrs.onet
{
    internal class InternalPlayerSession : MyNetPlayerInterface
    {
        // 세션제에서는 IReadOnlyPlayer가 자주 갱신된다고 한다.
        private readonly string _playerId;
        private readonly InternalRoomSession _room;

        string MyNetPlayerInterface.Id => _playerId;
        bool MyNetPlayerInterface.IsHost => ((MyNetRoomInterface)_room).HostId == _playerId;
        string MyNetPlayerInterface.Nickname => ((MyNetPlayerInterface)this).GetData(MyNet.PlayerPropertyNickname);
        internal IReadOnlyPlayer Player { get; set; }
        internal ISession Session => _room._session;

        internal InternalPlayerSession(InternalRoomSession room, IReadOnlyPlayer player)
        {
            _playerId = player.Id;
            _room = room;

            Player = player;
        }

        string MyNetPlayerInterface.GetData(string key)
        {
            if (Player.Properties.TryGetValue(key, out var value))
                return value.Value;
            else
                return string.Empty;
        }
    }
}
