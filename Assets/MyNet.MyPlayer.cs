using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static class MyPlayer
        {
            public interface UpdateConfigInterface
            {
                IEnumerable<Field> PlayerFields { get; }
                string PlayerId { get; }
                string RoomId { get; }
            }

            private static readonly Dictionary<Player, MyPlayerUnity> _unityPlayers = new();

            internal static IEnumerable<MyNet.Field> GetFields(string nickname)
            {
                yield return new()
                {
                    key = MyNet.PlayerPropertyNickname,
                    value = nickname,
                    visibility = Field.VisibilityEnum.Public,
                };
            }

            public static MyPlayerInterface GetOrCreate(Player player, Func<MyPlayerUnity> onFallback)
            {
                if (_unityPlayers.TryGetValue(player, out var value))
                    return value;

                value = onFallback?.Invoke();
                _unityPlayers[player] = value;
                return value;
            }

            public static void StartUpdate(UpdateConfigInterface config, Action<Lobby> onOk = default, Action onFailed = default, Action<LobbyServiceException> onException = default)
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
