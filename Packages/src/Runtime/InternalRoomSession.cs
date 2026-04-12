using System.Collections.Generic;
using System.Linq;
using Unity.Services.Multiplayer;

namespace oojjrs.onet
{
    internal class InternalRoomSession : MyNetRoomInterface
    {
        string MyNetRoomInterface.Code => Session.Code;
        bool MyNetRoomInterface.HasPassword => Session.HasPassword;
        MyNetPlayerInterface MyNetRoomInterface.Host => ((MyNetRoomInterface)this).Players.FirstOrDefault(t => t.IsHost);
        string MyNetRoomInterface.HostId => Session.Host;
        string MyNetRoomInterface.Id => Session.Id;
        bool MyNetRoomInterface.IsLocked => Session.IsLocked;
        bool MyNetRoomInterface.IsPrivate => Session.IsPrivate;
        int MyNetRoomInterface.PlayerCount => Session.PlayerCount;
        int MyNetRoomInterface.PlayerCountAvailable => Session.AvailableSlots;
        int MyNetRoomInterface.PlayerCountMax => Session.MaxPlayers;
        IEnumerable<MyNetPlayerInterface> MyNetRoomInterface.Players => Session.Players.Select(player => MyNet.Player.GetOrCreate(player, () => new(this)));
        string MyNetRoomInterface.Title => Session.Name;

        internal ISession Session { get; set; }

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
