using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static class Player
        {
            public interface UpdateConfigInterface
            {
                string LobbyId { get; }
                IEnumerable<Field> PlayerFields { get; }
                string PlayerId { get; }
            }

            public static void StartUpdate(UpdateConfigInterface config, Action<Unity.Services.Lobbies.Models.Lobby> onOk = default, Action onFailed = default, Action<LobbyServiceException> onException = default)
            {
                var go = new GameObject(nameof(MyNetPlayerUpdater), typeof(MyNetPlayerUpdater));
                var c = go.GetComponent<MyNetPlayerUpdater>();
                c.Config = config;

                c.OnException += onException;
                c.OnFailed += onFailed;
                c.OnOk += onOk;
            }
        }
    }
}
