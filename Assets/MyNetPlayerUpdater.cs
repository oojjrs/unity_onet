using System;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace oojjrs.onet
{
    public class MyNetPlayerUpdater : MonoBehaviour
    {
        public MyNet.Player.UpdateConfigInterface Config { get; set; }

        public event Action<LobbyServiceException> OnException;
        public event Action OnFailed;
        public event Action<Lobby> OnOk;

        private async void Start()
        {
            try
            {
                var lobby = await LobbyService.Instance.UpdatePlayerAsync(Config.LobbyId, Config.PlayerId, new()
                {
                    Data = MyNet.ToPlayerData(Config.PlayerFields),
                });
                if (this != default)
                {
                    // 유니티는 무조건 적용/동기화되었다고 간주하는 모양임.
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
                Destroy(gameObject);
        }
    }
}
