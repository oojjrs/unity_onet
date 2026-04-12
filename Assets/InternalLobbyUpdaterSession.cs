using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace oojjrs.onet
{
    internal class InternalLobbyUpdaterSession : MonoBehaviour
    {
        private float _nextLobbyUpdateAtSeconds;

        public MyNet.Lobby.UpdateConfigInterface Config { get; set; }
        public bool UpdateRequested { get; set; }

        public event Action<MyNetSessionException> OnException;
        public event Action<IEnumerable<MyNetRoomInterface>> OnUpdate;

        private async void Update()
        {
            // time scale이나 프레임 영향이 없어야 하므로 Time.time을 사용할 수 없다.
            var time = Time.realtimeSinceStartup;
            if (UpdateRequested || (time >= _nextLobbyUpdateAtSeconds))
            {
                _nextLobbyUpdateAtSeconds = time + Config.PollingDelaySeconds;

                UpdateRequested = false;

                try
                {
                    var results = await MultiplayerService.Instance.QuerySessionsAsync(new()
                    {
                    });

                    if ((Config.CancellationToken.IsCancellationRequested == false) && (this != default))
                        OnUpdate?.Invoke(results.Sessions.Select(session => MyNet.Lobby.GetOrCreate(session)));
                }
                catch (SessionException e)
                {
                    OnException?.Invoke(MyNet.ToException(e));
                }
            }
        }
    }
}
