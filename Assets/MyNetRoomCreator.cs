using System;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace oojjrs.onet
{
    public class MyNetRoomCreator : MonoBehaviour
    {
        public MyNet.Room.CreateConfigInterface Config { get; set; }

        public event Action<LobbyServiceException> OnException;
        public event Action OnFailed;
        public event Action<Lobby> OnOk;

        private async void Start()
        {
            try
            {
                var room = await LobbyService.Instance.CreateLobbyAsync(Config.Title, Config.MaxPlayers, new()
                {
                    Data = MyNet.ToRoomData(Config.RoomFields),
                    IsPrivate = Config.IsPrivate,
                    Player = new(id: Config.Account, data: MyNet.ToPlayerData(Config.PlayerFields)),
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
                MyNet.Room.StopCreate();
        }
    }
}
