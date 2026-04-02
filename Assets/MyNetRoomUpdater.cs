using System;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    internal class MyNetRoomUpdater : MonoBehaviour
    {
        public MyNet.MyRoom.UpdateConfigInterface Config { get; set; }

        public event Action<MyNetException> OnException;
        public event Action OnFailed;
        public event Action<MyRoomInterface> OnOk;

        private async void Start()
        {
            try
            {
                var lobby = await LobbyService.Instance.UpdateLobbyAsync(Config.RoomId, new()
                {
                    Data = MyNet.ToRoomData(Config.RoomFields),
                    IsPrivate = Config.IsPrivate,
                });
                if (this != default)
                {
                    if (lobby != default)
                        OnOk?.Invoke(MyNet.MyRoom.GetOrCreate(lobby));
                    else
                        OnFailed?.Invoke();
                }
            }
            catch (LobbyServiceException e)
            {
                OnException?.Invoke(MyNet.ToException(e));
            }

            if (this != default)
                MyNet.MyRoom.StopUpdate();
        }
    }
}
