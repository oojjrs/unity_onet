using System;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    public class MyNetLobbyExiter : MonoBehaviour
    {
        public MyNet.Lobby.ExitConfigInterface Config { get; set; }

        public event Action<LobbyServiceException> OnException;
        public event Action<string, string> OnOk;

        private async void Start()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(Config.LobbyId, Config.PlayerId);

                if (this != default)
                    OnOk?.Invoke(Config.LobbyId, Config.PlayerId);
            }
            catch (LobbyServiceException e)
            {
                OnException?.Invoke(e);
            }

            if (this != default)
                Destroy(gameObject);
        }
    }
}
