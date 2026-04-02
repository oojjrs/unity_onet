using System;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    internal class MyNetRoomHeartbeat : MonoBehaviour
    {
        private float _nextTimeSeconds;

        public float ErrorIntervalSeconds { get; set; }
        public float HeartbeatIntervalSeconds { get; set; }

        public event Action<MyNetException> OnException;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private async void Update()
        {
            var time = Time.realtimeSinceStartup;
            if (time >= _nextTimeSeconds)
            {
                _nextTimeSeconds = time + HeartbeatIntervalSeconds;

                try
                {
                    var ids = await LobbyService.Instance.GetJoinedLobbiesAsync();
                    if (this != default)
                    {
                        foreach (var id in ids)
                        {
                            var lobby = await LobbyService.Instance.GetLobbyAsync(id);
                            if (this != default)
                            {
                                if (lobby.HostId == AuthenticationService.Instance.PlayerId)
                                {
                                    await LobbyService.Instance.SendHeartbeatPingAsync(id);
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (LobbyServiceException e)
                {
                    _nextTimeSeconds += ErrorIntervalSeconds;

                    OnException?.Invoke(MyNet.ToException(e));
                }
            }
        }
    }
}
