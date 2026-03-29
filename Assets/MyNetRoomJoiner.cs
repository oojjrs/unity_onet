using System;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace oojjrs.onet
{
    public class MyNetRoomJoiner : MonoBehaviour
    {
        public MyNet.Room.JoinConfigInterface Config { get; set; }

        public event Action<LobbyServiceException> OnException;
        public event Action OnFailed;
        public event Action<Lobby> OnOk;

        private async void Start()
        {
            try
            {
                var lobby = await LobbyService.Instance.JoinLobbyByIdAsync(Config.LobbyId, new()
                {
                    Player = new(id: Config.Account, data: MyNet.ToPlayerData(Config.PlayerFields)),
                });
                if (this != default)
                {
                    if (lobby != default)
                        OnOk?.Invoke(lobby);
                    else
                        OnFailed?.Invoke();
                }
            }
            catch (LobbyServiceException e)
            {
                OnException?.Invoke(e);
            }

            if (this != default)
                MyNet.Room.StopJoin();
        }
    }
}
