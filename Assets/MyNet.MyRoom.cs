using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static class MyRoom
        {
            public interface CreateConfigInterface
            {
                string Account { get; }
                bool IsPrivate { get; }
                int MaxPlayers { get; }
                IEnumerable<Field> PlayerFields { get; }
                string PlayerNickname { get; }
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
                IEnumerable<Field> PlayerFields { get; }
                string PlayerNickname { get; }
                string RoomId { get; }
            }

            public interface UpdateConfigInterface
            {
                bool IsPrivate { get; }
                IEnumerable<Field> RoomFields { get; }
                string RoomId { get; }
            }

            private static GameObject _creator;
            private static GameObject _heartbeat;
            private static GameObject _joiner;
            private static readonly Dictionary<Lobby, MyRoomUnity> _unityRooms = new();
            private static GameObject _updater;

            internal static MyRoomInterface GetOrCreate(Lobby lobby)
            {
                if (_unityRooms.TryGetValue(lobby, out var value))
                    return value;

                value = new MyRoomUnity(lobby);
                _unityRooms[lobby] = value;
                return value;
            }

            public static void StartCreate(CreateConfigInterface config, Action<MyRoomInterface> onOk = default, Action onFailed = default, Action<MyNetException> onException = default)
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
            public static void StartExit(ExitConfigInterface config, Action<string, string> onOk = default, Action<MyNetException> onException = default)
            {
                var go = new GameObject(nameof(MyNetRoomExiter), typeof(MyNetRoomExiter));
                var c = go.GetComponent<MyNetRoomExiter>();
                c.Config = config;

                c.OnException += onException;
                c.OnOk += onOk;
            }

            // 샘플들이 다 15초라서 그냥 따라함.
            public static void StartHeartbeat(float heartbeatIntervalSeconds = 15, float errorIntervalSeconds = 5)
            {
                var go = new GameObject(nameof(MyNetRoomHeartbeat), typeof(MyNetRoomHeartbeat));
                var c = go.GetComponent<MyNetRoomHeartbeat>();
                c.ErrorIntervalSeconds = errorIntervalSeconds;
                c.HeartbeatIntervalSeconds = heartbeatIntervalSeconds;

                _heartbeat = go;
            }

            public static void StartJoin(JoinConfigInterface config, Action<MyRoomInterface> onOk = default, Action onFailed = default, Action<MyNetException> onException = default)
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

            public static void StartUpdate(UpdateConfigInterface config, Action<MyRoomInterface> onOk = default, Action onFailed = default, Action<MyNetException> onException = default)
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

            public static void StopHeartbeat()
            {
                if (_heartbeat != default)
                {
                    UnityEngine.Object.Destroy(_heartbeat);

                    _heartbeat = default;
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
