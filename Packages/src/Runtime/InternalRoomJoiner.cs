using System;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    internal class InternalRoomJoiner : MonoBehaviour
    {
        public MyNet.Room.JoinConfigInterface Config { get; set; }

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
                var lobby = await LobbyService.Instance.JoinLobbyByIdAsync(Config.RoomId, new()
                {
                    Player = new(id: Config.Account, data: MyNet.ToPlayerData(Config.PlayerFields, Config.PlayerNickname)),
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
                    MyNet.Room.StopJoin();
            }
        }
    }
}
