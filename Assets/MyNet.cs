using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    public static class MyNet
    {
        public static class Lobby
        {
            private static GameObject _updater;

            public static float UpdateIntervalSeconds { get; set; } = 5;
            public static bool UpdateRequested { get; set; }

            public static event Action<LobbyServiceException> OnException;
            public static event Action<List<Unity.Services.Lobbies.Models.Lobby>> OnUpdate;

            public static void StartUpdate()
            {
                StopUpdate();

                _updater = new GameObject(nameof(MyNetLobbyUpdater), typeof(MyNetLobbyUpdater));
            }

            public static void StopUpdate()
            {
                if (_updater != default)
                {
                    UnityEngine.Object.Destroy(_updater);

                    _updater = default;
                }
            }

            internal static void ThrowException(LobbyServiceException e)
            {
                OnException?.Invoke(e);
            }

            internal static void Update(List<Unity.Services.Lobbies.Models.Lobby> lobbies)
            {
                OnUpdate?.Invoke(lobbies);
            }
        }
    }
}
