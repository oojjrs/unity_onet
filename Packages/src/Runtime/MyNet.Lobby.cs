using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

            private static bool _isBusy;
            private static readonly Dictionary<string, InternalRoomSessionStub> _rooms = new();
            private static CancellationTokenSource _stopCancellationTokenSource;
            private static GameObject _updater;
            private static TaskCompletionSource<bool> _updateRequestedTcs;

            internal static MyNetRoomInterface GetOrCreate(ISessionInfo session)
            {
                if (session is null)
                    return null;

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
                if (_updater != null)
                {
                    var c = _updater.GetComponent<InternalLobbyUpdater>();
                    if (c != null)
                        c.UpdateRequested = true;
                }

                _updateRequestedTcs?.TrySetResult(true);
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

            [Obsolete("Use UpdateAsync instead.")]
            public static void StartUpdate(float updateIntervalSeconds = 5, Action<IEnumerable<MyNetRoomInterface>> onUpdate = null, Action<MyNetException> onException = null)
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
                if (_updater != null)
                {
                    UnityEngine.Object.Destroy(_updater);

                    _updater = null;
                }

                _stopCancellationTokenSource?.Cancel();
            }

            public static async Task UpdateAsync(UpdateConfigInterface config, MyNetLobbyCallbacksInterface callbacks)
            {
                await RunBusyOperationAsync(async () =>
                {
                    _stopCancellationTokenSource = new();
                    _updateRequestedTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

                    var lcts = CancellationTokenSource.CreateLinkedTokenSource(config.CancellationToken, _stopCancellationTokenSource.Token);
                    while (lcts.IsCancellationRequested == false)
                    {
                        try
                        {
                            var results = await MultiplayerService.Instance.QuerySessionsAsync(new()
                            {
                            });

                            if (lcts.IsCancellationRequested)
                                break;

                            callbacks?.OnOk(results.Sessions.Select(session => MyNet.Lobby.GetOrCreate(session)));
                        }
                        catch (SessionException e)
                        {
                            callbacks?.OnException(MyNet.ToException(e));
                        }

                        var delayTask = Task.Delay(TimeSpan.FromSeconds(config.PollingDelaySeconds), lcts.Token);
                        var completedTask = await Task.WhenAny(delayTask, _updateRequestedTcs.Task);
                        if (completedTask == _updateRequestedTcs.Task)
                            _updateRequestedTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
                    }

                    _stopCancellationTokenSource = null;
                    _updateRequestedTcs = null;
                }, callbacks);
            }
        }
    }
}
