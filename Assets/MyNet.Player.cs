using System;
using System.Collections.Generic;
using Unity.Services.Multiplayer;
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

            private static readonly Dictionary<string, InternalPlayerSession> _sessionPlayers = new();
            private static readonly Dictionary<Unity.Services.Lobbies.Models.Player, InternalPlayerUnity> _unityPlayers = new();

            internal static IEnumerable<MyNet.Field> GetFields(string nickname)
            {
                if (string.IsNullOrWhiteSpace(nickname) == false)
                {
                    yield return new()
                    {
                        key = MyNet.PlayerPropertyNickname,
                        value = nickname,
                        visibility = Field.VisibilityEnum.Public,
                    };
                }
            }

            internal static MyNetPlayerInterface GetOrCreate(IReadOnlyPlayer player, Func<InternalPlayerSession> onFallback)
            {
                if (player == default)
                    return default;

                if (_sessionPlayers.TryGetValue(player.Id, out var value))
                {
                    value.Player = player;
                    return value;
                }

                value = onFallback?.Invoke();

                _sessionPlayers[player.Id] = value;
                return value;
            }

            internal static MyNetPlayerInterface GetOrCreate(Unity.Services.Lobbies.Models.Player player, Func<InternalPlayerUnity> onFallback)
            {
                if (player == default)
                    return default;

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

            internal static void UpdateNickname(InternalPlayerSession player, string nickname, Action onOk = default, Action onFailed = default, Action<MyNetSessionException> onException = default)
            {
                var go = new GameObject(nameof(InternalPlayerSelfUpdater), typeof(InternalPlayerSelfUpdater));
                var c = go.GetComponent<InternalPlayerSelfUpdater>();
                c.PlayerProperties = MyNet.ToPlayerProperties(MyNet.Player.GetFields(nickname));
                c.Session = player.Session;

                c.OnException += onException;
                c.OnFailed += onFailed;
                c.OnOk += onOk;
            }
        }
    }
}
