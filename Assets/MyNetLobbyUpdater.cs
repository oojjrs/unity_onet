using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    internal class MyNetLobbyUpdater : MonoBehaviour
    {
        private float _nextLobbyUpdateAtSeconds;

        public float UpdateIntervalSeconds { get; set; }
        public bool UpdateRequested { get; set; }

        public event Action<MyNetException> OnException;
        public event Action<IEnumerable<MyRoomInterface>> OnUpdate;

        private async void Update()
        {
            // time scale이나 프레임 영향이 없어야 하므로 Time.time을 사용할 수 없다.
            var time = Time.realtimeSinceStartup;
            if (UpdateRequested || (time >= _nextLobbyUpdateAtSeconds))
            {
                _nextLobbyUpdateAtSeconds = time + UpdateIntervalSeconds;

                UpdateRequested = false;

                try
                {
                    var response = await LobbyService.Instance.QueryLobbiesAsync();
                    if (this != default)
                        OnUpdate?.Invoke(response.Results.Select(lobby => MyNet.MyRoom.GetOrCreate(lobby)));
                }
                catch (LobbyServiceException e)
                {
                    OnException?.Invoke(MyNet.ToException(e));
                }
            }
        }
    }
}
