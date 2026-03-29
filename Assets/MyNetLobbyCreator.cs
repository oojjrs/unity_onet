using System;
using System.Linq;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace oojjrs.onet
{
    public class MyNetLobbyCreator : MonoBehaviour
    {
        public MyNet.Lobby.CreateConfigInterface Config { get; set; }

        public event Action<LobbyServiceException> OnException;
        public event Action OnFailed;
        public event Action<Lobby> OnOk;

        private async void Start()
        {
            try
            {
                var lobby = await LobbyService.Instance.CreateLobbyAsync(Config.Title, Config.MaxPlayers, new()
                {
                    Data = Config.LobbyFields.ToDictionary(t => t.key, t => ToLobbyDataObject(t)),
                    IsPrivate = Config.IsPrivate,
                    Player = new(id: Config.Account, data: Config.PlayerFields.ToDictionary(t => t.key, t => MyNet.Lobby.ToPlayerDataObject(t))),
                });

                if (this != default)
                {
                    if (lobby != default)
                        OnOk?.Invoke(lobby);
                    else
                        OnFailed?.Invoke();
                }
            }
            catch (LobbyServiceException e)
            {
                OnException?.Invoke(e);
            }

            if (this != default)
                MyNet.Lobby.StopCreate();

            DataObject ToLobbyDataObject(MyNet.Lobby.Field field)
            {
                return new DataObject(Convert(field.visibility), field.value);

                DataObject.VisibilityOptions Convert(MyNet.Lobby.Field.VisibilityEnum e)
                {
                    return e switch
                    {
                        MyNet.Lobby.Field.VisibilityEnum.Public => DataObject.VisibilityOptions.Public,
                        MyNet.Lobby.Field.VisibilityEnum.Member => DataObject.VisibilityOptions.Member,
                        MyNet.Lobby.Field.VisibilityEnum.Private => DataObject.VisibilityOptions.Private,
                        _ => HandleException(e),
                    };

                    DataObject.VisibilityOptions HandleException(MyNet.Lobby.Field.VisibilityEnum e)
                    {
                        Debug.LogWarning($"{name}> UNEXPECTED VALUE: {e}. FALLING BACK TO PUBLIC.");
                        return DataObject.VisibilityOptions.Public;
                    }
                }
            }
        }
    }
}
