using System;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    internal class InternalRoomExiter : MonoBehaviour
    {
        public MyNet.Room.ExitConfigInterface Config { get; set; }

        public event Action<MyNetException> OnException;
        public event Action<string, string> OnOk;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(Config.RoomId, Config.PlayerId);

                if (this != null)
                    OnOk?.Invoke(Config.RoomId, Config.PlayerId);
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
