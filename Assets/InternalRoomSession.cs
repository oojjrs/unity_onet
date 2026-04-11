using System.Collections.Generic;
using System.Linq;
using Unity.Services.Multiplayer;

namespace oojjrs.onet
{
    internal class InternalRoomSession : MyNetRoomInterface
    {
        internal readonly ISession _session;

        string MyNetRoomInterface.Code => _session.Code;
        bool MyNetRoomInterface.HasPassword => _session.HasPassword;
        MyNetPlayerInterface MyNetRoomInterface.Host => ((MyNetRoomInterface)this).Players.FirstOrDefault(t => t.IsHost);
        string MyNetRoomInterface.HostId => _session.Host;
        string MyNetRoomInterface.Id => _session.Id;
        bool MyNetRoomInterface.IsLocked => _session.IsLocked;
        bool MyNetRoomInterface.IsPrivate => _session.IsPrivate;
        int MyNetRoomInterface.PlayerCount => _session.PlayerCount;
        int MyNetRoomInterface.PlayerCountAvailable => _session.AvailableSlots;
        int MyNetRoomInterface.PlayerCountMax => _session.MaxPlayers;
        IEnumerable<MyNetPlayerInterface> MyNetRoomInterface.Players => _session.Players.Select(player => MyNet.Player.GetOrCreate(player, () => new(this, player)));
        string MyNetRoomInterface.Title => _session.Name;

        internal InternalRoomSession(ISession session)
        {
            _session = session;
        }

        string MyNetRoomInterface.GetData(string key)
        {
            if (_session.Properties.TryGetValue(key, out var value))
                return value.Value;
            else
                return string.Empty;
        }
    }
}
