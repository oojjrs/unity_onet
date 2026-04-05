using System;
using System.Collections.Generic;
using UnityEngine;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static class Lobby
        {
            private static GameObject _updater;

            public static void RequestUpdate()
            {
                if (_updater != default)
                {
                    var c = _updater.GetComponent<InternalLobbyUpdater>();
                    c.UpdateRequested = true;
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
