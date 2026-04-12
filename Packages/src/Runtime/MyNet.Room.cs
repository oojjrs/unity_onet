using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static class Room
        {
            public interface CreateConfigInterface
            {
                CancellationToken CancellationToken { get; }
                bool IsLocked { get; }
                bool IsPrivate { get; }
                int MaxPlayers { get; }
                string Password { get; }
                IEnumerable<Field> PlayerFields { get; }
                string PlayerNickname { get; }
                IEnumerable<Field> RoomFields { get; }
                string Title { get; }
            }

            public interface ExitConfigInterface
            {
                CancellationToken CancellationToken { get; }
                string PlayerId { get; }
                string RoomId { get; }
            }

            public interface JoinConfigInterface
            {
                string Account { get; }
                CancellationToken CancellationToken { get; }
                string Code { get; }
                string Password { get; }
                IEnumerable<Field> PlayerFields { get; }
                string PlayerNickname { get; }
                string RoomId { get; }
            }

            public interface UpdateConfigInterface
            {
                CancellationToken CancellationToken { get; }
                bool IsPrivate { get; }
                IEnumerable<Field> RoomFields { get; }
                string RoomId { get; }
            }

            private static GameObject _creator;
            private static GameObject _heartbeat;
            private static bool _isBusy;
            private static GameObject _joiner;
            private static readonly Dictionary<string, InternalRoomSession> _sessionRooms = new();
            private static readonly Dictionary<Unity.Services.Lobbies.Models.Lobby, InternalRoomUnity> _unityRooms = new();
            private static GameObject _updater;

            public static async Task CreateAsync(CreateConfigInterface config, MyNetRoomCallbacksInterface callbacks)
            {
                await RunBusyOperationAsync(async () =>
                {
                    var session = await MultiplayerService.Instance.CreateSessionAsync(new()
                    {
                        IsLocked = config.IsLocked,
                        IsPrivate = config.IsPrivate,
                        MaxPlayers = config.MaxPlayers,
                        Name = config.Title,
                        Password = config.Password,
                        PlayerProperties = MyNet.ToPlayerProperties(config.PlayerFields, config.PlayerNickname),
                        SessionProperties = MyNet.ToSessionProperties(config.RoomFields),
                    });

                    if (config.CancellationToken.IsCancellationRequested == false)
                    {
                        if (session != default)
                            callbacks?.OnOk(MyNet.Room.GetOrCreate(session));
                        else
                            callbacks?.OnFailed(MyNetCallbacksInterface.FailureEnum.NotFoundRoom);
                    }
                }, callbacks);
            }

            internal static MyNetRoomInterface GetOrCreate(Unity.Services.Lobbies.Models.Lobby lobby)
            {
                if (lobby == default)
                    return default;

                if (_unityRooms.TryGetValue(lobby, out var value))
                    return value;

                value = new(lobby);
                _unityRooms[lobby] = value;
                return value;
            }

            internal static MyNetRoomInterface GetOrCreate(ISession session)
            {
                if (session == default)
                    return default;

                if (_sessionRooms.TryGetValue(session.Id, out var value))
                {
                    value.Session = session;
                    return value;
                }

                value = new()
                {
                    Session = session
                };

                _sessionRooms[session.Id] = value;
                return value;
            }

            public static async Task JoinByCodeAsync(JoinConfigInterface config, MyNetRoomCallbacksInterface callbacks)
            {
                if (string.IsNullOrWhiteSpace(config.Code))
                {
                    callbacks?.OnFailed(MyNetCallbacksInterface.FailureEnum.EmptyCode);
                    return;
                }

                await RunBusyOperationAsync(async () =>
                {
                    var session = await MultiplayerService.Instance.JoinSessionByCodeAsync(config.Code, new()
                    {
                        PlayerProperties = MyNet.ToPlayerProperties(config.PlayerFields, config.PlayerNickname),
                    });

                    if (config.CancellationToken.IsCancellationRequested == false)
                    {
                        if (session != default)
                            callbacks?.OnOk(MyNet.Room.GetOrCreate(session));
                        else
                            callbacks?.OnFailed(MyNetCallbacksInterface.FailureEnum.NotFoundRoom);
                    }
                }, callbacks);
            }

            public static async Task JoinByIdAsync(JoinConfigInterface config, MyNetRoomCallbacksInterface callbacks)
            {
                if (string.IsNullOrWhiteSpace(config.RoomId))
                {
                    callbacks?.OnFailed(MyNetCallbacksInterface.FailureEnum.EmptyRoomId);
                    return;
                }

                await RunBusyOperationAsync(async () =>
                {
                    var session = await MultiplayerService.Instance.JoinSessionByIdAsync(config.RoomId, new()
                    {
                        Password = config.Password,
                        PlayerProperties = MyNet.ToPlayerProperties(config.PlayerFields, config.PlayerNickname),
                    });

                    if (config.CancellationToken.IsCancellationRequested == false)
                    {
                        if (session != default)
                            callbacks?.OnOk(MyNet.Room.GetOrCreate(session));
                        else
                            callbacks?.OnFailed(MyNetCallbacksInterface.FailureEnum.NotFoundRoom);
                    }
                }, callbacks);
            }

            // Kick은 여러번 들어올 수 있으므로 Busy에 엮지 않는다.
            public static async Task KickAsync(ExitConfigInterface config, MyNetExitCallbacksInterface callbacks)
            {
                if (string.IsNullOrWhiteSpace(config.RoomId))
                {
                    callbacks?.OnFailed(MyNetCallbacksInterface.FailureEnum.EmptyRoomId);
                    return;
                }

                try
                {
                    if (MultiplayerService.Instance.Sessions.TryGetValue(config.RoomId, out var session))
                    {
                        await session.AsHost().RemovePlayerAsync(config.PlayerId);

                        if (config.CancellationToken.IsCancellationRequested == false)
                            callbacks?.OnOk(config.RoomId, config.PlayerId);
                    }
                    else
                    {
                        callbacks?.OnFailed(MyNetCallbacksInterface.FailureEnum.NotFoundRoom);
                    }
                }
                catch (SessionException e)
                {
                    callbacks?.OnException(MyNet.ToException(e));
                }
            }

            public static async Task LeaveAsync(ExitConfigInterface config, MyNetExitCallbacksInterface callbacks)
            {
                if (string.IsNullOrWhiteSpace(config.RoomId))
                {
                    callbacks?.OnFailed(MyNetCallbacksInterface.FailureEnum.EmptyRoomId);
                    return;
                }

                await RunBusyOperationAsync(async () =>
                {
                    if (MultiplayerService.Instance.Sessions.TryGetValue(config.RoomId, out var session))
                    {
                        if (config.PlayerId == session.CurrentPlayer.Id)
                        {
                            await session.LeaveAsync();

                            if (config.CancellationToken.IsCancellationRequested == false)
                                callbacks?.OnOk(config.RoomId, config.PlayerId);
                        }
                        else
                        {
                            callbacks?.OnFailed(MyNetCallbacksInterface.FailureEnum.NotPermitted);
                        }
                    }
                    else
                    {
                        callbacks?.OnFailed(MyNetCallbacksInterface.FailureEnum.NotFoundRoom);
                    }
                }, callbacks);
            }

            private static async Task RunBusyOperationAsync(Func<Task> operation, MyNetCallbacksInterface callbacks)
            {
                if (_isBusy)
                {
                    callbacks?.OnBusy();
                    return;
                }

                _isBusy = true;

                try
                {
                    await operation();
                }
                catch (SessionException e)
                {
                    callbacks?.OnException(MyNet.ToException(e));
                }
                finally
                {
                    _isBusy = false;
                }
            }

            [Obsolete("Use CreateAsync instead.")]
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
            [Obsolete("Use LeaveAsync or KickAsync instead.")]
            public static void StartExit(ExitConfigInterface config, Action<string, string> onOk = default, Action<MyNetException> onException = default)
            {
                var go = new GameObject(nameof(InternalRoomExiter), typeof(InternalRoomExiter));
                var c = go.GetComponent<InternalRoomExiter>();
                c.Config = config;

                c.OnException += onException;
                c.OnOk += onOk;
            }

            // 샘플들이 다 15초라서 그냥 따라함.
            [Obsolete("Don't use anymore")]
            public static void StartHeartbeat(float heartbeatIntervalSeconds = 15, float errorIntervalSeconds = 5)
            {
                var go = new GameObject(nameof(InternalRoomHeartbeat), typeof(InternalRoomHeartbeat));
                var c = go.GetComponent<InternalRoomHeartbeat>();
                c.ErrorIntervalSeconds = errorIntervalSeconds;
                c.HeartbeatIntervalSeconds = heartbeatIntervalSeconds;

                _heartbeat = go;
            }

            [Obsolete("Use JoinByCodeAsync or JoinByIdAsync instead.")]
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

            [Obsolete("Use UpdateAsync instead.")]
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

            [Obsolete("Don't use anymore")]
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

            public static async Task UpdateAsync(UpdateConfigInterface config, MyNetRoomCallbacksInterface callbacks)
            {
                if (string.IsNullOrWhiteSpace(config.RoomId))
                {
                    callbacks?.OnFailed(MyNetCallbacksInterface.FailureEnum.EmptyRoomId);
                    return;
                }

                await RunBusyOperationAsync(async () =>
                {
                    if (MultiplayerService.Instance.Sessions.TryGetValue(config.RoomId, out var session))
                    {
                        session.AsHost().IsPrivate = config.IsPrivate;
                        session.AsHost().SetProperties(MyNet.ToSessionProperties(config.RoomFields));

                        await session.AsHost().SavePropertiesAsync();

                        if (config.CancellationToken.IsCancellationRequested == false)
                            callbacks?.OnOk(MyNet.Room.GetOrCreate(session));
                    }
                    else
                    {
                        callbacks?.OnFailed(MyNetCallbacksInterface.FailureEnum.NotFoundRoom);
                    }


                }, callbacks);
            }
        }
    }
}
