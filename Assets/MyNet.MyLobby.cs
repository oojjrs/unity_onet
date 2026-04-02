using System;
using System.Collections.Generic;
using UnityEngine;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static class MyLobby
        {
            private static GameObject _updater;

            public static void RequestUpdate()
            {
                if (_updater != default)
                {
                    var c = _updater.GetComponent<MyNetLobbyUpdater>();
                    c.UpdateRequested = true;
                }
            }

            public static void StartUpdate(float updateIntervalSeconds = 5, Action<IEnumerable<MyRoomInterface>> onUpdate = default, Action<MyNetException> onException = default)
            {
                StopUpdate();

                var go = new GameObject(nameof(MyNetLobbyUpdater), typeof(MyNetLobbyUpdater));
                var c = go.GetComponent<MyNetLobbyUpdater>();
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
