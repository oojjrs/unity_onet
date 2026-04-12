using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static class Lobby
        {
            public interface UpdateConfigInterface
            {
                CancellationToken CancellationToken { get; }
                int PollingDelaySeconds { get; }
            }

            private static readonly Dictionary<string, InternalRoomSessionStub> _rooms = new();
            private static GameObject _updater;

            internal static MyNetRoomInterface GetOrCreate(ISessionInfo session)
            {
                if (session == default)
                    return default;

                if (_rooms.TryGetValue(session.Id, out var value))
                {
                    value.Session = session;
                    return value;
                }

                value = new()
                {
                    Session = session
                };

                _rooms[session.Id] = value;
                return value;
            }

            public static void RequestUpdate()
            {
                if (_updater != default)
                {
                    var c = _updater.GetComponent<InternalLobbyUpdater>();
                    if (c != default)
                        c.UpdateRequested = true;

                    var cc = _updater.GetComponent<InternalLobbyUpdaterSession>();
                    if (cc != default)
                        cc.UpdateRequested = true;
                }
            }

            public static void StartUpdate(float updateIntervalSeconds = 5, Action<IEnumerable<MyNetRoomInterface>> onUpdate = default, Action<MyNetException> onException = default)
            {
                StopUpdate();

                var go = new GameObject(nameof(InternalLobbyUpdater), typeof(InternalLobbyUpdater));
                var c = go.GetComponent<InternalLobbyUpdater>();
                c.UpdateIntervalSeconds = updateIntervalSeconds;

                c.OnException += onException;
                c.OnUpdate += onUpdate;

                _updater = go;
            }

            public static void StartUpdate(UpdateConfigInterface config, Action<IEnumerable<MyNetRoomInterface>> onUpdate = default, Action<MyNetSessionException> onException = default)
            {
                StopUpdate();

                var go = new GameObject(nameof(InternalLobbyUpdaterSession), typeof(InternalLobbyUpdaterSession));
                var c = go.GetComponent<InternalLobbyUpdaterSession>();
                c.Config = config;

                c.OnException += onException;
                c.OnUpdate += onUpdate;

                _updater = go;
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
