using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

            private static bool _isBusy;
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
                value.Player = player;

                _sessionPlayers[player.Id] = value;
                return value;
            }

            private static async Task RunBusyOperationAsync(Func<Task> operation, Action onBusy, Action<MyNetSessionException> onException)
            {
                if (_isBusy)
                {
                    onBusy?.Invoke();
                    return;
                }

                _isBusy = true;

                try
                {
                    await operation();
                }
                catch (SessionException e)
                {
                    onException?.Invoke(MyNet.ToException(e));
                }
                finally
                {
                    _isBusy = false;
                }
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

            public static async Task UpdateAsync(UpdateConfigInterface config, Action onOk = default, Action onBusy = default, Action onFailed = default, Action<MyNetSessionException> onException = default)
            {
                if (string.IsNullOrWhiteSpace(config.PlayerId))
                {
                    // TODO: 뭔가 해야함
                    return;
                }

                await RunBusyOperationAsync(async () =>
                {
                    if (MultiplayerService.Instance.Sessions.TryGetValue(config.RoomId, out var session))
                    {
                        if (session.CurrentPlayer?.Id == config.PlayerId)
                        {
                            session.CurrentPlayer.SetProperties(MyNet.ToPlayerProperties(config.PlayerFields));

                            await session.SaveCurrentPlayerDataAsync();

                            onOk?.Invoke();
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        onFailed?.Invoke();
                    }
                }, onBusy, onException);
            }
        }
    }
}
