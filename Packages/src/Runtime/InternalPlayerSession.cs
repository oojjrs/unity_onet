using Unity.Services.Multiplayer;

namespace oojjrs.onet
{
    internal class InternalPlayerSession : MyNetPlayerInterface
    {
        private readonly InternalRoomSession _room;

        string MyNetPlayerInterface.Id => Player.Id;
        bool MyNetPlayerInterface.IsHost => ((MyNetRoomInterface)_room).HostId == Player.Id;
        string MyNetPlayerInterface.Nickname => ((MyNetPlayerInterface)this).GetData(MyNet.PlayerPropertyNickname);

        // IReadOnlyPlayer가 자주 갱신된다고 한다.
        internal IReadOnlyPlayer Player { get; set; }
        internal ISession Session => _room.Session;

        internal InternalPlayerSession(InternalRoomSession room)
        {
            _room = room;
        }

        string MyNetPlayerInterface.GetData(string key)
        {
            if (Player.Properties == default)
                return string.Empty;

            if (Player.Properties.TryGetValue(key, out var value))
                return value.Value;
            else
                return string.Empty;
        }
    }
}
