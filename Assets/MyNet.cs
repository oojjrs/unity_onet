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
            public interface ConfigInterface
            {
                // TODO: 아직 indexing 기능은 지원하지 않...
                public struct Field
                {
                    public enum VisibilityEnum
                    {
                        Public,
                        Member,
                        Private,
                    }

                    public string key;
                    public string value;
                    public VisibilityEnum visibility;
                }

                string Account { get; }
                IEnumerable<Field> LobbyFields { get; }
                bool IsPrivate { get; }
                int MaxPlayers { get; }
                IEnumerable<Field> PlayerFields { get; }
                string Title { get; }
            }

            private static GameObject _creator;
            private static GameObject _updater;

            internal static ConfigInterface Config { get; private set; }

            public static float UpdateIntervalSeconds { get; set; } = 5;
            internal static bool UpdateRequested { get; set; }

            private static event Action<Unity.Services.Lobbies.Models.Lobby> OnCreate;
            private static event Action OnCreateFailed;
            private static event Action<LobbyServiceException> OnException;
            private static event Action<List<Unity.Services.Lobbies.Models.Lobby>> OnUpdate;

            internal static void RaiseCreated(Unity.Services.Lobbies.Models.Lobby lobby)
            {
                OnCreate?.Invoke(lobby);
            }

            internal static void RaiseCreateFailed()
            {
                OnCreateFailed?.Invoke();
            }

            internal static void RaiseException(LobbyServiceException e)
            {
                OnException?.Invoke(e);
            }

            public static void RequestUpdate()
            {
                UpdateRequested = true;
            }

            public static void StartCreate(ConfigInterface config, Action<Unity.Services.Lobbies.Models.Lobby> onCreate = default, Action onCreateFailed = default, Action<LobbyServiceException> onException = default)
            {
                Config = config;

                if (onCreate != default)
                {
                    OnCreate -= onCreate;
                    OnCreate += onCreate;
                }

                if (onCreateFailed != default)
                {
                    OnCreateFailed -= onCreateFailed;
                    OnCreateFailed += onCreateFailed;
                }

                if (onException != default)
                {
                    OnException -= onException;
                    OnException += onException;
                }

                _creator = new GameObject(nameof(MyNetLobbyCreator), typeof(MyNetLobbyCreator));
            }

            public static void StopCreate()
            {
                if (_creator != default)
                {
                    UnityEngine.Object.Destroy(_creator);

                    _creator = default;
                }
            }

            public static void StartUpdate(Action<List<Unity.Services.Lobbies.Models.Lobby>> onUpdate = default, Action<LobbyServiceException> onException = default)
            {
                StopUpdate();

                if (onException != default)
                {
                    OnException -= onException;
                    OnException += onException;
                }

                if (onUpdate != default)
                {
                    OnUpdate -= onUpdate;
                    OnUpdate += onUpdate;
                }

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

            internal static void Update(List<Unity.Services.Lobbies.Models.Lobby> lobbies)
            {
                OnUpdate?.Invoke(lobbies);
            }
        }
    }
}
