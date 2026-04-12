using System.Collections.Generic;
using System.Linq;
using Unity.Services.Multiplayer;

namespace oojjrs.onet
{
    internal class InternalRoomSessionStub : MyNetRoomInterface
    {
        string MyNetRoomInterface.Code => string.Empty;
        bool MyNetRoomInterface.HasPassword => Session.HasPassword;
        MyNetPlayerInterface MyNetRoomInterface.Host => default;
        string MyNetRoomInterface.HostId => Session.HostId;
        string MyNetRoomInterface.Id => Session.Id;
        bool MyNetRoomInterface.IsLocked => Session.IsLocked;
        bool MyNetRoomInterface.IsPrivate => false;
        int MyNetRoomInterface.PlayerCount => Session.MaxPlayers - Session.AvailableSlots;
        int MyNetRoomInterface.PlayerCountAvailable => Session.AvailableSlots;
        int MyNetRoomInterface.PlayerCountMax => Session.MaxPlayers;
        IEnumerable<MyNetPlayerInterface> MyNetRoomInterface.Players => Enumerable.Empty<MyNetPlayerInterface>();
        string MyNetRoomInterface.Title => Session.Name;

        internal ISessionInfo Session { get; set; }

        string MyNetRoomInterface.GetData(string key)
        {
            if (Session.Properties == default)
                return string.Empty;

            if (Session.Properties.TryGetValue(key, out var value))
                return value.Value;
            else
                return string.Empty;
        }
    }
}
