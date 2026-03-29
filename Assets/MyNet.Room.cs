using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static class Room
        {
            public interface CreateConfigInterface
            {
                string Account { get; }
                bool IsPrivate { get; }
                int MaxPlayers { get; }
                IEnumerable<Field> PlayerFields { get; }
                IEnumerable<Field> RoomFields { get; }
                string Title { get; }
            }

            public interface ExitConfigInterface
            {
                string PlayerId { get; }
                string RoomId { get; }
            }

            public interface JoinConfigInterface
            {
                string Account { get; }
                string RoomId { get; }
                IEnumerable<Field> PlayerFields { get; }
            }

            public interface UpdateConfigInterface
            {
                bool IsPrivate { get; }
                IEnumerable<Field> RoomFields { get; }
                string RoomId { get; }
            }

            private static GameObject _creator;
            private static GameObject _joiner;
            private static GameObject _updater;

            public static void StartCreate(CreateConfigInterface config, Action<Unity.Services.Lobbies.Models.Lobby> onOk = default, Action onFailed = default, Action<LobbyServiceException> onException = default)
            {
                StopCreate();

                var go = new GameObject(nameof(MyNetRoomCreator), typeof(MyNetRoomCreator));
                var c = go.GetComponent<MyNetRoomCreator>();
                c.Config = config;

                c.OnException += onException;
                c.OnFailed += onFailed;
                c.OnOk += onOk;

                _creator = go;
            }

            // 이 로직은 추방에도 사용되므로 여러 번 들어올 수 있다.
            public static void StartExit(ExitConfigInterface config, Action<string, string> onOk = default, Action<LobbyServiceException> onException = default)
            {
                var go = new GameObject(nameof(MyNetRoomExiter), typeof(MyNetRoomExiter));
                var c = go.GetComponent<MyNetRoomExiter>();
                c.Config = config;

                c.OnException += onException;
                c.OnOk += onOk;
            }

            public static void StartJoin(JoinConfigInterface config, Action<Unity.Services.Lobbies.Models.Lobby> onOk = default, Action onFailed = default, Action<LobbyServiceException> onException = default)
            {
                StopJoin();

                var go = new GameObject(nameof(MyNetRoomJoiner), typeof(MyNetRoomJoiner));
                var c = go.GetComponent<MyNetRoomJoiner>();
                c.Config = config;

                c.OnException += onException;
                c.OnFailed += onFailed;
                c.OnOk += onOk;

                _joiner = go;
            }

            public static void StartUpdate(UpdateConfigInterface config, Action<Unity.Services.Lobbies.Models.Lobby> onOk = default, Action onFailed = default, Action<LobbyServiceException> onException = default)
            {
                StopUpdate();

                var go = new GameObject(nameof(MyNetRoomUpdater), typeof(MyNetRoomUpdater));
                var c = go.GetComponent<MyNetRoomUpdater>();
                c.Config = config;

                c.OnException += onException;
                c.OnFailed += onFailed;
                c.OnOk += onOk;

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
