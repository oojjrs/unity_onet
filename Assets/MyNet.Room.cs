using System;
using System.Collections.Generic;
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
            private static readonly Dictionary<Unity.Services.Lobbies.Models.Lobby, InternalRoomUnity> _unityRooms = new();
            private static GameObject _updater;

            internal static MyNetRoomInterface GetOrCreate(Unity.Services.Lobbies.Models.Lobby lobby)
            {
                if (_unityRooms.TryGetValue(lobby, out var value))
                    return value;

                value = new InternalRoomUnity(lobby);
                _unityRooms[lobby] = value;
                return value;
            }

            public static void StartCreate(CreateConfigInterface config, Action<MyNetRoomInterface> onOk = default, Action onFailed = default, Action<MyNetException> onException = default)
            {
                StopCreate();

                var go = new GameObject(nameof(InternalRoomCreator), typeof(InternalRoomCreator));
                var c = go.GetComponent<InternalRoomCreator>();
                c.Config = config;

                c.OnException += onException;
                c.OnFailed += onFailed;
                c.OnOk += onOk;

                _creator = go;
            }

            // 이 로직은 추방에도 사용되므로 여러 번 들어올 수 있다.
            public static void StartExit(ExitConfigInterface config, Action<string, string> onOk = default, Action<MyNetException> onException = default)
            {
                var go = new GameObject(nameof(InternalRoomExiter), typeof(InternalRoomExiter));
                var c = go.GetComponent<InternalRoomExiter>();
                c.Config = config;

                c.OnException += onException;
                c.OnOk += onOk;
            }

            // 샘플들이 다 15초라서 그냥 따라함.
            public static void StartHeartbeat(float heartbeatIntervalSeconds = 15, float errorIntervalSeconds = 5)
            {
                var go = new GameObject(nameof(InternalRoomHeartbeat), typeof(InternalRoomHeartbeat));
                var c = go.GetComponent<InternalRoomHeartbeat>();
                c.ErrorIntervalSeconds = errorIntervalSeconds;
                c.HeartbeatIntervalSeconds = heartbeatIntervalSeconds;

                _heartbeat = go;
            }

            public static void StartJoin(JoinConfigInterface config, Action<MyNetRoomInterface> onOk = default, Action onFailed = default, Action<MyNetException> onException = default)
            {
                StopJoin();

                var go = new GameObject(nameof(InternalRoomJoiner), typeof(InternalRoomJoiner));
                var c = go.GetComponent<InternalRoomJoiner>();
                c.Config = config;

                c.OnException += onException;
                c.OnFailed += onFailed;
                c.OnOk += onOk;

                _joiner = go;
            }

            public static void StartUpdate(UpdateConfigInterface config, Action<MyNetRoomInterface> onOk = default, Action onFailed = default, Action<MyNetException> onException = default)
            {
                StopUpdate();

                var go = new GameObject(nameof(InternalRoomUpdater), typeof(InternalRoomUpdater));
                var c = go.GetComponent<InternalRoomUpdater>();
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
