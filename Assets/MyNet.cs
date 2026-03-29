using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace oojjrs.onet
{
    public static class MyNet
    {
        public static class Lobby
        {
            // TODO: 아직 indexing 기능은 지원하지 않...
            public struct Field
            {
                public enum VisibilityEnum
                {
                    Public,
                    Member,
                    Private,
                }

                public string key;
                public string value;
                public VisibilityEnum visibility;
            }

            public interface CreateConfigInterface
            {
                string Account { get; }
                IEnumerable<Field> LobbyFields { get; }
                bool IsPrivate { get; }
                int MaxPlayers { get; }
                IEnumerable<Field> PlayerFields { get; }
                string Title { get; }
            }

            public interface JoinConfigInterface
            {
                string Account { get; }
                string LobbyId { get; }
                IEnumerable<Field> PlayerFields { get; }
            }

            private static GameObject _creator;
            private static GameObject _joiner;
            private static GameObject _updater;

            public static void RequestUpdate()
            {
                if (_updater != default)
                {
                    var c = _updater.GetComponent<MyNetLobbyUpdater>();
                    c.UpdateRequested = true;
                }
            }

            public static void StartCreate(CreateConfigInterface config, Action<Unity.Services.Lobbies.Models.Lobby> onOk = default, Action onFailed = default, Action<LobbyServiceException> onException = default)
            {
                StopCreate();

                var go = new GameObject(nameof(MyNetLobbyCreator), typeof(MyNetLobbyCreator));
                var c = go.GetComponent<MyNetLobbyCreator>();
                c.Config = config;
                c.OnOk += onOk;
                c.OnFailed += onFailed;
                c.OnException += onException;

                _creator = go;
            }

            public static void StartJoin(JoinConfigInterface config, Action<Unity.Services.Lobbies.Models.Lobby> onOk = default, Action onFailed = default, Action<LobbyServiceException> onException = default)
            {
                StopJoin();

                var go = new GameObject(nameof(MyNetLobbyJoiner), typeof(MyNetLobbyJoiner));
                var c = go.GetComponent<MyNetLobbyJoiner>();
                c.Config = config;
                c.OnOk += onOk;
                c.OnFailed += onFailed;
                c.OnException += onException;

                _joiner = go;
            }

            public static void StartUpdate(float updateIntervalSeconds = 5, Action<List<Unity.Services.Lobbies.Models.Lobby>> onUpdate = default, Action<LobbyServiceException> onException = default)
            {
                StopUpdate();

                var go = new GameObject(nameof(MyNetLobbyUpdater), typeof(MyNetLobbyUpdater));
                var c = go.GetComponent<MyNetLobbyUpdater>();
                c.UpdateIntervalSeconds = updateIntervalSeconds;
                c.OnException += onException;
                c.OnUpdate += onUpdate;

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

            internal static PlayerDataObject ToPlayerDataObject(MyNet.Lobby.Field field)
            {
                return new PlayerDataObject(Convert(field.visibility), field.value);

                static PlayerDataObject.VisibilityOptions Convert(MyNet.Lobby.Field.VisibilityEnum e)
                {
                    return e switch
                    {
                        MyNet.Lobby.Field.VisibilityEnum.Public => PlayerDataObject.VisibilityOptions.Public,
                        MyNet.Lobby.Field.VisibilityEnum.Member => PlayerDataObject.VisibilityOptions.Member,
                        MyNet.Lobby.Field.VisibilityEnum.Private => PlayerDataObject.VisibilityOptions.Private,
                        _ => HandleException(e),
                    };

                    static PlayerDataObject.VisibilityOptions HandleException(MyNet.Lobby.Field.VisibilityEnum e)
                    {
                        Debug.LogWarning($"{typeof(MyNet).Namespace}> UNEXPECTED VALUE: {e}. FALLING BACK TO PUBLIC.");
                        return PlayerDataObject.VisibilityOptions.Public;
                    }
                }
            }
        }
    }
}
