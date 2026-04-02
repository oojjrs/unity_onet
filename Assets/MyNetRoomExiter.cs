using System;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    internal class MyNetRoomExiter : MonoBehaviour
    {
        public MyNet.MyRoom.ExitConfigInterface Config { get; set; }

        public event Action<MyNetException> OnException;
        public event Action<string, string> OnOk;

        private async void Start()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(Config.RoomId, Config.PlayerId);

                if (this != default)
                    OnOk?.Invoke(Config.RoomId, Config.PlayerId);
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
