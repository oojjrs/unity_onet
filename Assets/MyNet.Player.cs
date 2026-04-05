using System;
using System.Collections.Generic;
using UnityEngine;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static class Player
        {
            public interface UpdateConfigInterface
            {
                IEnumerable<Field> PlayerFields { get; }
                string PlayerId { get; }
                string RoomId { get; }
            }

            private static readonly Dictionary<Unity.Services.Lobbies.Models.Player, InternalPlayerUnity> _unityPlayers = new();

            internal static IEnumerable<MyNet.Field> GetFields(string nickname)
            {
                yield return new()
                {
                    key = MyNet.PlayerPropertyNickname,
                    value = nickname,
                    visibility = Field.VisibilityEnum.Public,
                };
            }

            internal static MyNetPlayerInterface GetOrCreate(Unity.Services.Lobbies.Models.Player player, Func<InternalPlayerUnity> onFallback)
            {
                if (_unityPlayers.TryGetValue(player, out var value))
                    return value;

                value = onFallback?.Invoke();
                _unityPlayers[player] = value;
                return value;
            }

            public static void StartUpdate(UpdateConfigInterface config, Action<MyNetRoomInterface> onOk = default, Action onFailed = default, Action<MyNetException> onException = default)
            {
                var go = new GameObject(nameof(InternalPlayerUpdater), typeof(InternalPlayerUpdater));
                var c = go.GetComponent<InternalPlayerUpdater>();
                c.Config = config;

                c.OnException += onException;
                c.OnFailed += onFailed;
                c.OnOk += onOk;
            }
        }
    }
}
