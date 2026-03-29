using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static class Lobby
        {
            public interface CreateConfigInterface
            {
                string Account { get; }
                IEnumerable<Field> LobbyFields { get; }
                bool IsPrivate { get; }
                int MaxPlayers { get; }
                IEnumerable<Field> PlayerFields { get; }
                string Title { get; }
            }

            public interface ExitConfigInterface
            {
                string LobbyId { get; }
                string PlayerId { get; }
            }

            public interface JoinConfigInterface
            {
                string Account { get; }
                string LobbyId { get; }
                IEnumerable<Field> PlayerFields { get; }
            }

            private static GameObject _creator;
            private static GameObject _joiner;
            private static GameObject _updater;

            public static void RequestUpdate()
            {
                if (_updater != default)
                {
                    var c = _updater.GetComponent<MyNetLobbyUpdater>();
                    c.UpdateRequested = true;
                }
            }

            public static void StartCreate(CreateConfigInterface config, Action<Unity.Services.Lobbies.Models.Lobby> onOk = default, Action onFailed = default, Action<LobbyServiceException> onException = default)
            {
                StopCreate();

                var go = new GameObject(nameof(MyNetLobbyCreator), typeof(MyNetLobbyCreator));
                var c = go.GetComponent<MyNetLobbyCreator>();
                c.Config = config;

                c.OnException += onException;
                c.OnFailed += onFailed;
                c.OnOk += onOk;

                _creator = go;
            }

            // 이 로직은 추방에도 사용되므로 여러 번 들어올 수 있다.
            public static void StartExit(ExitConfigInterface config, Action<string, string> onOk = default, Action<LobbyServiceException> onException = default)
            {
                var go = new GameObject(nameof(MyNetLobbyExiter), typeof(MyNetLobbyExiter));
                var c = go.GetComponent<MyNetLobbyExiter>();
                c.Config = config;

                c.OnException += onException;
                c.OnOk += onOk;
            }

            public static void StartJoin(JoinConfigInterface config, Action<Unity.Services.Lobbies.Models.Lobby> onOk = default, Action onFailed = default, Action<LobbyServiceException> onException = default)
            {
                StopJoin();

                var go = new GameObject(nameof(MyNetLobbyJoiner), typeof(MyNetLobbyJoiner));
                var c = go.GetComponent<MyNetLobbyJoiner>();
                c.Config = config;

                c.OnException += onException;
                c.OnFailed += onFailed;
                c.OnOk += onOk;

                _joiner = go;
            }

            public static void StartUpdate(float updateIntervalSeconds = 5, Action<List<Unity.Services.Lobbies.Models.Lobby>> onUpdate = default, Action<LobbyServiceException> onException = default)
            {
                StopUpdate();

                var go = new GameObject(nameof(MyNetLobbyUpdater), typeof(MyNetLobbyUpdater));
                var c = go.GetComponent<MyNetLobbyUpdater>();
                c.UpdateIntervalSeconds = updateIntervalSeconds;

                c.OnException += onException;
                c.OnUpdate += onUpdate;

                _updater = go;
            }

            public static void StopCreate()
            {
                if (_creator != default)
                {
                    UnityEngine.Object.Destroy(_creator);

                    _creator = default;
                }
            }

            public static void StopJoin()
            {
                if (_joiner != default)
                {
                    UnityEngine.Object.Destroy(_joiner);

                    _joiner = default;
                }
            }

            public static void StopUpdate()
            {
                if (_updater != default)
                {
                    UnityEngine.Object.Destroy(_updater);

                    _updater = default;
                }
            }
        }
    }
}
