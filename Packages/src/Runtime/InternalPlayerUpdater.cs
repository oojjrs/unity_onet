using System;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    internal class InternalPlayerUpdater : MonoBehaviour
    {
        public MyNet.Player.UpdateConfigInterface Config { get; set; }

        public event Action<MyNetException> OnException;
        public event Action OnFailed;
        public event Action<MyNetRoomInterface> OnOk;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            try
            {
                var lobby = await LobbyService.Instance.UpdatePlayerAsync(Config.RoomId, Config.PlayerId, new()
                {
                    Data = MyNet.ToPlayerData(Config.PlayerFields),
                });
                if (this != null)
                {
                    if (lobby is not null)
                        OnOk?.Invoke(MyNet.Room.GetOrCreate(lobby));
                    else
                        OnFailed?.Invoke();
                }
            }
            catch (LobbyServiceException e)
            {
                OnException?.Invoke(MyNet.ToException(e));
            }
            finally
            {
                if (this != null)
                    Destroy(gameObject);
            }
        }
    }
}
