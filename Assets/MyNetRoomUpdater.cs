using System;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace oojjrs.onet
{
    public class MyNetRoomUpdater : MonoBehaviour
    {
        public MyNet.Room.UpdateConfigInterface Config { get; set; }

        public event Action<LobbyServiceException> OnException;
        public event Action OnFailed;
        public event Action<Lobby> OnOk;

        private async void Start()
        {
            try
            {
                var room = await LobbyService.Instance.UpdateLobbyAsync(Config.RoomId, new()
                {
                    Data = MyNet.ToRoomData(Config.RoomFields),
                    IsPrivate = Config.IsPrivate,
                });
                if (this != default)
                {
                    if (room != default)
                        OnOk?.Invoke(room);
                    else
                        OnFailed?.Invoke();
                }
            }
            catch (LobbyServiceException e)
            {
                OnException?.Invoke(e);
            }

            if (this != default)
                MyNet.Room.StopUpdate();
        }
    }
}
