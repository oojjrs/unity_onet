using System;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    internal class InternalRoomUpdater : MonoBehaviour
    {
        public MyNet.Room.UpdateConfigInterface Config { get; set; }

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
                var lobby = await LobbyService.Instance.UpdateLobbyAsync(Config.RoomId, new()
                {
                    Data = MyNet.ToRoomData(Config.RoomFields),
                    IsPrivate = Config.IsPrivate,
                });
                if (this != default)
                {
                    if (lobby != default)
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
                if (this != default)
                    MyNet.Room.StopUpdate();
            }
        }
    }
}
