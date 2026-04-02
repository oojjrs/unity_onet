using System;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    internal class MyNetPlayerUpdater : MonoBehaviour
    {
        public MyNet.MyPlayer.UpdateConfigInterface Config { get; set; }

        public event Action<MyNetException> OnException;
        public event Action OnFailed;
        public event Action<MyRoomInterface> OnOk;

        private async void Start()
        {
            try
            {
                var lobby = await LobbyService.Instance.UpdatePlayerAsync(Config.RoomId, Config.PlayerId, new()
                {
                    Data = MyNet.ToPlayerData(Config.PlayerFields),
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
                Destroy(gameObject);
        }
    }
}
